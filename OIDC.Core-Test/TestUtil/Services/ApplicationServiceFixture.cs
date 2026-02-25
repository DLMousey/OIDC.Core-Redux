using Microsoft.EntityFrameworkCore;
using OIDC.Core.DAL;
using OIDC.Core.Services.Implementation;
using OIDC.Core.Services.Interface;

namespace OIDC.Core_Test.TestUtil.Services;

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