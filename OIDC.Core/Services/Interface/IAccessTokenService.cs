using OIDC.Core.DAL.Entities;

namespace OIDC.Core.Services.Interface;

public interface IAccessTokenService
{
    public Task<AccessToken?> FindAsync(User user, Application application, IList<string> scopes);

    public Task<AccessToken> CreateAsync(User user, Application application, IList<Scope> scopes);
}