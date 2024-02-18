using System;
using System.Linq;
using System.Net.Http;

namespace CrossUtility.Extensions;

public static class HttpResponseMessageExtension
{
    public static void EnsureAcceptHeadersMatches(this HttpResponseMessage responseMessage)
    {
        var requestAcceptHeader = responseMessage.RequestMessage?.Headers.Accept
            ?? throw new InvalidOperationException("'Accept' header not found");
        var responseContentType = responseMessage.Content?.Headers.ContentType
            ?? throw new InvalidOperationException("'Content-Type' header not found");

        if (requestAcceptHeader.IsNullOrEmpty())
            return;

        if (requestAcceptHeader.Any(acceptHeader => acceptHeader.MediaType == responseContentType.MediaType))
            return;

        responseMessage.Dispose();

        var expectedMediaTypes = string.Join(", ", requestAcceptHeader.Select(it => it.MediaType));
        throw new HttpRequestException(
            $"Unexpected Content-Type: '{responseContentType?.MediaType}', expecting: '{expectedMediaTypes}'"
        );
    }
}
