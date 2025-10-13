using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Nkraft.CrossUtility.Extensions;
using Nkraft.CrossUtility.Helpers;

namespace Nkraft.CrossUtility.Services.Implementation;

public class HttpService(HttpClient httpClient) : IHttpService
{
    private readonly HttpClient _httpClient = httpClient;

    HttpClient IHttpService.HttpClient => _httpClient;

    private static async Task<T> DeserializeAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
    {
        message.EnsureSuccessStatusCode();
        message.EnsureAcceptHeadersMatches();
        var deserializedResponse = await message.Content.ReadFromJsonAsync<T>(cancellationToken).ConfigureAwait(false);
        return deserializedResponse ?? throw new InvalidOperationException("Cannot deserialize response.");
    }
    
    async Task<T> IHttpService.GetAsync<T>(Uri requestUri, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false)
                       ?? throw new InvalidOperationException("Response is null.");
        return await DeserializeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    async Task<T> IHttpService.GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(path, cancellationToken).ConfigureAwait(false)
					   ?? throw new InvalidOperationException("Response is null.");
        return await DeserializeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    async Task<TResponse> IHttpService.PostFormAsync<TResponse, TRequest>(
        Uri requestUri, 
        TRequest request, 
        CancellationToken cancellationToken)
    {
        var serialized = JsonHelper.ToKeyValuePairs(request);
        var body = new FormUrlEncodedContent(serialized);
        var response = await _httpClient.PostAsync(requestUri, body, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        response.EnsureAcceptHeadersMatches();
        var tokenResponse = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken).ConfigureAwait(false)
							?? throw new InvalidOperationException($"Cannot deserialize '{typeof(TResponse).Name}'.");
        return tokenResponse;
    }
}