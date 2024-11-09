﻿using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Services.Implementation;

public class ScopeService(OIDCCoreMinimalDbContext context) : IScopeService
{
    // @TODO - consider warming cache with scope values during app startup
    public async Task<IList<Scope>> GetScopesAsync(IList<string> scopeNames)
    {
        List<Scope> scopes = new();
        foreach (string scopeName in scopeNames)
        {
            Scope? scope = await context.Scopes.FirstOrDefaultAsync(s => s.Name.Equals(scopeName));
            if (scope == null)
            {
                continue;
            }
            
            scopes.Add(scope);
        }

        return scopes;
    }
}