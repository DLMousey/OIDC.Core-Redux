﻿using System.ComponentModel.DataAnnotations;

namespace OIDC.Core_Minimal.DAL.ViewModels.Controllers.OAuthController;

public class ConsentViewModel
{
    [Required]
    public string ClientId { get; set; }

    [Required]
    public bool Consent { get; set; } = false;

    [Required] 
    public IList<string> Scopes { get; set; }

    [Required]
    public string State { get; set; }
}