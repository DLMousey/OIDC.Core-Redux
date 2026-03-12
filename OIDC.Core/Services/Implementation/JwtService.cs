using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core.DAL.Entities;
using OIDC.Core.DAL.ViewModels.Configuration;
using OIDC.Core.DAL.ViewModels.Controllers.WellKnownController;
using OIDC.Core.Services.Interface;

namespace OIDC.Core.Services.Implementation;

public class JwtService(
    IConfiguration configuration,
    IJwksKeyService jwksKeyService
) : IJwtService
{
    public string GenerateJwt(User user)
    {
        IList<string> roleNames = user.Roles.Select(r => r.Name).ToList();
        SigningCredentials creds = new SigningCredentials(jwksKeyService.GetSecurityKey(), SecurityAlgorithms.RsaSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("JWT:Issuer"),
            audience: configuration.GetValue<string>("JWT:Audience"),
            claims: new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)),
                new("username", user.Username),
                new("roles", string.Join(", ", roleNames.ToArray()))
            },
            expires: DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("JWT:ExpirationMinutes", 15)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateJwt(AccessToken accessToken, string? nonce)
    {
        SigningCredentials creds = new SigningCredentials(jwksKeyService.GetSecurityKey(), SecurityAlgorithms.RsaSha256);
        IList<string> roleNames = accessToken.User.Roles.Select(r => r.Name).ToList();
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("JWT:Issuer"),
            audience: configuration.GetValue<string>("JWT:Audience"),
            claims: new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, accessToken.User.Email),
                new(JwtRegisteredClaimNames.Sub, accessToken.UserId.ToString()),
                new(JwtRegisteredClaimNames.AuthTime,
                    accessToken.ExpiresAt.ToString("o", CultureInfo.InvariantCulture)),
                new(JwtRegisteredClaimNames.Iss, configuration.GetValue<string>("JWT:Issuer")),
                new(JwtRegisteredClaimNames.Aud, accessToken.Application.ClientId),
                new("username", accessToken.User.Username),
                new("clientId", accessToken.Application.ClientId),
                new("roles", string.Join(", ", roleNames.ToArray())),
                new Claim(JwtRegisteredClaimNames.Exp, accessToken.ExpiresAt.ToString("o", CultureInfo.InvariantCulture)),
                new Claim("name",  accessToken.User.Username),
                new Claim("email", accessToken.User.Email),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Nonce, nonce)
            },
            expires: accessToken.ExpiresAt,
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string? SigningKey => configuration.GetValue<string>("JWT:SigningKey");
    
    private Dictionary<string, string> SigningKeyJwks()
    {
        List<JwksKeysConfiguration> publicKeyConfigs = configuration.GetSection("OIDC:JWKS").Get<List<JwksKeysConfiguration>>();
        List<JwksKeysConfiguration> privateKeyConfigs = configuration.GetSection("OIDC:PrivateKeys").Get<List<JwksKeysConfiguration>>();

        Guid intendedKey = Guid.Parse("dcb81ddb-db87-43b4-8bda-efee2a3ecad9");
        string publicKey = publicKeyConfigs.First(pb => pb.KeyId.Equals(intendedKey)).KeyMaterial;
        string privateKey = privateKeyConfigs.First(pk => pk.KeyId.Equals(intendedKey)).KeyMaterial;

        return new Dictionary<string, string>
        {
            { "public", publicKey },
            { "private", privateKey }
        };
    }
}