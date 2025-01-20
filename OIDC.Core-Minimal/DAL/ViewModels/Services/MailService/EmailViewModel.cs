using System.ComponentModel.DataAnnotations;
using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.DAL.ViewModels.Services.MailService;

public class EmailViewModel
{
    [Required]
    public required string Slug { get; set; }

    [Required]
    public required string Subject { get; set; }

    [Required]
    public required string ToName { get; set; }

    [Required]
    [EmailAddress]
    public required string ToAddress { get; set; }

    public string? FromName { get; set; }

    public string? FromAddress { get; set; }

    public User? User { get; set; }

    public Dictionary<string, string>? Data { get; set; }
}