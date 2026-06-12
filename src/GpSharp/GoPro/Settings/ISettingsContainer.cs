namespace GpSharp.GoPro.Settings;

/// <summary>
/// Defines the interface for camera settings.
/// </summary>
public interface ISettingsContainer
{
    ISetting VideoResolution { get; }
    ISetting VideoFrameRate { get; }
}