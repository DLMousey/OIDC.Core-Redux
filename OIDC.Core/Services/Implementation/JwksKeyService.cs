using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core.Services.Interface;

namespace OIDC.Core.Services.Implementation;

public class RsaKeyService : IJwksKeyService
{
    private readonly RSA _rsa;
    private readonly string _keyId;

    public RsaKeyService(string privateKeyPath, string keyId)
    {
        _rsa = RSA.Create();
        _rsa.ImportFromPem(File.ReadAllText(privateKeyPath));
        _keyId = keyId;
    }

    public RsaSecurityKey GetSecurityKey() => 
        new RsaSecurityKey(_rsa) { KeyId = _keyId };

    public RSAParameters GetPublicKeyParameters() => 
        _rsa.ExportParameters(false);

    public string GetKeyId() => _keyId;
}