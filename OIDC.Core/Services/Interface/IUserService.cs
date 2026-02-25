using System.Security.Claims;
using OIDC.Core.DAL.Entities;
using OIDC.Core.DAL.ViewModels.Controllers.UserController;

namespace OIDC.Core.Services.Interface;

public interface IUserService
{
    public Task<User> CreateAsync(CreateAsyncViewModel vm);

    public Task<User?> FindByEmailAsync(string email);
    
    public Task<User?> FindByIdAsync(Guid userId);

    public bool ValidateCredentials(User user, string password);

    public Task<User> GetFromContextAsync(ClaimsPrincipal principal);

    public Task<IList<User>> GetListAsync();
}