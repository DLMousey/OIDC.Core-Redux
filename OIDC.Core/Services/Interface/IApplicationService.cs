using OIDC.Core.DAL.Entities;
using OIDC.Core.DAL.ViewModels.Controllers.ApplicationController;

namespace OIDC.Core.Services.Interface;

public interface IApplicationService
{
    public Task<Application> CreateAsync(CreateAsyncViewModel vm, User user);

    public Task<IList<Application>> GetPublishedAsync(User user);

    public Task<IList<Application>> GetAuthorisedAsync(User user);

    public Task<Application?> FindAsync(string clientId);

    public Task<Application?> AddUser(Application application, User user);

    public Task<Application?> RemoveUser(Application application, User user);
}