namespace GpSharp.GoPro.Commands;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    Task SetShutterAsync(bool on);

    /// <summary>
    /// Gets the list of media files on the camera.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, yielding file paths.</returns>
    Task<IEnumerable<string>> GetMediaListAsync();

    /// <summary>
    /// Captures a single photo.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CapturePhotoAsync();

    /// <summary>
    /// Starts video recording.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StartVideoAsync();

    /// <summary>
    /// Stops video recording.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StopVideoAsync();
}
