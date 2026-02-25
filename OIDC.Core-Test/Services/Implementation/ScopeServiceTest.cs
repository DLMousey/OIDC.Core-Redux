using Microsoft.EntityFrameworkCore;
using OIDC.Core.DAL;
using OIDC.Core.DAL.Entities;
using OIDC.Core.Services.Implementation;

namespace OIDC.Core_Test.Services.Implementation;

[TestFixture]
public class ScopeServiceTest
{
    private OIDCCoreMinimalDbContext _context;
    private ScopeService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OIDCCoreMinimalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new OIDCCoreMinimalDbContext(options);
        context.Scopes.Add(new Scope("profile.read"));
        context.Scopes.Add(new Scope("profile.write"));
        context.SaveChanges();
        
        _context = context;
        _service = new ScopeService(context);
    }
    
    [Test]
    public void RetrievesScopesListViaContext()
    {
        IList<Scope> scopes = _service.FindAllAsync().Result;
        
        Assert.That(scopes.Count, Is.EqualTo(2));
        Assert.That(scopes[0].Name, Is.EqualTo("profile.read"));
        Assert.That(scopes[1].Name, Is.EqualTo("profile.write"));
    }

    [Test, Sequential]
    public void RetrieveScopeByNameViaContext([Values("profile.read", "profile.write")] string name)
    {
        Scope? scope = _service.FindAsync(name).Result;

        Assert.NotNull(scope);
        Assert.That(scope.Name, Is.EqualTo(name));
    }

    [Test]
    public void RetrievesScopeListViaContextFromNames()
    {
        string[] names = { "profile.read", "profile.write" };
        IList<Scope> scopes = _service.GetScopesAsync(names).Result;
        
        Assert.That(scopes.Count, Is.EqualTo(2));
        Assert.That(scopes[0].Name, Is.EqualTo("profile.read"));
        Assert.That(scopes[1].Name, Is.EqualTo("profile.write"));
    }

    [Test]
    public void CreatesNewScope()
    {
        Scope scope = _service.CreateAsync("test.scope").Result;
        IList<Scope> scopes = _service.FindAllAsync().Result;
        
        Assert.That(scopes.Count, Is.EqualTo(3));
        Assert.That(scope.Name, Is.EqualTo("test.scope"));
    }

    [Test, Sequential]
    public void DoesNotCreateDuplicateScopes([Values("profile.read", "profile.write")] string name)
    {
        Scope scope = _service.CreateAsync(name).Result;
        Scope duplicate = _service.CreateAsync(name).Result;
        IList<Scope> scopes = _service.FindAllAsync().Result;
        
        Assert.That(scopes.Count, Is.EqualTo(2));
        Assert.That(duplicate.Name, Is.EqualTo(scope.Name));
    }
    
    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}