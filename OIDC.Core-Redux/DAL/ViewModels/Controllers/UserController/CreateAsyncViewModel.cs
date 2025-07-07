using System.ComponentModel.DataAnnotations;

namespace OIDC.Core_Minimal.DAL.ViewModels.Controllers.UserController;

public class CreateAsyncViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    [Required]
    public string Username { get; set; }
}