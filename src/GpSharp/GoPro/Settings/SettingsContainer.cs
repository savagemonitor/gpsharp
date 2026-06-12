namespace GpSharp.GoPro.Settings;

using System;
using System.Threading.Tasks;

public class SettingsContainer : ISettingsContainer
{
    // Placeholder for specific settings
    public ISetting VideoResolution { get; }
    public ISetting VideoFrameRate { get; }

    public SettingsContainer()
    {
        VideoResolution = new GenericSetting("video_resolution");
        VideoFrameRate = new GenericSetting("video_frame_rate");
    }
}

public interface ISettingsContainer
{
    ISetting VideoResolution { get; }
    ISetting VideoFrameRate { get; }
}

internal class GenericSetting : ISetting
{
    private readonly string _key;
    public object Value { get; private set; }

    public GenericSetting(string key) => _key = key;

    public Task<T> GetValueAsync<T>()
    {
        // Implementation would fetch from the current connection.
        return Task.FromResult(default(T));
    }

    public Task SetValueAsync<T>(T value)
    {
        // Implementation would send to the current connection.
        return Task.CompletedTask;
    }
}
