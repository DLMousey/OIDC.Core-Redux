using System.Security.Cryptography;

namespace OIDC.Core_Minimal.DAL.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public User User { get; set; }

    public Guid UserId { get; set; }

    public string Code { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public int Uses { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(1);
}