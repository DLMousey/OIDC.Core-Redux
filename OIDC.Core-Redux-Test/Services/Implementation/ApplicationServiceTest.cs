using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OIDC.Core_Minimal_Test.TestUtil;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.Services.Implementation;

namespace OIDC.Core_Minimal_Test.Services.Implementation;

public class ApplicationServiceTest
{
    private OIDCCoreMinimalDbContext _context;
    private ApplicationService _applicationService;
    private UserService _userService;
    private User _testUser;
    private Application _testApplication;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OIDCCoreMinimalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new OIDCCoreMinimalDbContext(options);
        _testUser = UserGenerator.GenerateFixed();
        _testApplication = ApplicationGenerator.GenerateFixed(_testUser);
        
        context.Users.Add(_testUser);
        context.Applications.Add(_testApplication);
        context.SaveChanges();


        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var mailService = new MailService(configuration);
        _applicationService = new ApplicationService(context);
        _userService = new UserService(context, mailService);
        
        _context = context;
    }

    [Test]
    public void CanRetrievePublishedApplicationsForUser()
    {
        IList<Application> applications = _applicationService.GetPublishedAsync(_testUser).Result;
        
        Assert.That(applications, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(applications.First(), Is.EqualTo(_testApplication));
            Assert.That(applications.First().Id, Is.EqualTo(_testApplication.Id));
            Assert.That(applications.First().UserId, Is.EqualTo(_testUser.Id));
        });
    }

    [Test]
    public void CanRetrieveApplicationsByClientId()
    {
        Application? application = _applicationService.FindAsync(_testApplication.ClientId).Result;
        
        Assert.NotNull(application);
        Assert.Multiple(() =>
        {
            Assert.That(application, Is.EqualTo(_testApplication));
            Assert.That(application.Id, Is.EqualTo(_testApplication.Id));
            Assert.That(application.ClientId, Is.EqualTo(_testApplication.ClientId));
        });
    }

    
    [TearDown]
    public void Teardown()
    {
        _context.Dispose();
    }
}