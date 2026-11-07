namespace GpSharp.GoPro.Connectors;

using System;
using System.Net;
using System.Net.Http;

/// <summary>
/// Represents an unsuccessful HTTP response and retains its body for protocol-specific errors.
/// </summary>
public sealed class HttpResponseException : HttpRequestException
{
    public HttpStatusCode ResponseStatusCode { get; }
    public string ResponseBody { get; }

    public HttpResponseException(HttpStatusCode statusCode, string responseBody)
        : base($"The HTTP request failed with status code {(int)statusCode} ({statusCode}).", null, statusCode)
    {
        ResponseStatusCode = statusCode;
        ResponseBody = responseBody ?? throw new ArgumentNullException(nameof(responseBody));
    }
}
