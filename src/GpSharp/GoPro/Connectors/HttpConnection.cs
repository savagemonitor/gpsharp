using System;
using System.Net.Http;
using System.Net.Http.Json;
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
        return await SendAsync(() => _httpClient.GetAsync(path));
    }

    public async Task<string> PostAsync(string path, object content)
    {
        return await SendAsync(() => _httpClient.PostAsJsonAsync(path, content));
    }

    public async Task<string> PutAsync<T>(string path, T content)
    {
        return await SendAsync(() => _httpClient.PutAsJsonAsync(path, content));
    }

    private static async Task<string> SendAsync(Func<Task<HttpResponseMessage>> send)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                using var response = await send();
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException) when (attempt < HttpRetries - 1)
            {
            }
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
