using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using OIDC.Core_Redux.Services.Interface;

namespace OIDC.Core_Redux.DAL.ViewModels.Configuration;

// Spec requires that all these are snake case properties so there's a lot of JsonPropertyName declarations in here
public class OpenIdConnectConfiguration
{
    [Description("REQUIRED. URL using the https scheme with no query or fragment components that the OP asserts as its Issuer Identifier. If Issuer discovery is supported (see Section 2), this value MUST be identical to the issuer value returned by WebFinger. This also MUST be identical to the iss Claim value in ID Tokens issued from this Issuer.")]
    [Required]
    public string Issuer { get; set; }
    
    [Description("REQUIRED. URL of the OP's OAuth 2.0 Authorization Endpoint [OpenID.Core]. This URL MUST use the https scheme and MAY contain port, path, and query parameter components.")]
    [JsonPropertyName("authorization_endpoint")]
    [Required]
    public string AuthorizationEndpoint { get; set; }

    // Since we don't support implicit flow - this will always be required
    [Description("URL of the OP's OAuth 2.0 Token Endpoint [OpenID.Core]. This is REQUIRED unless only the Implicit Flow is used. This URL MUST use the https scheme and MAY contain port, path, and query parameter components.")]
    [JsonPropertyName("token_endpoint")] 
    [Required]
    public string TokenEndpoint { get; set; }

    [Description("RECOMMENDED. URL of the OP's UserInfo Endpoint [OpenID.Core]. This URL MUST use the https scheme and MAY contain port, path, and query parameter components.")]
    [JsonPropertyName("userinfo_endpoint")]
    public string? UserInfoEndpoint { get; set; }

    [Description("REQUIRED. URL of the OP's JWK Set [JWK] document, which MUST use the https scheme. This contains the signing key(s) the RP uses to validate signatures from the OP. The JWK Set MAY also contain the Server's encryption key(s), which are used by RPs to encrypt requests to the Server. When both signing and encryption keys are made available, a use (public key use) parameter value is REQUIRED for all keys in the referenced JWK Set to indicate each key's intended usage. Although some algorithms allow the same key to be used for both signatures and encryption, doing so is NOT RECOMMENDED, as it is less secure. The JWK x5c parameter MAY be used to provide X.509 representations of keys provided. When used, the bare key values MUST still be present and MUST match those in the certificate. The JWK Set MUST NOT contain private or symmetric key values.")]
    [JsonPropertyName("jwks_uri")]
    [Required]
    public string JwksUri { get; set; }
    
    [Description("RECOMMENDED. URL of the OP's Dynamic Client Registration Endpoint [OpenID.Registration], which MUST use the https scheme.")]
    [JsonPropertyName("registration_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RegistrationEndpoint { get; set; }

    [Description("RECOMMENDED. JSON array containing a list of the OAuth 2.0 [RFC6749] scope values that this server supports. The server MUST support the openid scope value. Servers MAY choose not to advertise some supported scope values even when this parameter is used, although those defined in [OpenID.Core] SHOULD be listed, if supported.")]
    [JsonPropertyName("scopes_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? ScopesSupported { get; set; }
    
    [Description("REQUIRED. JSON array containing a list of the OAuth 2.0 response_type values that this OP supports. Dynamic OpenID Providers MUST support the code, id_token, and the id_token token Response Type values.")]
    [JsonPropertyName("response_types_supported")]
    [Required]
    public IList<string> ResponseTypesSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the OAuth 2.0 response_mode values that this OP supports, as specified in OAuth 2.0 Multiple Response Type Encoding Practices [OAuth.Responses]. If omitted, the default for Dynamic OpenID Providers is [\"query\", \"fragment\"].")]
    [JsonPropertyName("response_modes_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? ResponseModesSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the OAuth 2.0 Grant Type values that this OP supports. Dynamic OpenID Providers MUST support the authorization_code and implicit Grant Type values and MAY support other Grant Types. If omitted, the default value is [\"authorization_code\", \"implicit\"].")]
    [JsonPropertyName("grant_types_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? GrantTypesSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the Authentication Context Class References that this OP supports.")]
    [JsonPropertyName("acr_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? AcrValuesSupported { get; set; }

    // This will never change - hard coding for spec compliance
    [Description("REQUIRED. JSON array containing a list of the Subject Identifier types that this OP supports. Valid types include pairwise and public.")]
    [JsonPropertyName("subject_types_supported")]
    [Required]
    public IList<string> SubjectTypesSupported { get; set; }
    
    // Support will need adding for the different algorithms in the controller but for development we'll use 1
    [Description("REQUIRED. JSON array containing a list of the JWS signing algorithms (alg values) supported by the OP for the ID Token to encode the Claims in a JWT [JWT]. The algorithm RS256 MUST be included. The value none MAY be supported but MUST NOT be used unless the Response Type used returns no ID Token from the Authorization Endpoint (such as when using the Authorization Code Flow).")]
    [JsonPropertyName("id_token_signing_alg_values_supported")]
    [Required]
    public IList<string> IdTokenSigningAlgValuesSupported { get; set; }
    
    [Description("OPTIONAL. JSON array containing a list of the JWE encryption algorithms (alg values) supported by the OP for the ID Token to encode the Claims in a JWT [JWT].")]
    [JsonPropertyName("id_token_encryption_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? IdTokenEncryptionAlgValuesSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the JWE encryption algorithms (enc values) supported by the OP for the ID Token to encode the Claims in a JWT [JWT].")]
    [JsonPropertyName("id_token_encryption_enc_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? IdTokenEncryptionEncValuesSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the JWS [JWS] signing algorithms (alg values) [JWA] supported by the UserInfo Endpoint to encode the Claims in a JWT [JWT]. The value none MAY be included.")]
    [JsonPropertyName("userinfo_signing_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? UserInfoSigningAlgValuesSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the JWE encryption algorithms (enc values) [JWA] supported by the UserInfo Endpoint to encode the Claims in a JWT [JWT].")]
    [JsonPropertyName("userinfo_encryption_enc_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? UserInfoEncryptionAlgValuesSupported { get; set; }
    
    [Description("OPTIONAL. JSON array containing a list of the JWS signing algorithms (alg values) supported by the OP for Request Objects, which are described in Section 6.1 of OpenID Connect Core 1.0 [OpenID.Core]. These algorithms are used both when the Request Object is passed by value (using the request parameter) and when it is passed by reference (using the request_uri parameter). Servers SHOULD support none and RS256.")]
    [JsonPropertyName("request_object_signing_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? RequestObjectSigningAlgValuesSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the JWE encryption algorithms (alg values) supported by the OP for Request Objects. These algorithms are used both when the Request Object is passed by value and when it is passed by reference.")]
    [JsonPropertyName("request_object_encryption_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? RequestObjectEncryptionAlgValuesSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the JWE encryption algorithms (enc values) supported by the OP for Request Objects. These algorithms are used both when the Request Object is passed by value and when it is passed by reference.")]
    [JsonPropertyName("request_object_encryption_enc_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? RequestObjectEncryptionEncValuesSupported { get; set; }

    [Description(
        "OPTIONAL. JSON array containing a list of Client Authentication methods supported by this Token Endpoint. The options are client_secret_post, client_secret_basic, client_secret_jwt, and private_key_jwt, as described in Section 9 of OpenID Connect Core 1.0 [OpenID.Core]. Other authentication methods MAY be defined by extensions. If omitted, the default is client_secret_basic -- the HTTP Basic Authentication Scheme specified in Section 2.3.1 of OAuth 2.0 [RFC6749].")]
    [JsonPropertyName("token_endpoint_auth_methods_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? TokenEndpointAuthMethodsSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the JWS signing algorithms (alg values) supported by the Token Endpoint for the signature on the JWT [JWT] used to authenticate the Client at the Token Endpoint for the private_key_jwt and client_secret_jwt authentication methods. Servers SHOULD support RS256. The value none MUST NOT be used.")]
    [JsonPropertyName("token_endpoint_auth_signing_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? TokenEndpointAuthSigningAlgValuesSupported { get; set; }
    
    [Description("OPTIONAL. JSON array containing a list of the display parameter values that the OpenID Provider supports. These values are described in Section 3.1.2.1 of OpenID Connect Core 1.0 [OpenID.Core].")]
    [JsonPropertyName("display_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? DisplayValuesSupported { get; set; }

    [Description("OPTIONAL. JSON array containing a list of the Claim Types that the OpenID Provider supports. These Claim Types are described in Section 5.6 of OpenID Connect Core 1.0 [OpenID.Core]. Values defined by this specification are normal, aggregated, and distributed. If omitted, the implementation supports only normal Claims.")]
    [JsonPropertyName("claim_types_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? ClaimTypesSupported { get; set; }

    [Description(
        "RECOMMENDED. JSON array containing a list of the Claim Names of the Claims that the OpenID Provider MAY be able to supply values for. Note that for privacy or other reasons, this might not be an exhaustive list.")]
    [JsonPropertyName("claims_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? ClaimsSupported { get; set; }

    [Description("OPTIONAL. URL of a page containing human-readable information that developers might want or need to know when using the OpenID Provider. In particular, if the OpenID Provider does not support Dynamic Client Registration, then information on how to register Clients needs to be provided in this documentation.")]
    [JsonPropertyName("service_documentation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ServiceDocumentation { get; set; }

    [Description("OPTIONAL. Languages and scripts supported for values in Claims being returned, represented as a JSON array of BCP47 [RFC5646] language tag values. Not all languages and scripts are necessarily supported for all Claim values.")]
    [JsonPropertyName("claims_locales_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? ClaimsLocalesSupported { get; set; }

    [Description("OPTIONAL. Languages and scripts supported for the user interface, represented as a JSON array of BCP47 [RFC5646] language tag values.")]
    [JsonPropertyName("ui_locales_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? UiLocalesSupported { get; set; }
    
    [Description("OPTIONAL. Boolean value specifying whether the OP supports use of the claims parameter, with true indicating support. If omitted, the default value is false.")]
    [JsonPropertyName("claims_parameter_supported")]
    public bool ClaimsParameterSupported { get; set; } = false;

    [Description("OPTIONAL. Boolean value specifying whether the OP supports use of the request parameter, with true indicating support. If omitted, the default value is false.")]
    [JsonPropertyName("request_parameter_supported")]
    public bool RequestParameterSupported { get; set; } = false;

    [Description("OPTIONAL. Boolean value specifying whether the OP supports use of the request_uri parameter, with true indicating support. If omitted, the default value is true.")]
    [JsonPropertyName("request_uri_parameter_supported")]
    public bool RequestUriParameterSupported { get; set; } = false;

    [Description("OPTIONAL. Boolean value specifying whether the OP requires any request_uri values used to be pre-registered using the request_uris registration parameter. Pre-registration is REQUIRED when the value is true. If omitted, the default value is false.")]
    [JsonPropertyName("require_request_uri_registration")]
    public bool RequireRequestUriRegistration { get; set; } = false;

    [Description("OPTIONAL. URL that the OpenID Provider provides to the person registering the Client to read about the OP's requirements on how the Relying Party can use the data provided by the OP. The registration process SHOULD display this URL to the person registering the Client if it is given.")]
    [JsonPropertyName("op_policy_uri")]
    public string? OpPolicyUri { get; set; }

    [Description("OPTIONAL. URL that the OpenID Provider provides to the person registering the Client to read about the OpenID Provider's terms of service. The registration process SHOULD display this URL to the person registering the Client if it is given.")]
    [JsonPropertyName("op_tos_uri")]
    public string? OpTosUri { get; set; }

    public OpenIdConnectConfiguration(IConfiguration configuration)
    {
        // Required properties
        Issuer = configuration.GetValue<string>("OIDC:Issuer") ?? throw new InvalidOperationException("Missing issuer from OIDC config");
        AuthorizationEndpoint = configuration.GetValue<string>("OIDC:AuthorizationEndpoint") ?? throw new InvalidOperationException("Missing authorization endpoint from OIDC config");
        TokenEndpoint = configuration.GetValue<string>("OIDC:TokenEndpoint") ?? throw new InvalidOperationException("Missing token endpoint from OIDC config");
        UserInfoEndpoint = configuration.GetValue<string>("OIDC:UserInfoEndpoint") ?? throw new InvalidOperationException("Missing userinfo endpoint from OIDC config");
        JwksUri = configuration.GetValue<string>("OIDC:JwksUri") ?? throw new InvalidOperationException("Missing jwks uri from OIDC config");
        RegistrationEndpoint = configuration.GetValue<string>("OIDC:RegistrationEndpoint") ?? throw new InvalidOperationException("Missing registration endpoint from OIDC config");
        ScopesSupported = configuration.GetSection("OIDC:ScopesSupported").Get<List<string>>() ?? throw new InvalidOperationException("Missing supported scopes from OIDC config");
        ResponseTypesSupported = configuration.GetSection("OIDC:ResponseTypesSupported").Get<List<string>>() ?? throw new InvalidOperationException("Missing supported response types from OIDC config");
        SubjectTypesSupported = configuration.GetSection("OIDC:SubjectTypesSupported").Get<List<string>>() ?? throw new InvalidOperationException("Missing supported subject types from OIDC config");
        IdTokenSigningAlgValuesSupported = configuration.GetSection("OIDC:IdTokenSigningAlgValuesSupported").Get<List<string>>() ?? throw new InvalidOperationException("Missing id token signing alg values supported from OIDC config");
        
        // Optional and recommended properties
        ResponseModesSupported = configuration.GetSection("OIDC:ResponseModesSupported").Get<List<string>>() ?? null;
        GrantTypesSupported = configuration.GetSection("OIDC:GrantTypesSupported").Get<List<string>>() ?? null;
        AcrValuesSupported = configuration.GetSection("OIDC:AcrValuesSupported").Get<List<string>>() ?? null;
        IdTokenEncryptionAlgValuesSupported = configuration.GetSection("OIDC:IdTokenEncryptionAlgValuesSupported").Get<List<string>>() ?? null;
        IdTokenEncryptionEncValuesSupported = configuration.GetSection("OIDC:IdTokenEncryptionEncValuesSupported").Get<List<string>>() ?? null;
        UserInfoSigningAlgValuesSupported = configuration.GetSection("OIDC:UserInfoSigningAlgValuesSupported").Get<List<string>>() ?? null;
        UserInfoEncryptionAlgValuesSupported = configuration.GetSection("OIDC:UserInfoEncryptionAlgValuesSupported").Get<List<string>>() ?? null;
        RequestObjectSigningAlgValuesSupported = configuration.GetSection("OIDC:RequestObjectSigningAlgValuesSupported").Get<List<string>>() ?? null;
        RequestObjectEncryptionAlgValuesSupported = configuration.GetSection("OIDC:RequestObjectEncryptionAlgValuesSupported").Get<List<string>>() ?? null;
        RequestObjectEncryptionEncValuesSupported = configuration.GetSection("OIDC:RequestObjectEncryptionEncValuesSupported").Get<List<string>>() ?? null;
        TokenEndpointAuthMethodsSupported = configuration.GetSection("OIDC:TokenEndpointAuthMethodsSupported").Get<List<string>>() ?? null;
        TokenEndpointAuthSigningAlgValuesSupported = configuration.GetSection("OIDC:TokenEndpointAuthSigningAlgValuesSupported").Get<List<string>>() ?? null;
        DisplayValuesSupported = configuration.GetSection("OIDC:DisplayValuesSupported").Get<List<string>>() ?? null;
        ClaimTypesSupported = configuration.GetSection("OIDC:ClaimTypesSupported").Get<List<string>>() ?? null;
        ClaimsSupported = configuration.GetSection("OIDC:ClaimsSupported").Get<List<string>>() ?? null;
        ServiceDocumentation = configuration.GetValue<string>("OIDC:ServiceDocumentation") ?? null;
        ClaimsLocalesSupported = configuration.GetSection("OIDC:ClaimsLocalesSupported").Get<List<string>>() ?? null;
        UiLocalesSupported = configuration.GetSection("OIDC:UiLocalesSupported").Get<List<string>>() ?? null;
        ClaimsParameterSupported = configuration.GetValue("OIDC:ClaimsParameterSupported", false);
        RequestParameterSupported = configuration.GetValue("OIDC:RequestParameterSupported", false);
        RequestUriParameterSupported = configuration.GetValue("OIDC:RequestUriParameterSupported", false);
        RequireRequestUriRegistration = configuration.GetValue("OIDC:RequireRequestUriRegistration", false);
        OpPolicyUri = configuration.GetValue<string>("OIDC:OpPolicyUrl") ?? null;
        OpTosUri = configuration.GetValue<string>("OIDC:OpTosUrl") ?? null;
    }
}