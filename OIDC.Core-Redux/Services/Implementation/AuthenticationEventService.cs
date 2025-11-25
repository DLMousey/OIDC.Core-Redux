using System.Net;
using Microsoft.EntityFrameworkCore;
using OIDC.Core_Redux.DAL;
using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.Services.Interface;

namespace OIDC.Core_Redux.Services.Implementation;

public class AuthenticationEventService(OIDCCoreMinimalDbContext context) : IAuthenticationEventService
{
    public async Task<IList<AuthenticationEvent>> GetUserEvents(User user) =>
        await context.AuthenticationEvents.Where(a => a.UserId.Equals(user.Id)).ToListAsync();

    public async Task<IList<AuthenticationEvent>> GetUserEvents(Guid userId) =>
        await context.AuthenticationEvents.Where(a => a.UserId.Equals(userId)).ToListAsync();

    public async Task<AuthenticationEvent> RecordAuthenticationEvent(bool success, User? user, IPAddress? ipAddress)
    {
        AuthenticationEvent authEvent = new AuthenticationEvent
        {
            UserId = user?.Id,
            Success = success,
            IpAddress = ipAddress,
        };

        await context.AuthenticationEvents.AddAsync(authEvent);
        await context.SaveChangesAsync();

        return authEvent;
    }
}