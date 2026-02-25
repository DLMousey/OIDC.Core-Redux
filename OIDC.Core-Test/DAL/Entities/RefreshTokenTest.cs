using OIDC.Core.DAL.Entities;

namespace OIDC.Core_Test.DAL.Entities;

[TestFixture]
public class RefreshTokenTest
{
    [Test]
    public void NewInstanceHasGuidPopulated()
    {
        RefreshToken refreshToken = new RefreshToken();
        Assert.NotNull(refreshToken.Id);
        Assert.That(refreshToken.Id, Is.Not.EqualTo(Guid.Empty));
        
    }

    [Test]
    public void NewInstanceHasCodePopulated()
    {
        RefreshToken refreshToken = new RefreshToken();
        Assert.NotNull(refreshToken.Code);
        Assert.That(refreshToken.Code, Is.Not.EqualTo(string.Empty));
    }
}