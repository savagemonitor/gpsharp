namespace GpSharp.GoPro.Status;

using System;
using System.Threading.Tasks;
using GpSharp.GoPro.Connectors;

/// <summary>
/// Provides a collection of status items for the
/// camera via HTTP.
/// </summary>
public class StatusesContainer : IStatusesContainer
{
    private readonly IHttpConnection _connection;

    public IStatus InternalBatteryPercentage { get; }
    public IStatus IsEncoding { get; }

    public StatusesContainer(IHttpConnection connection)
    {
        _connection = connection;
        InternalBatteryPercentage = new GenericStatus(_connection, "internal_battery_percentage");
        IsEncoding = new GenericStatus(_connection, "is_encoding");
    }
}

/// <summary>
/// Defines the interface for camera statuses.
/// </summary>
public interface IStatusesContainer
{
    IStatus InternalBatteryPercentage { get; }
    IStatus IsEncoding { get; }
}

/// <summary>
/// A generic implementation of a status item that performs HTTP operations.
/// </summary>
internal class GenericStatus : IStatus
{
    private readonly IHttpConnection _connection;
    private readonly string _key;
    public object Value { get; private set; } = default!;

    public GenericStatus(IHttpConnection connection, string key)
    {
        _connection = connection;
        _key = key;
    }

    /// <summary>
    /// Retrieves the value of the status from the camera.
    /// </summary>
    public async Task<T?> GetValueAsync<T>()
    {
        try
        {
            var response = await _connection.GetAsync($"/status/{_key}");
            return default(T);
        }
        catch
        {
            return default(T);
        }
    }

    /// <summary>
    /// Sets the value of the status on the camera.
    /// </summary>
    public async Task SetValueAsync<T>(T value)
    {
        var payload = new { value };
        await _connection.PutAsync($"/status/{_key}", payload);
    }
}
