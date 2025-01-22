using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OIDC.Core_Minimal_Test.TestUtil;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.Services.Implementation;

namespace OIDC.Core_Minimal_Test.Services.Implementation;

[TestFixture]
public class UserServiceTest
{
    private UserService _userService;
    private User _testUser;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OIDCCoreMinimalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var context = new OIDCCoreMinimalDbContext(options);
        _testUser = UserGenerator.GenerateFixed();
        _userService = new UserService(context, new MailService(configuration));

        context.Users.Add(_testUser);
        context.SaveChanges();
    }

    [Test]
    public void CanRetrieveUserByEmail()
    {
        User? user = _userService.FindByEmailAsync(_testUser.Email).Result;

        Assert.That(user, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(user.Id, Is.EqualTo(_testUser.Id));
            Assert.That(user.Email, Is.EqualTo(_testUser.Email));
            Assert.That(user.Username, Is.EqualTo(_testUser.Username));
        });
    }
}