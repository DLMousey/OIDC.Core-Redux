using System.Security.Claims;
using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.DAL.ViewModels.Controllers.UserController;

namespace OIDC.Core_Redux.Services.Interface;

public interface IUserService
{
    public Task<User> CreateAsync(CreateAsyncViewModel vm);

    public Task<User?> FindByEmailAsync(string email);
    
    public Task<User?> FindByIdAsync(Guid userId);

    public bool ValidateCredentials(User user, string password);

    public Task<User> GetFromContextAsync(ClaimsPrincipal principal);

    public Task<IList<User>> GetListAsync();
}