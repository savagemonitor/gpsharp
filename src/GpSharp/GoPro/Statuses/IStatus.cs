namespace GpSharp.GoPro.Statuses;

using System.Threading;
using System.Threading.Tasks;
using GpSharp.Core;

/// <summary>
/// Represents a status metric from the camera.
/// </summary>
public interface IStatus
{
    int Id { get; }
    Task<Result<T>> GetValueAsync<T>(CancellationToken cancellationToken = default);
}
