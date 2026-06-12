namespace GpSharp.Network;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

/// <summary>
/// Handles the underlying HTTP communication with a GoPro device.
/// </summary>
public class HttpConnection : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public HttpConnection(string baseUrl)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
    }

    public async Task<string> GetAsync(string path)
    {
        var response = await _httpClient.GetAsync(path);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
