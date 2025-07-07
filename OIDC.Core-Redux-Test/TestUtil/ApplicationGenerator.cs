using Bogus;
using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux_Test.TestUtil;

public static class ApplicationGenerator
{
    public static Guid FixedGuid() => Guid.Parse("bf8c51f9-9772-4c25-89c4-5c5a9fd60828");

    public static Application GenerateFixed(User? user)
    {
        if (user == null)
        {
            user = UserGenerator.GenerateFixed();
        }
        
        return new Application
        {
            Id = FixedGuid(),
            Name = "Test Application",
            HomepageUrl = "https://example.com",
            CallbackUrl = "https://example.com/oauth",
            CancelUrl = "https://example.com/oauth?cancelled=true",
            User = user,
            UserId = user.Id
        };
    }

    public static Application GenerateRandom()
    {
        Faker faker = new Faker();
        string url = faker.Internet.Url();
        User user = UserGenerator.GenerateRandom();

        return new Application
        {
            Name = faker.Internet.DomainName(),
            HomepageUrl = url,
            CallbackUrl = url + "/oauth",
            CancelUrl = url + "/oauth?cancelled=true",
            User = user,
            UserId = user.Id,
            Created = faker.Date.Past()
        };
    }
}