using Bogus;
using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux_Test.TestUtil;

public static class UserGenerator
{
    public static Guid FixedGuid() => Guid.Parse("77085fb8-5dae-4d97-bda5-16bf21e88bf5");
    
    public static User GenerateFixed()
    {
        return new User
        {
            Id = FixedGuid(),
            Email = "info@example.com",
            Username = "TestUser",
            Password = BCrypt.Net.BCrypt.HashPassword("password")
        };
    }

    public static User GenerateRandom()
    {
        Faker faker = new Faker();
        return new User
        {
            Email = faker.Internet.Email(),
            Username = faker.Internet.UserName(),
            Password = BCrypt.Net.BCrypt.HashPassword(faker.Internet.Password()),
            CreatedAt = faker.Date.Past()
        };
    }
}