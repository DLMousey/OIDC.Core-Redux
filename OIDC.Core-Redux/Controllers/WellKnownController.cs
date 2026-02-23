using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Redux.DAL.ViewModels.Configuration;
using OIDC.Core_Redux.DAL.ViewModels.Controllers.WellKnownController;
using OIDC.Core_Redux.Services.Interface;

namespace OIDC.Core_Redux.Controllers;

[ApiController]
[Route("/.well-known")]
[AllowAnonymous]
public class WellKnownController(
    IConfiguration configuration, 
    IScopeService scopeService
) : ControllerBase
{
    
    [HttpGet("openid-configuration")]
    public IActionResult OpenIdConfiguration()
    {
        try
        {
            return Ok(new OpenIdConnectConfiguration(configuration));
        }
        catch
        {
            return StatusCode(500, "An error occurred while fetching the OIDC config");
        }
    }

    [HttpGet("jwks")]
    public IActionResult Jwks()
    {
        IConfigurationSection section = configuration.GetSection("OIDC:JWKS");
        IList<JsonWebKeyViewModel> entries = section.Get<IList<JsonWebKeyViewModel>>() ?? new List<JsonWebKeyViewModel>();
        
        return Ok(new
        {
            keys = entries
        });
    }
}