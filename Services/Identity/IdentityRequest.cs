using System.Text.Json.Serialization;

namespace CrossUtility.Services.Identity
{
    public abstract class IdentityRequest
    {
        public IdentityRequest(string endPoint, string clientId, string? scope = null)
        {
            EndPoint = endPoint;
            ClientId = clientId;
            Scope = scope;
        }

        [JsonIgnore]
        public string EndPoint { get; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; }

        [JsonPropertyName("scope")]
        public string? Scope { get; }
    }
}
