namespace GpSharp.GoPro.Status;

using System;
using System.Threading.Tasks;

/// <summary>
/// Provides a collection of status items for the camera.
/// </summary>
public class StatusesContainer : IStatusesContainer
{
    public IStatus InternalBatteryPercentage { get; }
    public IStatus IsEncoding { get; }

    public StatusesContainer()
    {
        InternalBatteryPercentage = new GenericStatus("internal_battery_percentage");
        IsEncoding = new GenericStatus("is_encoding");
    }
}

public interface IStatusesContainer
{
    IStatus InternalBatteryPercentage { get; }
    IStatus IsEncoding { get; }
}

internal class GenericStatus : IStatus
{
    private readonly string _key;
    public object? Value { get; private set; }

    public GenericStatus(string key) => _key = key;

    public Task<T?> GetValueAsync<T>()
    {
        // Implementation would fetch from the current connection.
        return Task.FromResult(default(T));
    }
}
