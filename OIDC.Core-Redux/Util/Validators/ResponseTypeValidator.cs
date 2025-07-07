namespace OIDC.Core_Minimal.Util.Attributes;

public static class ResponseTypeValidator
{
    private static readonly IList<string> ResponseTypes = new List<string>
    {
        "code"
    };

    public static bool Validate(string? value) => ResponseTypes.Contains(value);
}