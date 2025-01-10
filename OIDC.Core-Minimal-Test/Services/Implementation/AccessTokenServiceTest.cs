using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.Services.Implementation;

namespace OIDC.Core_Minimal_Test.Services.Implementation;

[TestFixture]
public class AccessTokenServiceTest
{
    private OIDCCoreMinimalDbContext _context;
    private ScopeService _scopeService;
    private AccessTokenService _accessTokenService;
    private Application _testApplication;
    private User _testUser;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OIDCCoreMinimalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new OIDCCoreMinimalDbContext(options);
        _testUser = new User
        {
            Email = "info@example.com",
            Username = "Test User",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
        };
        _testApplication = new Application
        {
            Name = "Test Application",
            HomepageUrl = "https://example.com",
            CallbackUrl = "https://example.com/oauth",
            CancelUrl = "https://example.com/oauth-cancel",
            User = _testUser,
            UserId = _testUser.Id
        };
        Scope read = new Scope("profile.read");
        Scope write = new Scope("profile.write");
        
        context.Scopes.Add(read);
        context.Scopes.Add(write);
        context.Users.Add(_testUser);
        context.AccessTokens.Add(new AccessToken
        {
            User = _testUser,
            UserId = _testUser.Id,
            Application = _testApplication,
            ApplicationId = _testApplication.Id,
            Scopes = new List<Scope> { read, write }
        });
        context.SaveChanges();

        _context = context;
        _scopeService = new ScopeService(context);
        _accessTokenService = new AccessTokenService(_context, _scopeService);
    }

    [Test]
    public void RetrieveAccessTokenByUserApplicationAndScopes()
    {
        IList<string> scopes = new List<string> { "profile.read", "profile.write" };
        AccessToken? accessToken = _accessTokenService.FindAsync(_testUser, _testApplication, scopes).Result;
        
        Assert.IsNotNull(accessToken);
        Assert.That(accessToken.ApplicationId, Is.EqualTo(_testApplication.Id));
        Assert.That(accessToken.UserId, Is.EqualTo(_testUser.Id));
    }

    [Test]
    public void CreatesNewAccessToken()
    {
        IList<Scope> scopes = _scopeService.FindAllAsync().Result;
        AccessToken accessToken = _accessTokenService.CreateAsync(_testUser, _testApplication, scopes).Result;
        
        Assert.That(_context.AccessTokens.Count(), Is.EqualTo(2));
        Assert.That(accessToken.ApplicationId, Is.EqualTo(_testApplication.Id));
        Assert.That(accessToken.UserId, Is.EqualTo(_testUser.Id));
        Assert.That(accessToken.Scopes, Is.EquivalentTo(scopes));
    }
}