using System.Diagnostics.Metrics;
using OIDC.Core.DAL.Entities;

namespace OIDC.Core.Util.Metrics;

public class AuthenticationEvents
{
    private readonly Gauge<int> _authenticationEvents;

    public AuthenticationEvents(IMeterFactory meterFactory)
    {
        Meter authEventsMeter = meterFactory.Create("oidccore.authentication");
        _authenticationEvents = authEventsMeter.CreateGauge<int>("attempts", description: "Authentication attempt regardless of outcome");
    }

    private void Record(bool success = false, User? user = null, string authType = "credentials")
    {
        _authenticationEvents.Record(
            1, 
            new KeyValuePair<string, object?>("user-id", user != null ? user.Id : "no-id"),
            new KeyValuePair<string, object?>("success", success),
            new KeyValuePair<string, object?>("auth-type", authType)
        );
    }

    public void RecordSuccess(User? user = null, string authType = "credentials")
    {
        Record(true, user, authType);
    }

    public void RecordFail(User? user = null, string authType = "credentials")
    {
        Record(false, user, authType);
    }
}