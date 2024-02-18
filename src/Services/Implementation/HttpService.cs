using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CrossUtility.Extensions;
using CrossUtility.Helpers;

namespace CrossUtility.Services.Implementation;

public class HttpService(HttpClient httpClient) : IHttpService
{
    public HttpClient HttpClient { get; } = httpClient;

    private static async Task<T> SendAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
    {
        message.EnsureSuccessStatusCode();
        message.EnsureAcceptHeadersMatches();
        var deserializedResponse = await message.Content.ReadFromJsonAsync<T>(cancellationToken);
        return deserializedResponse ?? throw new InvalidOperationException("Cannot deserialize response");
    }
    
    public async Task<T> GetAsync<T>(Uri requestUri, CancellationToken cancellationToken)
    {
        var response = await HttpClient.GetAsync(requestUri, cancellationToken)
                       ?? throw new InvalidOperationException("Response is null");
        return await SendAsync<T>(response, cancellationToken);
    }

    public async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        var response = await HttpClient.GetAsync(path, cancellationToken)
                       ?? throw new InvalidOperationException("Response is null");
        return await SendAsync<T>(response, cancellationToken);
    }

    public async Task<TResponse> PostFormAsync<TResponse, TRequest>(
        Uri requestUri, 
        TRequest request, 
        CancellationToken cancellationToken)
        where TRequest : notnull
    {
        var serialized = JsonHelper.ToKeyValuePairs(request);
        var body = new FormUrlEncodedContent(serialized);
        var response = await HttpClient.PostAsync(requestUri, body, cancellationToken);
        response.EnsureSuccessStatusCode();
        response.EnsureAcceptHeadersMatches();
        var tokenResponse = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken)
                            ?? throw new InvalidOperationException($"Cannot deserialize '{typeof(TResponse).Name}'");
        return tokenResponse;
    }
}