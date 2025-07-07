using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.Services.Interface;

public interface IRefreshTokenService
{
    public Task<RefreshToken?> FindCurrentAsync(User user);

    public Task<RefreshToken> CreateAsync(User user);

    public Task<RefreshToken?> FindAsync(string code);
    
    public Task<RefreshToken> RecordUse(RefreshToken refreshToken);
}