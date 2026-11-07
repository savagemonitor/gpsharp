namespace GpSharp.GoPro.Settings;

/// <summary>
/// Provides HTTP-backed access to supported camera setting identifiers.
/// </summary>
public interface ISettingsContainer
{
    ISetting VideoResolution { get; }
    ISetting VideoFrameRate { get; }

    /// <summary>
    /// Gets a setting by its Open GoPro protocol identifier, which must be in the range 0 through 255.
    /// </summary>
    ISetting GetSetting(int settingId);
}