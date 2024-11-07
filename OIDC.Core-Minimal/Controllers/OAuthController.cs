using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Minimal.DAL.Strategies;

namespace OIDC.Core_Minimal.Controllers;

[ApiController]
[Route("/oauth")]
[AllowAnonymous]
public class OAuthController : ControllerBase
{
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

        switch (strategy)
        {
            case OAuthStrategiser.Strategy.AuthorizationCode:
                return await AuthorisationCode(strategiser);
            case OAuthStrategiser.Strategy.ClientCredentials:
                return await ClientCredentials(strategiser);
            case OAuthStrategiser.Strategy.PasswordGrant:
                return await PasswordGrant(strategiser);
            case OAuthStrategiser.Strategy.RefreshToken:
                return await RefreshTokenGrant(strategiser);
            default:
                return BadRequest("Unknown oauth grant type requested");
        }
    }

    private async Task<IActionResult> AuthorisationCode(OAuthStrategiser strategiser)
    {
        return Ok("You requested an authorisation code");
    }

    private async Task<IActionResult> ClientCredentials(OAuthStrategiser strategiser)
    {
        return Ok("You requested a client credentials grant");
    }

    private async Task<IActionResult> PasswordGrant(OAuthStrategiser strategiser)
    {
        return Ok("You requested a password grant");
    }

    private async Task<IActionResult> RefreshTokenGrant(OAuthStrategiser strategiser)
    {
        return Ok("You requested a refresh token grant");
    }
}