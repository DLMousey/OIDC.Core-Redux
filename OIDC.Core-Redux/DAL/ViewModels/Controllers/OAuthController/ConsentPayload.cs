namespace OIDC.Core_Redux.DAL.ViewModels.Controllers.OAuthController;

public class ConsentPayload
{
    public Guid UserId { get; set; }

    public string ClientId { get; set; }

    public IList<string> Scopes { get; set; }

    public string? CodeChallenge { get; set; }

    public string? CodeVerifier { get; set; }
}