using CrossUtility.Helpers;
using System;
using System.Text.Json.Serialization;
using static CrossUtility.Services.Identity.CryptoHelper;

namespace CrossUtility.Services.Identity.Requests
{
    public class AuthorizationRequest : IdentityRequest
    {
        private const int StandardMinCodeVerifierLength = 43;
        private const int StandardMaxCodeVerifierLength = 128;

        public AuthorizationRequest(
            string endPoint,
            string redirectUri,
            string clientId,
            string responseType,
            string scope) : base(endPoint, clientId, scope)
        {
            RedirectUri = redirectUri;
            ResponseType = responseType;
        }

        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; }

        [JsonPropertyName("response_type")]
        public string ResponseType { get; }

        [JsonPropertyName("state")]
        public Guid State { get; } = Guid.NewGuid();

        [JsonPropertyName("code_challenge")]
        public string CodeChallenge => GenerateCodeChallenge();

        [JsonPropertyName("code_challenge_method")]
        public string CodeChallengeMethod => ChallengeMethod.ToString();

        [JsonIgnore]
        public string CodeVerifier { get; } = RandomString(StandardMinCodeVerifierLength, StandardMaxCodeVerifierLength);

        [JsonIgnore]
        public ChallengeMethod ChallengeMethod { get; init; }

        public Uri BuildUrl()
        {
            var queryString = QueryStringHelper.ToQueryString(this);
            return new Uri($"{EndPoint}?{queryString}");
        }

        private string GenerateCodeChallenge()
        {
            return ChallengeMethod switch
            {
                ChallengeMethod.Plain => CodeVerifier,
                ChallengeMethod.S256 => ToSha256(CodeVerifier),
                _ => throw new InvalidOperationException($"Invalid {nameof(ChallengeMethod)}")
            };
        }
    }
}
