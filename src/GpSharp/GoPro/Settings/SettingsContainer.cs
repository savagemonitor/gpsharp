namespace GpSharp.GoPro.Settings;

using System;
using System.Threading.Tasks;
using GpSharp.Network;

/// <summary>
/// Provides access to camera settings via HTTP.
/// </summary>
public class SettingsContainer : ISettingsContainer
{
    private readonly IHttpConnection _connection;

    public ISetting VideoResolution { get; }
    public ISetting VideoFrameRate { get; }

    public SettingsContainer(IHttpConnection connection)
    {
        _connection = connection;
        VideoResolution = new GenericSetting(_connection, "video_resolution");
        VideoFrameRate = new GenericSetting(_connection, "video_frame_rate");
    }
}
