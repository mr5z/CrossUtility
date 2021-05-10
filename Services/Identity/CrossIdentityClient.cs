using CrossUtility.Extensions;
using CrossUtility.Helpers;
using CrossUtility.Services.Identity.Requests;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CrossUtility.Services.Identity
{
    public class CrossIdentityClient : IIdentityClient
    {
        private readonly HttpClient httpClient;

        public CrossIdentityClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public virtual Task<AuthorizationResponse> Authorize(AuthorizationRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenResponse> RequestToken(TokenRequest request, CancellationToken cancellationToken)
        {
            var serialized = JsonHelper.ToKeyValuePairs(request);
            var body = new FormUrlEncodedContent(serialized);
            var response = await httpClient.PostAsync(request.EndPoint, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            response.EnsureAcceptHeadersMatches();
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return tokenResponse!;
        }
    }
}
