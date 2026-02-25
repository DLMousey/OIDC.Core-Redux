using OIDC.Core.DAL.Entities;

namespace OIDC.Core.Services.Interface;

public interface IRefreshTokenService
{
    public Task<RefreshToken?> FindCurrentAsync(User user);

    public Task<RefreshToken> CreateAsync(User user);

    public Task<RefreshToken?> FindAsync(string code);
    
    public Task<RefreshToken> RecordUse(RefreshToken refreshToken);
}