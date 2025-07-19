﻿using System.Reflection;
using OIDC.Core_Redux.Util.Validators;

namespace OIDC.Core_Redux.DAL.Strategies;

public class OAuthStrategiser
{
    public enum Strategy
    {
        AuthorizationCode,
        AuthorizationCodeExchange,
        Pkce,
        PkceExchange,
        ClientCredentials,
        DeviceCode,
        RefreshToken,
        PasswordGrant,
        OpenIdConnect,
        Unknown
    };
    
    public string? ResponseType { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? RedirectUri { get; set; }
    public string? Scope { get; set; }
    public string? State { get; set; }
    public string? Code { get; set; }
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
    public string? CodeVerifier { get; set; }
    public string? GrantType { get; set; }
    public string? DeviceCode { get; set; }
    public string? RefreshToken { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

    public List<string> Scopes { get; set; } = new List<string>();

    public OAuthStrategiser(string queryString)
    {
        var splitQuery = queryString.Split('&').ToList();

        // Iterate over every pair in the query string provided, use reflection to check
        // if the key matches a property on this. This API does not support passing these values
        // in the request body to remain spec compliant.
        foreach (string queryPair in splitQuery)
        {
            string[] parts = queryPair.Split("=");
            string key = parts[0].Replace("&", "").Replace("?", "");
            string value = parts[1];

            key = NormaliseKey(key);
            PropertyInfo? dynamicProperty = GetType().GetTypeInfo().GetDeclaredProperty(key);
            if (dynamicProperty == null)
            {
                return;
            }
            
            dynamicProperty.SetValue(this, Convert.ChangeType(value, dynamicProperty.PropertyType));
        }
    }

    // Secondary constructor for request bodies, primarily back channel requests
    public OAuthStrategiser(Dictionary<string, string> body)
    {
        foreach (var kv in body)
        {
            string key = NormaliseKey(kv.Key);
            PropertyInfo? dynamicProperty = GetType().GetTypeInfo().GetDeclaredProperty(key);
            if (dynamicProperty == null)
            {
                return;
            }

            dynamicProperty.SetValue(this, Convert.ChangeType(kv.Value, dynamicProperty.PropertyType));
        }
    }
    
    public Strategy DetermineStrategy()
    {
        BuildScopes(true);

        // Existence
        bool wantsCodeResponse = ResponseType is "code";
        bool hasOpenIdScope = Scopes.Contains("openid");
        bool hasCode = !string.IsNullOrEmpty(Code);
        bool hasClientId = !string.IsNullOrEmpty(ClientId);
        bool hasClientSecret = !string.IsNullOrEmpty(ClientSecret);
        bool hasRedirectUri = !string.IsNullOrEmpty(RedirectUri);
        bool hasScope = !string.IsNullOrEmpty(Scope);
        bool hasState = !string.IsNullOrEmpty(State);
        bool hasCodeChallenge = !string.IsNullOrEmpty(CodeChallenge);
        bool hasCodeChallengeMethod = !string.IsNullOrEmpty(CodeChallengeMethod);
        bool hasCodeVerifier = !string.IsNullOrEmpty(CodeVerifier);
        bool hasGrantType = !string.IsNullOrEmpty(GrantType);
        bool hasDeviceCode = !string.IsNullOrEmpty(DeviceCode);
        bool hasRefreshToken = !string.IsNullOrEmpty(RefreshToken);
        bool hasUsername = !string.IsNullOrEmpty(Username);
        bool hasPassword = !string.IsNullOrEmpty(Password);
        
        // Validity
        bool validResponseType = ResponseTypeValidator.Validate(ResponseType);
        bool validCodeChallengeMethod = CodeChallengeMethodValidator.Validate(CodeChallengeMethod);
        bool validGrantType = GrantTypeValidator.Validate(GrantType);

        // Comparison
        bool isAuthorizationCode = wantsCodeResponse && 
                                   validResponseType && 
                                   hasClientId && 
                                   hasRedirectUri && 
                                   hasScope && 
                                   hasState;
        bool isAuthorizationCodeExchange = hasGrantType &&
                                           validGrantType &&
                                           GrantType == "authorization_code" &&
                                           hasCode &&
                                           hasRedirectUri &&
                                           hasClientId &&
                                           hasClientSecret;
        bool isPkce = wantsCodeResponse && 
                      validResponseType && 
                      hasClientId && 
                      hasRedirectUri && 
                      hasScope && 
                      hasState && 
                      hasCodeChallenge && 
                      hasCodeChallengeMethod && 
                      validCodeChallengeMethod;
        bool isPkceExchange = hasGrantType &&
                              validGrantType &&
                              GrantType == "authorization_code" &&
                              hasCode &&
                              hasRedirectUri &&
                              hasClientId &&
                              hasCodeVerifier;
        bool isClientCredentials = !wantsCodeResponse &&
                                   hasGrantType &&
                                   validGrantType &&
                                   GrantType == "client_credentials" &&
                                   hasClientId &&
                                   hasClientSecret;
        bool isDeviceCode = !wantsCodeResponse &&
                            hasGrantType &&
                            validGrantType &&
                            hasClientId &&
                            hasDeviceCode;
        bool isRefreshToken = !wantsCodeResponse &&
                              hasGrantType &&
                              validGrantType &&
                              GrantType == "refresh_token" &&
                              hasRefreshToken &&
                              hasClientId &&
                              hasClientSecret;
        bool isPasswordGrant = !wantsCodeResponse &&
                               hasGrantType &&
                               validGrantType &&
                               GrantType == "password" &&
                               hasUsername &&
                               hasPassword &&
                               hasClientId;

        bool isOidcGrant = wantsCodeResponse &&
                           hasGrantType &&
                           validGrantType &&
                           hasOpenIdScope &&
                           hasClientId &&
                           hasState &&
                           hasRedirectUri;
        
        if (isAuthorizationCode)
        {
            return Strategy.AuthorizationCode;
        }

        if (isAuthorizationCodeExchange)
        {
            return Strategy.AuthorizationCodeExchange;
        }

        if (isPkce)
        {
            return Strategy.Pkce;
        }

        if (isPkceExchange)
        {
            return Strategy.PkceExchange;
        }

        if (isClientCredentials)
        {
            return Strategy.ClientCredentials;
        }

        if (isDeviceCode)
        {
            return Strategy.DeviceCode;
        }

        if (isRefreshToken)
        {
            return Strategy.RefreshToken;
        }

        if (isPasswordGrant)
        {
            return Strategy.PasswordGrant;
        }

        if (isOidcGrant)
        {
            return Strategy.OpenIdConnect;
        }
        
        // If we made it all the way down here and haven't managed to match a combination of arguments
        // and validation to one of the strategies - we'll send a /shrug with a Strategy.Unknown
        return Strategy.Unknown;
    }

    // Transform the space-delimited set of case-sensitive strings into a list.
    // Some services use a comma-delimited list but the spec calls for a space-delimited one
    // so that's what we'll do here. The current list of transformed scopes can be cleared by
    // settting the clearList argument to true.
    private void BuildScopes(bool clearList = false)
    {
        if (Scope == null)
        {
            return;
        }

        if (clearList)
        {
            Scopes.Clear();
        }

        Scope = Uri.UnescapeDataString(Scope);
        Scopes = Scope.Split(" ").ToList();
    }

    private string NormaliseKey(string key)
    {
        // Locate the underscore and capitalise the next letter after it
        int index = key.IndexOf('_');
        Char[] charArray = key.ToCharArray();
        charArray[index + 1] = Char.ToUpper(charArray[index + 1]);
            
        // Uppercase the first letter
        charArray[0] = Char.ToUpper(charArray[0]);
            
        // Re-construct string from char array and remove underscore
        return new string(charArray).Replace("_", "");
    }
}