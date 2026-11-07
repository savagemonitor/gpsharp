namespace GpSharp.GoPro.Commands;

using System.Threading.Tasks;
using GpSharp.Core;
using GpSharp.Data;

/// <summary>
/// Interface for accessing camera commands.
/// </summary>
public interface ICommandsContainer
{
    /// <summary>
    /// Sets the shutter state (record on/off).
    /// </summary>
    /// <param name="on">True to start recording, false to stop.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result<Unit>> SetShutterAsync(bool on);

    /// <summary>
    /// Gets the camera media list.
    /// </summary>
    /// <returns>A task representing the asynchronous operation and its parsed media list.</returns>
    Task<Result<MediaList>> GetMediaListAsync();
}
