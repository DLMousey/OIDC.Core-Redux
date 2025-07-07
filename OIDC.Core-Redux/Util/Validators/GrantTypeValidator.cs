namespace OIDC.Core_Minimal.Util.Attributes;

public static class GrantTypeValidator
{
    private static readonly IList<string> GrantTypes = new List<string>
    {
        "authorization_code",
        "client_credentials",
        "urn:ietf:params:oauth:grant-type:device_code",
        "refresh_token",
        "password"
    };

    public static bool Validate(string? value) => GrantTypes.Contains(value);
}