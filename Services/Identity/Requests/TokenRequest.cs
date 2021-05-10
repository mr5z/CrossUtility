using System;
using System.Text.Json.Serialization;

namespace CrossUtility.Services.Identity.Requests
{
    public abstract class TokenRequest : IdentityRequest
    {
        public TokenRequest(
            string endPoint,
            string clientId,
            SupportedGrantType requestedGrantType,
            string? scope = null) : base(endPoint, clientId, scope)
        {
            RequestedGrantType = requestedGrantType;
        }

        [JsonPropertyName("grant_type")]
        public string GrantType => RequestedGrantType switch
        {
            SupportedGrantType.AuthorizationCode => "authorization_code",
            SupportedGrantType.Password => "password",
            _ => throw new NotSupportedException(nameof(GrantType))
        };

        [JsonIgnore]
        public SupportedGrantType RequestedGrantType { get; }
    }
}
