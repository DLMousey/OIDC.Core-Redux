using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL.Configuration;

namespace OIDC.Core_Minimal.DAL.Entities;

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
    
    [JsonIgnore]
    public ICollection<Scope> Scopes { get; set; }
}