using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OIDC.Core.DAL.Entities;

namespace OIDC.Core.DAL.Configuration;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasOne<User>(a => a.User)
            .WithMany(u => u.PublishedApplications)
            .HasForeignKey(a => a.UserId);

        builder.HasMany<User>(a => a.Users)
            .WithMany(u => u.AuthorisedApplications);

        builder.HasMany<AccessToken>(a => a.AccessTokens)
            .WithOne(at => at.Application)
            .HasForeignKey(at => at.ApplicationId);
    }
}