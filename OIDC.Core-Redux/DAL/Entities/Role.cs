using Microsoft.EntityFrameworkCore;
using OIDC.Core_Redux.DAL.Configuration;

namespace OIDC.Core_Redux.DAL.Entities;

[EntityTypeConfiguration(typeof(RoleConfiguration))]
public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public ICollection<User> Users { get; set; }
}