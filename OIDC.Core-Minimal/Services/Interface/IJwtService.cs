using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.Services.Interface;

public interface IJwtService
{
    public string GenerateJwt(User user);
}