using System.ComponentModel.DataAnnotations;
using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.DAL.ViewModels.Controllers.ApplicationController;

public class CreateAsyncViewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; }

    [Required]
    [Url]
    public string HomepageUrl { get; set; }

    public string? Description { get; set; }

    [Required]
    [Url]
    public string CallbackUrl { get; set; }
}