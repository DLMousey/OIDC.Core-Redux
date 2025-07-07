namespace OIDC.Core_Redux.DAL.ViewModels.Controllers.OAuthController;

public class AuthorisationRequestVm
{
    public string? ResponseType { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? RedirectUri { get; set; }
    public string? Scope { get; set; }
    public string? State { get; set; }
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
    public string? GrantType { get; set; }
    public string? DeviceCode { get; set; }
    public string? RefreshToken { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}