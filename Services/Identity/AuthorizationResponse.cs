using System;
using System.Text.Json.Serialization;

namespace CrossUtility.Services.Identity
{
    public class AuthorizationResponse
    {
        [JsonPropertyName("code")]
        public string? Code { get; init; }
        [JsonPropertyName("scope")]
        public string? Scope { get; init; }
        [JsonPropertyName("state")]
        public Guid State { get; init; }
        [JsonPropertyName("session_state")]
        public string? SessionState { get; init; }
    }
}
