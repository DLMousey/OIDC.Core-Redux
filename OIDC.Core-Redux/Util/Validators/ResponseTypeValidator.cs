namespace OIDC.Core_Redux.Util.Validators;

public static class ResponseTypeValidator
{
    private static readonly IList<string> ResponseTypes = new List<string>
    {
        "code"
    };

    public static bool Validate(string? value) => ResponseTypes.Contains(value);
}