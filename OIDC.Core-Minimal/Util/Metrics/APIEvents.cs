using System.Diagnostics;
using System.Diagnostics.Metrics;
using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.Util.Metrics;

public class APIEvents
{
    private readonly ActivitySource _activitySource;
    private readonly Meter _rootMeter;
    private readonly UpDownCounter<int> _oauthEvents;
    private readonly UpDownCounter<int> _authenticationEvents;

    public APIEvents(
        IMeterFactory meterFactory, 
        IConfiguration config,
        ILogger<APIEvents> logger
    )
    {
        _activitySource = new ActivitySource("OIDCCore.API");
        string rootMeterName = "OIDCCore.API";
        logger.LogInformation(rootMeterName);
        
        _rootMeter = meterFactory.Create(rootMeterName);
        _oauthEvents = _rootMeter.CreateUpDownCounter<int>(name: "oidccore.api.oauth.grant_events", description: "OAuth events");
        _authenticationEvents = _rootMeter.CreateUpDownCounter<int>("oidccore.api.authentication.attempts",
            description: "Authentication attempts");
    }

    public void RecordOAuthGrantUsage(string grantType, User? user = null, Application? application = null, bool success = true)
    {
        _oauthEvents.Add(
            1,
            new KeyValuePair<string, object?>("user_id", user != null ? user.Id : "no-user-id"),
            new KeyValuePair<string, object?>("application_id", application != null ? application.Id : "no-application-id"),
            new KeyValuePair<string, object?>("grant_type", grantType),
            new KeyValuePair<string, object?>("success", success)
        );
    }

    public void RecordOAuthGrantUsage(string grantType, Guid userId, Application? application = null, bool success = true)
    {
        _oauthEvents.Add(
            1,
            new KeyValuePair<string, object?>("user_id", userId),
            new KeyValuePair<string, object?>("application_id", application != null ? application.Id : "no-application-id"),
            new KeyValuePair<string, object?>("grant_type", grantType),
            new KeyValuePair<string, object?>("success", success)
        );
    }
    
    public void RecordAuthenticationAttempt(User? user = null, string authType = "credentials", bool success = true)
    {
        _authenticationEvents.Add(
            1, 
            new KeyValuePair<string, object?>("user-id", user != null ? user.Id : "no-id"),
            new KeyValuePair<string, object?>("success", success),
            new KeyValuePair<string, object?>("auth-type", authType)
        );
    }

}