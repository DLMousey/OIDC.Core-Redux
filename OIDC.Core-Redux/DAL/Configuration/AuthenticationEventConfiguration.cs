using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux.DAL.Configuration;

public class AuthenticationEventConfiguration : IEntityTypeConfiguration<AuthenticationEvent>
{
    public void Configure(EntityTypeBuilder<AuthenticationEvent> builder)
    {
        builder.HasOne<User>(a => a.User)
            .WithMany(u => u.AuthenticationEvents)
            .HasForeignKey(a => a.UserId);
    }
}