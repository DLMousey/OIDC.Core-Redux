using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.DAL.Strategies;
using OIDC.Core_Redux.DAL.ViewModels.Controllers.OAuthController;
using OIDC.Core_Redux.DAL.ViewModels.Controllers.UserController;
using OIDC.Core_Redux.Services.Interface;
using OIDC.Core_Redux.Util.Metrics;
using IFormCollection = Microsoft.AspNetCore.Http.IFormCollection;

namespace OIDC.Core_Redux.Controllers;

[ApiController]
[Route("/oauth")]
[AllowAnonymous]
public class OAuthController(
    IApplicationService applicationService,
    IUserService userService,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IScopeService scopeService,
    IDistributedCache cache,
    IJwtService jwtService,
    APIEvents apiEvents,
    ILogger<OAuthController> logger
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
            apiEvents.RecordOAuthGrantUsage(strategiser.GrantType!, null, application, false);
            return BadRequest("Invalid client id provided");
        }
        
        apiEvents.RecordOAuthGrantUsage(strategiser.GrantType ?? "unknown/invalid", null, application);
        
        switch (strategy)
        {
            case OAuthStrategiser.Strategy.AuthorizationCode:
                return await AuthorisationCode(strategiser, application);
            case OAuthStrategiser.Strategy.AuthorizationCodeExchange:
                return await AuthorisationCodeExchange(strategiser, application);
            case OAuthStrategiser.Strategy.Pkce:
                return await PKCE(strategiser, application);
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
        Dictionary<string, string>? pairs = null;
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
            switch (Request.ContentType)
            {
                case "application/x-www-form-urlencoded":
                    pairs = new Dictionary<string, string>();
                    IFormCollection values = await Request.ReadFormAsync();
                    foreach (string valuesKey in values.Keys)
                    {
                        pairs[valuesKey] = values[valuesKey]!;
                    }
                    break;
                case "application/json":
                    pairs = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(Request.Body);
                    break;
            }
            
            if (pairs == null)
            {
                return BadRequest("Unable to parse request body");
            }
        
            strategiser = new OAuthStrategiser(pairs);
        }
        
        OAuthStrategiser.Strategy strategy = strategiser.DetermineStrategy();
        if (strategiser.ClientId == null)
        {
            // Sanity check pairs are set properly and the value is cached
            if (pairs == null || !pairs.TryGetValue("code", out var clientId))
            {
                return BadRequest("No client id provided");
            }
            
            // Try getting the client id from the authorisation request, otherwise fail
            string? lookup = await cache.GetStringAsync(clientId);
            if (lookup == null)
            {
                return BadRequest("No client id provided");
            }
            
            ConsentPayload? payload = JsonSerializer.Deserialize<ConsentPayload>(lookup);
            strategiser.ClientId = payload!.ClientId;
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
            case OAuthStrategiser.Strategy.PkceExchange:
                return await PKCEExchange(strategiser, application);
            case OAuthStrategiser.Strategy.RefreshToken:
                return await RefreshTokenGrant(strategiser, application);
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
        
        apiEvents.RecordOAuthGrantUsage("authorization_code-consent", user);
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
            Scopes = vm.Scopes,
            CodeChallenge = vm.CodeChallenge,
            CodeVerifier = vm.CodeVerifier
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
        
        // Add the user to the application's user list
        await applicationService.AddUser(application, user);
        
        return Ok(new
        {
            access_token = jwtService.GenerateJwt(token),
            expires_in = token.ExpiresAt.Subtract(DateTime.UtcNow).TotalSeconds
        });
    }

    #endregion
    
    #region PKCE

    private async Task<IActionResult> PKCE(OAuthStrategiser strategiser, Application application)
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
        
        apiEvents.RecordOAuthGrantUsage("pkce-consent", user);
        if (existingAt == null)
        {
            return Ok(new
            {
                application,
                scopes
            });
        }
        
        return Ok(new
        {
            application,
            scopes,
            authorised = true
        });
    }

    private async Task<IActionResult> PKCEExchange(OAuthStrategiser strategiser, Application application)
    {
        if (strategiser.ClientId != application.ClientId)
        {
            return BadRequest("Invalid client id provided");
        }

        if (strategiser.Code == null)
        {
            return BadRequest("Must provide authorisation code");
        }

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

        ConsentPayload? payload = JsonSerializer.Deserialize<ConsentPayload>(payloadString);
        if (payload == null)
        {
            return new JsonResult("Malformed cache data, please perform authorisation flow again")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        User? user = await userService.FindByIdAsync(payload.UserId);
        string codeChallenge = payload.CodeChallenge;
        string codeVerifier = payload.CodeVerifier;

        byte[] verifierBytes = Encoding.UTF8.GetBytes(codeVerifier);
        var verifierHash = SHA256.HashData(verifierBytes);

        if (Base64UrlEncoder.Encode(verifierHash) != codeChallenge)
        {
            return BadRequest("Invalid code verifier provided");
        }
        
        // Hash verifier with sha-256, check it matches the challenge from the cache
        
        IList<Scope> scopes = await scopeService.GetScopesAsync(strategiser.Scopes);
        AccessToken token = await accessTokenService.CreateAsync(user, application, scopes);

        // await cache.RemoveAsync(strategiser.Code);

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
        // @TODO - make this idempotent based off app id
        User user = await userService.CreateAsync(new CreateAsyncViewModel
        {
            Email = $"{prefix}@localhost",
            Password = password,
            Username = prefix
        });
        
        IList<Scope> scopes = await scopeService.GetScopesAsync(strategiser.Scopes);
        
        // Generate access token linked to system user and return immediately
        AccessToken accessToken = await accessTokenService.CreateAsync(user, application, scopes);
        apiEvents.RecordOAuthGrantUsage(strategiser.GrantType!, user, application);
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

    // Should assume any requests being made to this endpoint are anonymous and use the refresh token
    // as the source of truth on the user.
    private async Task<IActionResult> RefreshTokenGrant(OAuthStrategiser strategiser, Application application)
    {
        // Should never trigger this block since a non-null refresh token is required for the 
        // strategiser to correctly identify a refresh token grant attempt, but doesn't hurt
        // to be paranoid.
        if (strategiser.RefreshToken == null)
        {
            logger.LogInformation("Recorded oauth event failure");
            apiEvents.RecordOAuthGrantUsage(strategiser.GrantType!, null, application);
            return BadRequest("No refresh token provided");
        }
        
        // Deliberately obfuscated response to prevent this endpoint being used as a screening 
        // tool - should ideally log and count requests that make it here as they could
        // represent an attacker abusing the API
        RefreshToken? refreshToken = await refreshTokenService.FindAsync(strategiser.RefreshToken);
        if (refreshToken == null)
        {
            logger.LogInformation("Recorded oauth event failure");
            apiEvents.RecordOAuthGrantUsage(strategiser.GrantType!, null, application);
            return BadRequest("No refresh token provided");
        }

        string jwt = jwtService.GenerateJwt(refreshToken.User);
        await refreshTokenService.RecordUse(refreshToken);
        
        logger.LogInformation("Recorded oauth event success");
        apiEvents.RecordOAuthGrantUsage(strategiser.GrantType!, refreshToken.User, application);
        
        return Ok(new
        {
            access_token = jwt,
            expires_in = 300
        });
    }
}