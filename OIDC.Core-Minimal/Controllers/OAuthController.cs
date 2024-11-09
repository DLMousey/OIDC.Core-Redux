using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.Strategies;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.OAuthController;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.UserController;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Controllers;

[ApiController]
[Route("/oauth")]
[AllowAnonymous]
public class OAuthController(
    IApplicationService applicationService,
    IUserService userService,
    IAccessTokenService accessTokenService,
    IScopeService scopeService,
    IDistributedCache cache
) : ControllerBase
{
    #region catch-all
    [EndpointDescription("Catch all endpoint for authorisation requests, dynamically determines grant type")]
    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> AuthoriseAsync()
    {
        if (!Request.QueryString.HasValue)
        {
            return BadRequest("No query parameters specified");
        }
        
        OAuthStrategiser strategiser = new OAuthStrategiser(Request.QueryString.Value);
        OAuthStrategiser.Strategy strategy = strategiser.DetermineStrategy();

        if (strategiser.ClientId == null)
        {
            return BadRequest("No client id provided");
        }
        
        Application? application = await applicationService.FindAsync(strategiser.ClientId);

        if (application == null)
        {
            return BadRequest("Invalid client id provided");
        }
        
        switch (strategy)
        {
            case OAuthStrategiser.Strategy.AuthorizationCode:
                return await AuthorisationCode(strategiser, application);
            case OAuthStrategiser.Strategy.AuthorizationCodeExchange:
                return await AuthorisationCodeExchange(strategiser, application);
            case OAuthStrategiser.Strategy.ClientCredentials:
                return await ClientCredentials(strategiser, application);
            case OAuthStrategiser.Strategy.PasswordGrant:
                return await PasswordGrant(strategiser, application);
            case OAuthStrategiser.Strategy.RefreshToken:
                return await RefreshTokenGrant(strategiser, application);
            default:
                return BadRequest("Unknown oauth grant type requested");
        }
    }

    [EndpointDescription("Catch all endpoint for token exchanges, dynamically determines grant type")]
    [HttpPost("token")]
    public async Task<IActionResult> TokenAsync()
    {
        OAuthStrategiser strategiser;
        if (Request.QueryString.HasValue)
        {
            strategiser = new OAuthStrategiser(Request.QueryString.Value);
        }
        else
        {
            if (!Request.Body.CanSeek)
            {
                Request.EnableBuffering();
            }

            Request.Body.Position = 0;
            Dictionary<string, string>? pairs = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(Request.Body);

            if (pairs == null)
            {
                return BadRequest("Unable to parse request body");
            }
        
            strategiser = new OAuthStrategiser(pairs);
        }
        
        OAuthStrategiser.Strategy strategy = strategiser.DetermineStrategy();
        if (strategiser.ClientId == null)
        {
            return BadRequest("No client id provided");
        }
        
        Application? application = await applicationService.FindAsync(strategiser.ClientId);

        if (application == null)
        {
            return BadRequest("Invalid client id provided");
        }

        switch (strategy)
        {
            case OAuthStrategiser.Strategy.AuthorizationCodeExchange:
                return await AuthorisationCodeExchange(strategiser, application);
            case OAuthStrategiser.Strategy.ClientCredentials:
                return await ClientCredentials(strategiser, application);
            default:
                return BadRequest("Unknown oauth grant type requested");
        }
    }
    
    #endregion

    #region authorisation-code

    // Return the data required for the consent screen - consent is handled by ConsentResultAsync method
    private async Task<IActionResult> AuthorisationCode(OAuthStrategiser strategiser, Application application)
    {
        User user;

        try
        {
            user = await userService.GetFromContextAsync(User);
        }
        catch (Exception e)
        {
            return BadRequest("Failed to retrieve user from context");
        }

        AccessToken? existingAt = await accessTokenService.FindAsync(user, application, strategiser.Scopes);
        IList<Scope> scopes = await scopeService.GetScopesAsync(strategiser.Scopes);
        
        // No existing access token found (including scopes) - considering this a new request flow
        if (existingAt == null)
        {
            return Ok(new
            {
                application,
                scopes,
            });
        }
        
        // Existing access token found (including scopes) - adding authorised property so front end
        // can redirect the user straight back to the application
        return Ok(new
        {
            application,
            scopes,
            authorised = true
        });
    }
    
    [HttpPost("consent")]
    [Authorize]
    public async Task<IActionResult> ConsentResultAsync([FromBody] ConsentViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        User? user;
        try
        {
            user = await userService.GetFromContextAsync(User);
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to retrieve user from context");
        }
        
        Application? application = await applicationService.FindAsync(vm.ClientId);
        if (application == null)
        {
            return BadRequest("Invalid client id provided");
        }

        // User denied consent - redirect them back to either the cancel url if specified 
        // or the homepage of the application they were sent here from
        if (!vm.Consent)
        {
            return Redirect(application.CancelUrl ?? application.HomepageUrl);
        }
        
        // User gave consent - generate an authorisation code and store in cache, 
        // then redirect the user to the application's callback url
        string authorisationCode = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(16));
        ConsentPayload payload = new ConsentPayload
        {
            UserId = user.Id,
            ClientId = vm.ClientId,
            Scopes = vm.Scopes
        };

        string encodedPayload = JsonSerializer.Serialize(payload);
        DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(15));

        await cache.SetAsync(authorisationCode, Encoding.UTF8.GetBytes(encodedPayload), options);
        // return Redirect(application.CallbackUrl + "?code=" + authorisationCode + "&state=" + vm.State);
        return Ok(new
        {
            redirectUrl = application.CallbackUrl,
            code = authorisationCode,
            state = vm.State
        });
    }
    
    private async Task<IActionResult> AuthorisationCodeExchange(OAuthStrategiser strategiser, Application application)
    {
        if (strategiser.ClientSecret != application.ClientSecret || strategiser.RedirectUri != application.CallbackUrl)
        {
            return BadRequest("Invalid client id provided");
        }

        if (strategiser.Code == null)
        {
            return BadRequest("Must provide authorisation code");
        }

        // If it's not in cache the app either provided the wrong code or took too long to perform
        // the token exchange.
        byte[]? payloadRaw = await cache.GetAsync(strategiser.Code);
        string payloadString;
        if (payloadRaw == null)
        {
            return BadRequest("Invalid authorisation code provided");
        }
        else
        {
            payloadString = Encoding.UTF8.GetString(payloadRaw);
        }
        
        // If the payload is null after deserialisation it probably means something went severely
        // wrong on our end and we'll need the user to run through the consent flow again.
        ConsentPayload? payload = JsonSerializer.Deserialize<ConsentPayload>(payloadString);
        if (payload == null)
        {
            return new JsonResult("Malformed cache data, please perform authorisation flow again")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        User? user = await userService.FindByIdAsync(payload.UserId);
        if (user == null)
        {
            return BadRequest("Invalid user id provided");
        }
        
        // Generate an access token for the application to use this user's account with the scopes from the 
        // consent payload, then return it to the application
        IList<Scope> scopes = await scopeService.GetScopesAsync(payload.Scopes);
        AccessToken token = await accessTokenService.CreateAsync(user, application, scopes);

        // Purge the authorisation code from cache to prevent re-use
        await cache.RemoveAsync(strategiser.Code);
        
        return Ok(new
        {
            access_token = token.Code,
            expires_in = token.ExpiresAt.Subtract(DateTime.UtcNow).TotalSeconds
        });
    }

    #endregion

    #region client-credentials
    private async Task<IActionResult> ClientCredentials(OAuthStrategiser strategiser, Application application)
    {
        if (strategiser.ClientSecret != application.ClientSecret)
        {
            return BadRequest("Invalid client secret provided");
        }

        string prefix = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(12));
        string password = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        
        // Generate a system user for the client
        User user = await userService.CreateAsync(new CreateAsyncViewModel
        {
            Email = $"{prefix}@localhost",
            Password = password,
            Username = prefix
        });
        
        IList<Scope> scopes = await scopeService.GetScopesAsync(strategiser.Scopes);
        
        // Generate access token linked to system user and return immediately
        AccessToken accessToken = await accessTokenService.CreateAsync(user, application, scopes);
        
        return Ok(new
        {
            access_token = accessToken.Code,
            expires_in = accessToken.ExpiresAt.Subtract(DateTime.UtcNow).TotalSeconds
        });
    }
    
    #endregion

    [Obsolete("Password grant is no longer recommended - this endpoint will always return an error response")]
    private async Task<IActionResult> PasswordGrant(OAuthStrategiser strategiser, Application application)
    {
        return BadRequest("Password grant is no longer supported");
    }

    private async Task<IActionResult> RefreshTokenGrant(OAuthStrategiser strategiser, Application application)
    {
        return Ok("You requested a refresh token grant");
    }
}