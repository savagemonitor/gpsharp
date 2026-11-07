using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GpSharp.GoPro.Connectors;

/// <summary>
/// Handles the underlying HTTP communication with a GoPro device.
/// </summary>
public class HttpConnection : IHttpConnection, IDisposable
{
    private const int HttpRetries = 5;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public HttpConnection(string baseUrl) : this(baseUrl, new HttpClient())
    {
    }

    public HttpConnection(string baseUrl, HttpClient httpClient)
    {
        _baseUrl = baseUrl;
        _httpClient = httpClient;
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }
    }

    public async Task<string> GetAsync(string path)
    {
        return await SendAsync(_ => _httpClient.GetAsync(path), CancellationToken.None);
    }

    public async Task<string> GetAsync(string path, CancellationToken cancellationToken)
    {
        return await SendAsync(token => _httpClient.GetAsync(path, token), cancellationToken);
    }

    public async Task<string> PostAsync(string path, object content)
    {
        return await SendAsync(_ => _httpClient.PostAsJsonAsync(path, content), CancellationToken.None);
    }

    public async Task<string> PutAsync<T>(string path, T content)
    {
        return await SendAsync(_ => _httpClient.PutAsJsonAsync(path, content), CancellationToken.None);
    }

    private static async Task<string> SendAsync(
        Func<CancellationToken, Task<HttpResponseMessage>> send,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                using var response = await send(cancellationToken).ConfigureAwait(false);
                var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpResponseException(response.StatusCode, body);
                }

                return body;
            }
            catch (HttpResponseException error) when (
                (int)error.ResponseStatusCode >= (int)HttpStatusCode.InternalServerError &&
                attempt < HttpRetries - 1)
            {
            }
            catch (HttpRequestException error) when (
                error.StatusCode is null && attempt < HttpRetries - 1)
            {
            }
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
