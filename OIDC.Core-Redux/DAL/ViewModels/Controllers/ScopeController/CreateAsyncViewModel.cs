using System.Text.Json.Serialization;

namespace OIDC.Core_Redux.DAL.ViewModels.Controllers.ScopeController;

public class CreateAsyncViewModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}