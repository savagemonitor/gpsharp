namespace GpSharp.GoPro.Settings;

/// <summary>
/// Provides HTTP-backed access to supported camera setting identifiers.
/// </summary>
public interface ISettingsContainer
{
    ISetting VideoResolution { get; }
    ISetting VideoFrameRate { get; }

    /// <summary>
    /// Gets a setting by its non-negative Open GoPro protocol identifier.
    /// </summary>
    ISetting GetSetting(int settingId);
}