using System.Diagnostics.Metrics;
using OIDC.Core_Minimal.DAL.Entities;

namespace OIDC.Core_Minimal.Util.Metrics;

public class OAuthEvents
{
    private readonly Gauge<int> _oauthEvents;

    public OAuthEvents(IMeterFactory meterFactory)
    {
        Meter oauthEventsMeter = meterFactory.Create("OIDCCore.OAuth.Events");
        _oauthEvents = oauthEventsMeter.CreateGauge<int>("OIDCCore.OAuth.Events", description: "OAuth events");
    }

    public void Record(string grantType, User? user = null, Application? application = null)
    {
        _oauthEvents.Record(
            1,
            new KeyValuePair<string, object?>("user_id", user != null ? user.Id : "no-user-id"),
            new KeyValuePair<string, object?>("application_id", application != null ? application.Id : "no-application-id"),
            new KeyValuePair<string, object?>("grant_type", grantType)
        );
    }
}