using System.ComponentModel.DataAnnotations;

namespace OIDC.Core_Redux.DAL.ViewModels.Controllers.AuthenticationController;

public class RefreshAsyncViewModel
{
    [Required]
    public string RefreshToken { get; set; }
}