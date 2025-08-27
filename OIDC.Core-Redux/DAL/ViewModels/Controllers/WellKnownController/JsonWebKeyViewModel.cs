using System.Text.Json.Serialization;

namespace OIDC.Core_Redux.DAL.ViewModels.Controllers.WellKnownController;

public class JsonWebKeyViewModel
{
    [JsonPropertyName("key")]
    public required string Key { get; set; }

    [JsonPropertyName("alg")]
    public required string Algorithm { get; set; }

    [JsonPropertyName("len")]
    public int Length { get; set; }
    
}