namespace OIDC.Core.DAL.ViewModels.Configuration;

public class JwksKeysConfiguration
{
    public string KeyMaterial { get; set; }

    public string Algorithm { get; set; }

    public string IntendedUse { get; set; }

    public Guid KeyId { get; set; }

    public string KeyType { get; set; }
}