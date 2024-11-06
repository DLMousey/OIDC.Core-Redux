using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.ApplicationController;

namespace OIDC.Core_Minimal.Services.Interface;

public interface IApplicationService
{
    public Task<Application> CreateAsync(CreateAsyncViewModel vm, User user);

    public Task<IList<Application>> GetPublishedAsync(User user);
}