using Microsoft.EntityFrameworkCore;
using OIDC.Core_Redux.DAL;
using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.Services.Interface;

namespace OIDC.Core_Redux.Services.Implementation;

public class RefreshTokenService(OIDCCoreMinimalDbContext context) : IRefreshTokenService
{
    public async Task<RefreshToken?> FindCurrentAsync(User user)
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(
            rt => rt.UserId.Equals(user.Id) &&
                  rt.ExpiresAt >= DateTime.UtcNow);
    }

    public async Task<RefreshToken> CreateAsync(User user)
    {
        RefreshToken token = new RefreshToken
        {
            User = user,
            UserId = user.Id
        };

        await context.RefreshTokens.AddAsync(token);
        await context.SaveChangesAsync();

        return token;
    }

    public async Task<RefreshToken?> FindAsync(string code)
    {
        var result = await context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(
                rt => rt.Code.Equals(code) &&
                      rt.ExpiresAt >= DateTime.UtcNow
            );

        return result;
    }

    public async Task<RefreshToken> RecordUse(RefreshToken refreshToken)
    {
        refreshToken.Uses += 1;
        
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync();

        return refreshToken;
    }
}