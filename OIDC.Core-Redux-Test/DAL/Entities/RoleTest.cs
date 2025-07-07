using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux_Test.DAL.Entities;

[TestFixture]
public class RoleTest
{
    [Test]
    public void NewInstanceHasGuidSet()
    {
        Role role = new Role();
        Assert.NotNull(role.Id);
        Assert.That(role.Id, Is.Not.EqualTo(Guid.Empty));
    }
}