namespace GpSharp.GoPro.Settings;

using System;
using GpSharp.GoPro.Connectors;

/// <summary>
/// Provides access to camera settings over the Open GoPro HTTP API.
/// </summary>
public class SettingsContainer : ISettingsContainer
{
    public ISetting VideoResolution { get; }
    public ISetting VideoFrameRate { get; }

    private readonly IHttpConnection _connection;

    public SettingsContainer(IHttpConnection connection)
    {
        _connection = connection;
        VideoResolution = new GenericSetting(2, _connection);
        VideoFrameRate = new GenericSetting(3, _connection);
    }

    public ISetting GetSetting(int settingId) =>
        new GenericSetting(ValidateId(settingId, nameof(settingId)), _connection);

    internal static byte ValidateId(int id, string parameterName) =>
        id is >= byte.MinValue and <= byte.MaxValue
            ? (byte)id
            : throw new ArgumentOutOfRangeException(parameterName, id, "Open GoPro setting and status IDs must be in the range 0-255.");
}
