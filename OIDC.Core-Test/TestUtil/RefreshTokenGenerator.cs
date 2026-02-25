using OIDC.Core.DAL.Entities;

namespace OIDC.Core_Test.TestUtil;

public class RefreshTokenGenerator
{
    public static Guid FixedGuid() => Guid.Parse("58985300-b353-4346-9a6e-2d2f0c81b9dd");

    public static string FixedCode() =>
        "d+aGwHsPI9RpWaVoP4UFfsT3EjytsMBKUbrbj1KC9GsswzNe03GQVt3zNWci9ldY3SldtG7DRdQ+l7BHHmtKvA==";

    public static RefreshToken GenerateFixed()
    {
        User user = UserGenerator.GenerateFixed();
        
        return new RefreshToken
        {
            Id = FixedGuid(),
            UserId = user.Id,
            Code = FixedCode(),
            Uses = 0
        };
    }
}