using OIDC.Core.DAL.Entities;

namespace OIDC.Core_Test.DAL.Entities;

[TestFixture]
public class AuthenticationEventTest
{
    [Test]
    public void NewInstanceHasGuidPopulated()
    {
        AuthenticationEvent authEvent = new AuthenticationEvent();
        Assert.NotNull(authEvent.Id);
        Assert.That(authEvent.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void NewInstanceHasFalsySuccessValue()
    {
        AuthenticationEvent authEvent = new AuthenticationEvent();
        Assert.NotNull(authEvent.Success);
        Assert.False(authEvent.Success);
    }

    [Test]
    public void NewInstanceHasCreatedAtPopulated()
    {
        AuthenticationEvent authEvent = new AuthenticationEvent();
        Assert.NotNull(authEvent.CreatedAt);
        Assert.That(authEvent.CreatedAt, Is.Not.EqualTo(default(DateTime)));
    }
}