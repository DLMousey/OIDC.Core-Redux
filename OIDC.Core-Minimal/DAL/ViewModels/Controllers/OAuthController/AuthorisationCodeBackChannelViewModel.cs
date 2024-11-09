using System.ComponentModel.DataAnnotations;

namespace OIDC.Core_Minimal.DAL.ViewModels.Controllers.OAuthController;

public class AuthorisationCodeBackChannelViewModel
{
    [RegularExpression("\bauthorization_code\b")]
    public string GrantType { get; set; }

    [Required]
    public string Code { get; set; }

    [Required]
    public string RedirectUri { get; set; }

    [Required]
    public string ClientId { get; set; }

    [Required]
    public string ClientSecret { get; set; }
}