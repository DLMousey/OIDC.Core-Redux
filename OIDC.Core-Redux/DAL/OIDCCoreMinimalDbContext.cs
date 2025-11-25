using Microsoft.EntityFrameworkCore;
using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux.DAL;

public class OIDCCoreMinimalDbContext(DbContextOptions options) : DbContext(options)
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<Application> Applications { get; set; }
    public virtual DbSet<Scope> Scopes { get; set; }
    public virtual DbSet<AccessToken> AccessTokens { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<AuthenticationEvent> AuthenticationEvents { get; set; }
}