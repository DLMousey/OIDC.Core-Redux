using System.Net;
using OIDC.Core_Redux.DAL.Entities;

namespace OIDC.Core_Redux.Services.Interface;

public interface IAuthenticationEventService
{
    public Task<IList<AuthenticationEvent>> GetUserEvents(User user);
    
    public Task<IList<AuthenticationEvent>> GetUserEvents(Guid userId);

    public Task<AuthenticationEvent> RecordAuthenticationEvent(bool success, User? user, IPAddress? ipAddress);
}