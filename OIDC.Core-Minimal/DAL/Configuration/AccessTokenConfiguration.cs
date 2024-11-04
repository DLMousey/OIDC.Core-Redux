using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.DAL.Configuration;

public class AccessTokenConfiguration : IEntityTypeConfiguration<AccessToken>
{
    public void Configure(EntityTypeBuilder<AccessToken> builder)
    {
        builder.HasMany<Scope>(at => at.Scopes)
            .WithMany(s => s.AccessTokens);

        builder.HasOne<Application>(at => at.Application)
            .WithMany(a => a.AccessTokens)
            .HasForeignKey(at => at.ApplicationId);
        
        builder.HasOne<User>(at => at.User)
            .WithMany(u => u.AccessTokens)
            .HasForeignKey(at => at.UserId);
    }
}