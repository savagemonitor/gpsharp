namespace GpSharp.GoPro.Statuses;

/// <summary>
/// Provides status API placeholders until BLE support is implemented.
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
