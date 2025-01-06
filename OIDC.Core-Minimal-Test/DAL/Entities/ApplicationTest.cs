using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal_Test.DAL.Entities;

[TestFixture]
public class ApplicationTest
{
    [Test]
    public void NewInstanceHasGuidPopulated()
    {
        Application application = new Application();
        Assert.NotNull(application.Id);
        Assert.That(application.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void NewInstanceHasClientIdPopulated()
    {
        Application application = new Application();
        Assert.NotNull(application.ClientId);
        Assert.That(application.ClientId, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void NewInstanceHasClientSecretPopulated()
    {
        Application application = new Application();
        Assert.NotNull(application.ClientSecret);
        Assert.That(application.ClientSecret, Is.Not.EqualTo(string.Empty));
    }
}