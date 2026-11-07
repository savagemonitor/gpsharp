using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GpSharp.Core;
using GpSharp.Data;
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
    public async Task<Result<Unit>> SetShutterAsync(bool on)
    {
        var action = on ? "start" : "stop";
        try
        {
            await _connection.GetAsync($"/gopro/camera/shutter/{action}");
            return Result<Unit>.Success(Unit.Value);
        }
        catch (HttpRequestException error)
        {
            return Result<Unit>.Failure(error);
        }
    }

    /// <summary>
    /// Gets the camera media list.
    /// </summary>
    public async Task<Result<MediaList>> GetMediaListAsync()
    {
        try
        {
            var response = await _connection.GetAsync("/gopro/media/list");
            var mediaList = JsonSerializer.Deserialize<MediaList>(response)
                ?? throw new JsonException("The GoPro media list response was empty.");
            return Result<MediaList>.Success(mediaList);
        }
        catch (HttpRequestException error)
        {
            return Result<MediaList>.Failure(error);
        }
        catch (JsonException error)
        {
            return Result<MediaList>.Failure(error);
        }
    }
}
