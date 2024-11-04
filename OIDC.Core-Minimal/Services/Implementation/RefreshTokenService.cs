using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Services.Implementation;

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
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(
            rt => rt.Code.Equals(code) &&
                  rt.ExpiresAt >= DateTime.UtcNow
        );
    }

    public async Task<RefreshToken> RecordUse(RefreshToken refreshToken)
    {
        refreshToken.Uses += 1;
        
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync();

        return refreshToken;
    }
}