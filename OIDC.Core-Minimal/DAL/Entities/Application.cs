using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL.Configuration;

namespace OIDC.Core_Minimal.DAL.Entities;

[EntityTypeConfiguration(typeof(ApplicationConfiguration))]
public class Application
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public string Url { get; set; }

    public User User { get; set; }

    public Guid UserId { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public ICollection<AccessToken> AccessTokens { get; set; }

    public ICollection<User> Users { get; set; }
}