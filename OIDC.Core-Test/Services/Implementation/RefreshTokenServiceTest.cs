using Microsoft.EntityFrameworkCore;
using OIDC.Core_Test.TestUtil;
using OIDC.Core.DAL;
using OIDC.Core.DAL.Entities;
using OIDC.Core.Services.Implementation;

namespace OIDC.Core_Test.Services.Implementation;

[TestFixture]
public class RefreshTokenServiceTest
{
    private User _testUser;
    private RefreshToken _testRefreshToken;
    private RefreshTokenService _refreshTokenService;
    private OIDCCoreMinimalDbContext _context;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OIDCCoreMinimalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new OIDCCoreMinimalDbContext(options);
        _testUser = UserGenerator.GenerateFixed();
        _testRefreshToken = RefreshTokenGenerator.GenerateFixed();

        _context.Users.Add(_testUser);
        _context.RefreshTokens.Add(_testRefreshToken);
        _context.SaveChanges();
        
        _refreshTokenService = new RefreshTokenService(_context);
    }

    [Test]
    public void CanRetrieveRefreshTokenByCode()
    {
        RefreshToken? refreshToken = _refreshTokenService.FindAsync(_testRefreshToken.Code).Result;
        
        Assert.That(refreshToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(refreshToken.Id, Is.EqualTo(_testRefreshToken.Id));
            Assert.That(refreshToken.Code, Is.EqualTo(_testRefreshToken.Code));
            Assert.That(refreshToken.User, Is.EqualTo(_testUser));
        });
    }

    [Test]
    public void CanRecordUsesOfToken()
    {
        RefreshToken token = _refreshTokenService.RecordUse(_testRefreshToken).Result;
        Assert.That(token.Uses, Is.EqualTo(1));
    }

    [Test]
    public void CanRecordMultipleUsesOfToken()
    {
        _testRefreshToken.Uses = 0;
        _context.Update(_testRefreshToken);
        _context.SaveChanges();
        
        _testRefreshToken = _refreshTokenService.RecordUse(_testRefreshToken).Result;
        Assert.That(_testRefreshToken.Uses, Is.EqualTo(1));
        
        _testRefreshToken = _refreshTokenService.RecordUse(_testRefreshToken).Result;
        Assert.That(_testRefreshToken.Uses, Is.EqualTo(2));
        
        _testRefreshToken = _refreshTokenService.RecordUse(_testRefreshToken).Result;
        Assert.That(_testRefreshToken.Uses, Is.EqualTo(3));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}