namespace GpSharp.GoPro.Statuses;

using System.Threading;
using System.Threading.Tasks;
using GpSharp.Core;
using GpSharp.Data;

/// <summary>
/// Provides HTTP-backed reads of camera statuses and a complete state snapshot.
/// </summary>
public interface IStatusesContainer
{
    IStatus InternalBatteryPercentage { get; }
    IStatus IsEncoding { get; }

    /// <summary>
    /// Gets a status by its Open GoPro protocol identifier, which must be in the range 0 through 255.
    /// </summary>
    IStatus GetStatus(int statusId);

    /// <summary>
    /// Reads all status and setting values returned by the camera-state endpoint.
    /// </summary>
    Task<Result<CameraState>> GetCameraStateAsync(CancellationToken cancellationToken = default);
}
