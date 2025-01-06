using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL.Configuration;

namespace OIDC.Core_Minimal.DAL.Entities;

[EntityTypeConfiguration(typeof(RoleConfiguration))]
public class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public ICollection<User> Users { get; set; }
}