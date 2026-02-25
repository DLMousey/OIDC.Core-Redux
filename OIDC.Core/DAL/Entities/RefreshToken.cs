using System.Security.Cryptography;

namespace OIDC.Core.DAL.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public User User { get; set; }

    public Guid UserId { get; set; }

    // Turns out either dotnet or EF core is stripping out the == b64 padding causing queries to fail here.
    // Swapped out for a hex string in the meantime - ideally should urlencode b64 which doesn't add padding
    public string Code { get; set; } = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

    public int Uses { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(1);
}