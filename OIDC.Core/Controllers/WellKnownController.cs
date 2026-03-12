using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core.DAL.ViewModels.Configuration;
using OIDC.Core.DAL.ViewModels.Controllers.WellKnownController;
using OIDC.Core.Services.Interface;

namespace OIDC.Core.Controllers;

[ApiController]
[Route("/.well-known")]
[AllowAnonymous]
public class WellKnownController(
    IConfiguration configuration, 
    IScopeService scopeService,
    IJwksKeyService jwksKeyService
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
        RSAParameters p = jwksKeyService.GetPublicKeyParameters();
        string keyId = jwksKeyService.GetKeyId();
        
        List<JsonWebKeyViewModel> keys = new List<JsonWebKeyViewModel>();
        
        keys.Add(new JsonWebKeyViewModel
        {
            KeyType = "RSA",
            KeyId = keyId,
            IntendedUse = "sig",
            Algorithm = "RS256",
            KeyMaterial = Base64UrlEncoder.Encode(p.Modulus),
            Exponent = Base64UrlEncoder.Encode(p.Exponent),
            Path = ""
        });

        return Ok(new { keys = keys });
    }
}