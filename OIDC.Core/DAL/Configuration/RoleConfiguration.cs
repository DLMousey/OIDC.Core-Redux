using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OIDC.Core.DAL.Entities;

namespace OIDC.Core.DAL.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasMany<User>(r => r.Users)
            .WithMany(u => u.Roles);

        builder.HasData(
            new Role
            {
                Id = Guid.Parse("1cbfe6c0-f75c-4814-a662-329209eca45f"),
                Name = "User"
            },
            new Role
            {
                Id = Guid.Parse("169059be-06c6-49ed-9ffe-e5c6fba7a6cb"),
                Name = "Administrator"
            }
        );
    }
}