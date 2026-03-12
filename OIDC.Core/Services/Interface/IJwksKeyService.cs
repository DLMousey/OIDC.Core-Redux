using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace OIDC.Core.Services.Interface;

public interface IJwksKeyService
{
    public RsaSecurityKey GetSecurityKey();
    public RSAParameters GetPublicKeyParameters();
    public string GetKeyId();
}