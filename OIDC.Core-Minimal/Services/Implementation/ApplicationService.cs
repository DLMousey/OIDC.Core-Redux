using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.ApplicationController;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Services.Implementation;

public class ApplicationService(OIDCCoreMinimalDbContext context) : IApplicationService
{
    public async Task<Application> CreateAsync(CreateAsyncViewModel vm, User user)
    {
        Application application = new Application
        {
            Name = vm.Name,
            HomepageUrl = vm.HomepageUrl,
            CallbackUrl = vm.CallbackUrl,
            User = user,
            UserId = user.Id
        };

        await context.Applications.AddAsync(application);
        await context.SaveChangesAsync();

        return application;
    }

    public async Task<IList<Application>> GetPublishedAsync(User user) => 
        await context.Applications.Where(a => a.UserId.Equals(user.Id)).ToListAsync();

    public async Task<Application?> FindAsync(string clientId) =>
        await context.Applications.FirstOrDefaultAsync(a => a.ClientId.Equals(clientId));
}