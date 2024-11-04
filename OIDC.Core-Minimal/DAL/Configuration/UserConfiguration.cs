using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.DAL.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany<Application>(u => u.PublishedApplications)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);

        builder.HasMany<Application>(u => u.AuthorisedApplications)
            .WithMany(a => a.Users);

        builder.HasMany<AccessToken>(u => u.AccessTokens)
            .WithOne(at => at.User)
            .HasForeignKey(at => at.UserId);
    }
}