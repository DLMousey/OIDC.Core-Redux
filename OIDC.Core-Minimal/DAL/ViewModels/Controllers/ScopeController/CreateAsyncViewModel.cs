using System.Text.Json.Serialization;

namespace OIDC.Core_Minimal.DAL.ViewModels.Controllers.ScopeController;

public class CreateAsyncViewModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}