using System.Text.Json.Serialization;

namespace OIDC.Core.DAL.ViewModels.Controllers.WellKnownController;

public class JsonWebKeyViewModel
{
    [JsonPropertyName("kty")] 
    public required string KeyType { get; set; }

    [JsonPropertyName("kid")] 
    public required string KeyId { get; set; }

    [JsonPropertyName("use")]
    public required string IntendedUse { get; set; }
    
    [JsonPropertyName("alg")]
    public required string Algorithm { get; set; }
    
    [JsonPropertyName("n")]
    public required string KeyMaterial { get; set; }
}