using System.Net;

namespace OIDC.Core.DAL.Entities;

public class AuthenticationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool Success { get; set; } = false;

    public IPAddress? IpAddress { get; set; }

    public Guid? UserId { get; set; }

    public User? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}