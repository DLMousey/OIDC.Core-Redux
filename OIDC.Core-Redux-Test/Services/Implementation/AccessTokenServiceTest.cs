using Microsoft.EntityFrameworkCore;
using OIDC.Core_Redux_Test.TestUtil;
using OIDC.Core_Redux.DAL;
using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.Services.Implementation;

namespace OIDC.Core_Redux_Test.Services.Implementation;

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
        _testUser = UserGenerator.GenerateFixed();
        _testApplication = ApplicationGenerator.GenerateFixed(_testUser);

        IList<Scope> scopes = ScopeGenerator.GenerateFixed();
        foreach (Scope scope in scopes)
        {
            context.Scopes.Add(scope);
        }
        
        context.Users.Add(_testUser);
        context.AccessTokens.Add(AccessTokenGenerator.GenerateFixed(scopes));
        
        context.SaveChanges();

        _context = context;
        _scopeService = new ScopeService(context);
        _accessTokenService = new AccessTokenService(_context, _scopeService);
    }

    [Test]
    public void RetrieveAccessTokenByUserApplicationAndScopes()
    {
        AccessToken? accessToken = _accessTokenService.FindAsync(
            _testUser, 
            _testApplication, 
            ScopeGenerator.FixedNames()
        ).Result;
        
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

    [TearDown]
    public void Teardown()
    {
        _context.Dispose();
    }
}