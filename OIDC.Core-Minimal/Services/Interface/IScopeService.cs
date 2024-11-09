using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.Services.Interface;

public interface IScopeService
{
    public Task<IList<Scope>> GetScopesAsync(IList<string> scopeNames);
}