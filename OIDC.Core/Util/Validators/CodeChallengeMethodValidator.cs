namespace OIDC.Core.Util.Validators;

public static class CodeChallengeMethodValidator
{
    private static readonly IList<string> ChallengeMethods = new List<string>
    {
        "SHA256"
    };

    public static bool Validate(string? value) => ChallengeMethods.Contains(value);
}