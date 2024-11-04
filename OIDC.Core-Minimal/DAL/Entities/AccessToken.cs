using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL.Configuration;

namespace OIDC.Core_Minimal.DAL.Entities;

[EntityTypeConfiguration(typeof(AccessTokenConfiguration))]
public class AccessToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public User User { get; set; }

    public Guid UserId { get; set; }

    public Application Application { get; set; }

    public ICollection<Scope> Scopes { get; set; }

    public Guid ApplicationId { get; set; }
}