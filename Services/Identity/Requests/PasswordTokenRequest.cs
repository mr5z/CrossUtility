using System.Text.Json.Serialization;

namespace CrossUtility.Services.Identity.Requests
{
    public class PasswordTokenRequest : TokenRequest
    {
        public PasswordTokenRequest(
            string endPoint,
            string clientId,
            string? username,
            string? password,
            string? scope = null)
            : base(endPoint, clientId, SupportedGrantType.Password, scope)
        {
            Username = username;
            Password = password;
        }

        [JsonPropertyName("username")]
        public string? Username { get; }

        [JsonPropertyName("password")]
        public string? Password { get; }
    }
}
