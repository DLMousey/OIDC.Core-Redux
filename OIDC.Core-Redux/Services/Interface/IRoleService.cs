using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux.Services.Interface;

public interface IRoleService
{
    public Task<IList<Role>> ListRolesAsync();
    
    public Task<Role?> FindAsync(Guid roleId);

    public Task<User> AttachRoleAsync(User user, Role role);

    public Task<User> DetachRoleAsync(User user, Role role);
}