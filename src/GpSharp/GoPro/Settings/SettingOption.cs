namespace GpSharp.GoPro.Settings;

/// <summary>
/// A setting option reported by the camera when a requested option is rejected.
/// </summary>
public sealed record SettingOption(int Id, string DisplayName);
