using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL.Configuration;

namespace OIDC.Core_Minimal.DAL.Entities;

[EntityTypeConfiguration(typeof(UserConfiguration))]
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Email { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    public string Username { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public ICollection<Application> PublishedApplications { get; set; }

    [JsonIgnore]
    public ICollection<Application> AuthorisedApplications { get; set; }

    [JsonIgnore]
    public ICollection<AccessToken> AccessTokens { get; set; }

    [JsonIgnore]
    public ICollection<RefreshToken> RefreshTokens { get; set; }
}