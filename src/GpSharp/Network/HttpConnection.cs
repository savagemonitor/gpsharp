using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GpSharp.Network;

/// <summary>
/// Handles the underlying HTTP communication with a GoPro device.
/// </summary>
public class HttpConnection : IHttpConnection, IDisposable
{
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
        var response = await _httpClient.GetAsync(path);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string path, object content)
    {
        var response = await _httpClient.PostAsJsonAsync(path, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PutAsync<T>(string path, T content)
    {
        var response = await _httpClient.PutAsJsonAsync(path, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
