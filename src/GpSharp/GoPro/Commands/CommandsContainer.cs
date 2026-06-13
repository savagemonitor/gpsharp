using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GpSharp.GoPro.Connectors;

namespace GpSharp.GoPro.Commands;

/// <summary>
/// Provides access to camera commands and operations via HTTP.
/// </summary>
public class CommandsContainer : ICommandsContainer
{
    private readonly IHttpConnection _connection;

    public CommandsContainer(IHttpConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Sets the shutter state (record on/off).
    /// </summary>
    /// <param name="on">True to start recording, false to stop.</param>
    public Task SetShutterAsync(bool on)
    {
        var payload = new { on = on };
        return _connection.PostAsync("/camera/shutter", payload)
            .ContinueWith(_ => Task.CompletedTask);
        // Note: In a real implementation, we'd parse the response or handle errors.
        // For now, we just ensure it performs the call.
    }

    /// <summary>
    /// Gets the list of media files on the camera.
    /// </summary>
    public async Task<IEnumerable<string>> GetMediaListAsync()
    {
        try
        {
            var response = await _connection.GetAsync("/camera/media_list");
            // Placeholder: In a real implementation, we would parse JSON list of files.
            return Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Captures a single photo.
    /// </summary>
    public async Task CapturePhotoAsync()
    {
        await _connection.PostAsync("/camera/capture_photo", new { });
    }

    /// <summary>
    /// Starts video recording.
    /// </summary>
    public async Task StartVideoAsync()
    {
        await _connection.PostAsync("/camera/start_video", new { });
    }

    /// <summary>
    /// Stops video recording.
    /// </summary>
    public async Task StopVideoAsync()
    {
        await _connection.PostAsync("/camera/stop_video", new { });
    }
}
