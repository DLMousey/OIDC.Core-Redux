using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux_Test.DAL.Entities;

[TestFixture]
public class UserTest
{
    [Test]
    public void NewInstanceHasGuidPopulated()
    {
        User user = new User();
        Assert.NotNull(user.Id);
        Assert.That(user.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void NewInstanceHasDateCreatedPopulated()
    {
        User user = new User();
        Assert.NotNull(user.CreatedAt);
        Assert.That(user.CreatedAt, Is.Not.EqualTo(default(DateTime)));
    }
}