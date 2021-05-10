using System;
using System.Text.Json.Serialization;

namespace CrossUtility.Services.Identity.Requests
{
    public class AuthorizationTokenRequest : TokenRequest
    {
        public AuthorizationTokenRequest(
            string endPoint,
            string clientId,
            string redirectUri,
            Guid state,
            string codeVerifier,
            string? code,
            string? scope = null) : base(endPoint, clientId, SupportedGrantType.AuthorizationCode, scope)
        {
            RedirectUri = redirectUri;
            State = state;
            CodeVerifier = codeVerifier;
            Code = code;
        }

        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; }

        [JsonPropertyName("state")]
        public Guid State { get; }

        [JsonPropertyName("code_verifier")]
        public string CodeVerifier { get; }

        [JsonPropertyName("code")]
        public string? Code { get; }
    }
}
