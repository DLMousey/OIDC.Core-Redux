using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Services.Implementation;

public class AccessTokenService(
    OIDCCoreMinimalDbContext context,
    IScopeService scopeService
) : IAccessTokenService
{
    public async Task<AccessToken?> FindAsync(User user, Application application, IList<string> scopes)
    {
        AccessToken? accessToken = await context.AccessTokens
            .Include(at => at.Scopes)
            .FirstOrDefaultAsync(at =>
                at.UserId.Equals(user.Id) &&
                at.ApplicationId.Equals(application.Id)
        );

        if (accessToken == null)
        {
            return null;
        }

        IList<Scope> requestedScopes = await scopeService.GetScopesAsync(scopes);
        if (requestedScopes.SequenceEqual(accessToken.Scopes))
        {
            return accessToken;
        }

        return null;
    }

    public async Task<AccessToken> CreateAsync(User user, Application application, IList<Scope> scopes)
    {
        AccessToken token = new AccessToken
        {
            User = user,
            UserId = user.Id,
            Application = application,
            ApplicationId = application.Id,
            Scopes = scopes
        };

        await context.AccessTokens.AddAsync(token);
        await context.SaveChangesAsync();

        return token;
    }
}