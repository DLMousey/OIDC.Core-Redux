using System.ComponentModel.DataAnnotations;

namespace OIDC.Core_Redux.DAL.ViewModels.Controllers.AuthenticationController;

public class AuthenticateAsyncViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}