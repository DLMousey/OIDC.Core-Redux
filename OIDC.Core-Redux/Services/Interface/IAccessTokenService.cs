using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.Services.Interface;

public interface IAccessTokenService
{
    public Task<AccessToken?> FindAsync(User user, Application application, IList<string> scopes);

    public Task<AccessToken> CreateAsync(User user, Application application, IList<Scope> scopes);
}