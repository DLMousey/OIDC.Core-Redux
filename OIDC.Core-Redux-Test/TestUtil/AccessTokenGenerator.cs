using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux_Test.TestUtil;

public static class AccessTokenGenerator
{
    public static Guid FixedGuid() => Guid.Parse("e0635229-541a-446b-8f0b-894451d7dcff");

    public static AccessToken GenerateFixed(IList<Scope>? scopes = null)
    {
        if (scopes == null)
        {
            scopes = ScopeGenerator.GenerateFixed();
        }
        
        User user = UserGenerator.GenerateFixed();
        Application application = ApplicationGenerator.GenerateFixed(user);

        return new AccessToken
        {
            UserId = user.Id,
            ApplicationId = application.Id,
            Scopes = scopes
        };
    }
}