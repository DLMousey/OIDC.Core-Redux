using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux.Services.Interface;

public interface IJwtService
{
    public string GenerateJwt(User user);

    public string GenerateJwt(AccessToken accessToken);
}