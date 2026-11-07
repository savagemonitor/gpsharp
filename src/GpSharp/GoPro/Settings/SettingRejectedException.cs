namespace GpSharp.GoPro.Settings;

using System;
using System.Collections.Generic;

/// <summary>
/// The camera rejected a setting option and supplied the currently supported options.
/// </summary>
public sealed class SettingRejectedException : Exception
{
    public int ErrorCode { get; }
    public int SettingId { get; }
    public int OptionId { get; }
    public IReadOnlyList<SettingOption> SupportedOptions { get; }

    internal SettingRejectedException(
        int errorCode,
        int settingId,
        int optionId,
        IReadOnlyList<SettingOption> supportedOptions)
        : base($"The camera rejected option {optionId} for setting {settingId} with error {errorCode}.")
    {
        ErrorCode = errorCode;
        SettingId = settingId;
        OptionId = optionId;
        SupportedOptions = supportedOptions;
    }
}
