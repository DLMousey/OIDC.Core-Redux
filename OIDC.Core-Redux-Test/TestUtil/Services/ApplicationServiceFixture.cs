using Microsoft.EntityFrameworkCore;
using OIDC.Core_Redux.DAL;
using OIDC.Core_Redux.Services.Implementation;
using OIDC.Core_Redux.Services.Interface;

namespace OIDC.Core_Redux_Test.TestUtil.Services;

public static class ApplicationServiceFixture
{
    public static IApplicationService CreateApplicationService(Guid? databaseId)
    {
        if (databaseId == null)
        {
            databaseId = Guid.NewGuid();
        }
        
        var options = new DbContextOptionsBuilder<OIDCCoreMinimalDbContext>()
            .UseInMemoryDatabase(databaseId.ToString()!)
            .Options;

        var context = new OIDCCoreMinimalDbContext(options);
        return new ApplicationService(context);
    }
}