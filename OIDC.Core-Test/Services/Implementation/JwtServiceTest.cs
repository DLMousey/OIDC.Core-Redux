using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core_Test.TestUtil;
using OIDC.Core.DAL.Entities;
using OIDC.Core.Services.Implementation;
using OIDC.Core.Services.Interface;

namespace OIDC.Core_Test.Services.Implementation;

[TestFixture]
public class JwtServiceTest
{
    private IJwtService _jwtService;
    private IJwtService _noConfigJwtService;

    [SetUp]
    public void Setup()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "JWT:SigningKey", Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(128)) }
        };
        
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        IConfiguration emptyConfiguration = new ConfigurationBuilder().Build();

        _jwtService = new JwtService(configuration);
        _noConfigJwtService = new JwtService(emptyConfiguration);
    }

    [Test]
    public void ThrowsExceptionIfSigningKeyIsMissing()
    {
        User user = UserGenerator.GenerateRandom();
        IList<Scope> scopes = ScopeGenerator.GenerateFixed();
        AccessToken token = AccessTokenGenerator.GenerateFixed(scopes);
        
        Assert.Throws<ApplicationException>(() => _noConfigJwtService.GenerateJwt(user));
        Assert.Throws<ApplicationException>(() => _noConfigJwtService.GenerateJwt(token));
    }

    [Test]
    public void CanGenerateJwtForUser()
    {
        User user = UserGenerator.GenerateRandom();
        user.Roles = new List<Role>
        {
            new Role
            {
                Name = "User"
            }
        };
        
        string jwt = _jwtService.GenerateJwt(user);

        Assert.That(jwt, Is.Not.Null);
        Assert.That(jwt, Is.Not.Empty);
    }
}