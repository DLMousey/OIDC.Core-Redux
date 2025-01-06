using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal_Test.DAL.Entities;

[TestFixture]
public class AccessTokenTest
{
    [Test]
    public void NewInstanceHasGuidPopulated()
    {
        AccessToken accessToken = new AccessToken();
        Assert.NotNull(accessToken.Id);
        Assert.That(accessToken.Id, Is.Not.EqualTo(Guid.Empty));
        
    }

    [Test]
    public void NewInstanceHasCodePopulated()
    {
        AccessToken accessToken = new AccessToken();
        Assert.NotNull(accessToken.Code);
        Assert.That(accessToken.Code, Is.Not.EqualTo(string.Empty));
    }
}