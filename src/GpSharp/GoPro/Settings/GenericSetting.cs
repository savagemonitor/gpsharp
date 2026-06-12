namespace GpSharp.GoPro.Settings;

using System;
using System.Threading.Tasks;
using GpSharp.Network;

/// <summary>
/// A generic implementation of a setting that performs HTTP operations.
/// </summary>
internal class GenericSetting : ISetting
{
    private readonly IHttpConnection _connection;
    private readonly string _key;
    public object? Value { get; private set; }

    public GenericSetting(IHttpConnection connection, string key)
    {
        _connection = connection;
        _key = key;
    }

    /// <summary>
    /// Retrieves the value of the setting from the camera.
    /// </summary>
    public async Task<T?> GetValueAsync<T>()
    {
        try
        {
            var response = await _connection.GetAsync($"/settings/{_key}");
            return default(T); 
        }
        catch
        {
            return default(T);
        }
    }

    /// <summary>
    /// Sets the value of the setting on the camera.
    /// </summary>
    public async Task SetValueAsync<T>(T value)
    {
        var payload = new { value };
        await _connection.PutAsync($"/settings/{_key}", payload);
    }
}