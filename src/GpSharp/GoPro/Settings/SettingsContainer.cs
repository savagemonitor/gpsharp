namespace GpSharp.GoPro.Settings;

/// <summary>
/// Provides setting API placeholders until BLE support is implemented.
/// </summary>
public class SettingsContainer : ISettingsContainer
{
    public ISetting VideoResolution { get; }
    public ISetting VideoFrameRate { get; }

    public SettingsContainer()
    {
        VideoResolution = new GenericSetting("video_resolution");
        VideoFrameRate = new GenericSetting("video_frame_rate");
    }
}
