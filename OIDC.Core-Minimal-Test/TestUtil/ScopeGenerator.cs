using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal_Test.TestUtil;

public static class ScopeGenerator
{
    public static IList<string> FixedNames() => new List<string> { "profile.read", "profile.write" };

    public static IList<Scope> GenerateFixed()
    {
        return new List<Scope>
        {
            new Scope("profile.read"),
            new Scope("profile.write")
        };
    }
}