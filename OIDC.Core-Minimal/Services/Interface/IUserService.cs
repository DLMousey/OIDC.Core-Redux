using System.Security.Claims;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.UserController;

namespace OIDC.Core_Minimal.Services.Interface;

public interface IUserService
{
    public Task<User> CreateAsync(CreateAsyncViewModel vm);

    public Task<User?> FindByEmailAsync(string email);
    
    public Task<User?> FindByIdAsync(Guid userId);

    public bool ValidateCredentials(User user, string password);

    public Task<User> GetFromContextAsync(ClaimsPrincipal principal);
}