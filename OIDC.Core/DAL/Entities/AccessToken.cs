using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core.DAL.Configuration;

namespace OIDC.Core.DAL.Entities;

[EntityTypeConfiguration(typeof(AccessTokenConfiguration))]
public class AccessToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public User User { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }

    public Application Application { get; set; }

    public Guid ApplicationId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(3600);

    public string Code { get; set; } = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

    [JsonIgnore]
    public ICollection<Scope> Scopes { get; set; }
}