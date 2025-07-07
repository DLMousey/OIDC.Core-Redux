using Microsoft.EntityFrameworkCore;
using OIDC.Core_Redux.DAL;
using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.Services.Interface;

namespace OIDC.Core_Redux.Services.Implementation;

public class RoleService(OIDCCoreMinimalDbContext context) : IRoleService
{
    public async Task<IList<Role>> ListRolesAsync()
    {
        return await context.Roles.ToListAsync();
    }

    public async Task<Role?> FindAsync(Guid roleId)
    {
        return await context.Roles.FindAsync(roleId);
    }

    public async Task<User> AttachRoleAsync(User user, Role role)
    {
        if (!user.Roles.Contains(role))
        {
            user.Roles.Add(role);
        
            context.Update(user);
            await context.SaveChangesAsync();

            return user;
        }

        return user;
    }

    public async Task<User> DetachRoleAsync(User user, Role role)
    {
        if (user.Roles.Contains(role))
        {
            user.Roles.Remove(role);
            
            context.Update(user);
            await context.SaveChangesAsync();
            
            return user;
        }

        return user;
    }
}