using OIDC.Core.DAL.Entities;

namespace OIDC.Core_Test.DAL.Entities;

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