using OIDC.Core.DAL.Entities;

namespace OIDC.Core.Services.Interface;

public interface IScopeService
{
    public Task<IList<Scope>> GetScopesAsync(IList<string> scopeNames);

    public Task<IList<Scope>> FindAllAsync();

    public Task<Scope?> FindAsync(string name);

    public Task<Scope> CreateAsync(string name);

    public void Delete(string name);
}