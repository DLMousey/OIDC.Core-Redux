using OIDC.Core.DAL.Entities;

namespace OIDC.Core.Services.Interface;

public interface IJwtService
{
    public string GenerateJwt(User user);

    public string GenerateJwt(AccessToken accessToken);
}