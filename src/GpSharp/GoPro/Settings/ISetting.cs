namespace GpSharp.GoPro.Settings;

using System.Threading;
using System.Threading.Tasks;
using GpSharp.Core;

/// <summary>
/// Reads or writes a setting using its Open GoPro protocol identifier.
/// </summary>
public interface ISetting
{
    int Id { get; }

    /// <summary>
    /// Reads the setting's current option value.
    /// </summary>
    Task<Result<T>> GetValueAsync<T>(CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes a non-negative integer option value. The camera may reject unsupported options.
    /// </summary>
    Task<Result<Unit>> SetValueAsync<T>(T value, CancellationToken cancellationToken = default);
}
