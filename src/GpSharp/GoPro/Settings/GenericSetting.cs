namespace GpSharp.GoPro.Settings;

using System;
using System.Threading.Tasks;

/// <summary>
/// A setting placeholder for the BLE-only setting operations in the Kotlin SDK.
/// </summary>
internal class GenericSetting : ISetting
{
    private readonly string _key;

    public GenericSetting(string key)
    {
        _key = key;
    }

    public Task<T> GetValueAsync<T>() =>
        throw new NotSupportedException($"Setting '{_key}' requires BLE support, which is not part of Phase 1.");

    public Task SetValueAsync<T>(T value) =>
        throw new NotSupportedException($"Setting '{_key}' requires BLE support, which is not part of Phase 1.");
}
