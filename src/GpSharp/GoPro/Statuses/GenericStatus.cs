using System;
using System.Threading.Tasks;

namespace GpSharp.GoPro.Statuses;

/// <summary>
/// A status placeholder for the BLE-only status operations in the Kotlin SDK.
/// </summary>
internal sealed class GenericStatus : IStatus
{
    private readonly string _key;

    public GenericStatus(string key)
    {
        _key = key;
    }

    public Task<T> GetValueAsync<T>() =>
        throw new NotSupportedException($"Status '{_key}' requires BLE support, which is not part of Phase 1.");
}
