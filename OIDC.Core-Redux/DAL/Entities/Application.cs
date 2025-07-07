using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core_Redux.DAL.Configuration;

namespace OIDC.Core_Redux.DAL.Entities;

[EntityTypeConfiguration(typeof(ApplicationConfiguration))]
public class Application
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(32)]
    public string Name { get; set; }

    public string HomepageUrl { get; set; }
    
    public string CallbackUrl { get; set; }

    public string? CancelUrl { get; set; }

    [MaxLength(24)]
    public string ClientId { get; set; } = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(16));

    [JsonIgnore]
    [MaxLength(88)]
    public string ClientSecret { get; set; } = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

    [JsonIgnore]
    public User User { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<AccessToken> AccessTokens { get; set; }

    [JsonIgnore]
    public ICollection<User> Users { get; set; }
}