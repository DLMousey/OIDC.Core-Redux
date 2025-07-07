using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.Services.Interface;

namespace OIDC.Core_Redux.Services.Implementation;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public string GenerateJwt(User user)
    {
        if (SigningKey == null)
        {
            throw new ApplicationException("JWT:SigningKey is missing");
        }
        
        IList<string> roleNames = user.Roles.Select(r => r.Name).ToList();
        
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateJwt(AccessToken accessToken)
    {
        if (SigningKey == null)
        {
            throw new ApplicationException("JWT:SigningKey is missing");
        }
        
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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
                new("username", accessToken.User.Username),
                new("clientId", accessToken.Application.ClientId),
                new("roles", string.Join(", ", roleNames.ToArray()))
            },
            expires: accessToken.ExpiresAt,
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string? SigningKey => configuration.GetValue<string>("JWT:SigningKey");
}