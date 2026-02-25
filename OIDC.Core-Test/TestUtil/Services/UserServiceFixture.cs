using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OIDC.Core.DAL;
using OIDC.Core.Services.Implementation;
using OIDC.Core.Services.Interface;

namespace OIDC.Core_Test.TestUtil.Services;

public static class UserServiceFixture
{
    public static IUserService CreateUserService(Guid? databaseId)
    {
        if (databaseId == null)
        {
            databaseId = Guid.NewGuid();
        }
        
        var options = new DbContextOptionsBuilder<OIDCCoreMinimalDbContext>()
            .UseInMemoryDatabase(databaseId.ToString()!)
            .Options;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        
        var context = new OIDCCoreMinimalDbContext(options);
        var userService = new UserService(context, new MailService(configuration));

        return userService;
    }
}