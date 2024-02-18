using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CrossUtility.Services;

public interface IHttpService
{
    HttpClient HttpClient { get; }
    
    Task<T> GetAsync<T>(string relativePath, CancellationToken cancellationToken = default);
    Task<T> GetAsync<T>(Uri requestUri, CancellationToken cancellationToken = default);
    
    Task<TResponse> PostFormAsync<TResponse, TRequest>(
        Uri requestUri, 
        TRequest request, 
        CancellationToken cancellationToken = default)
        where TRequest : notnull;
}