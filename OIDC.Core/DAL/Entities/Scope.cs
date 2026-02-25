using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OIDC.Core.DAL.Configuration;

namespace OIDC.Core.DAL.Entities;

[EntityTypeConfiguration(typeof(ScopeConfiguration))]
public class Scope
{
    public string Name { get; set; }

    [JsonIgnore]
    public ICollection<AccessToken> AccessTokens { get; set; }

    public Scope(string name)
    {
        Name = name;
        AccessTokens = new List<AccessToken>();
    }
}