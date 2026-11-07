namespace GpSharp.GoPro.Statuses;

/// <summary>
/// Provides access to camera statuses.
/// </summary>
public interface IStatusesContainer
{
    IStatus InternalBatteryPercentage { get; }
    IStatus IsEncoding { get; }
}
