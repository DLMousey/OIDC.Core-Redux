using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.DAL.Configuration;

public class ScopeConfiguration : IEntityTypeConfiguration<Scope>
{
    public void Configure(EntityTypeBuilder<Scope> builder)
    {
        builder.HasKey(s => s.Name);

        builder.HasData(
            new Scope("profile.read"),
            new Scope("profile.write"),
            new Scope("applications.authorised"),
            new Scope("applications.published")
        );

        builder.HasMany<AccessToken>(s => s.AccessTokens)
            .WithMany(at => at.Scopes);
    }
}