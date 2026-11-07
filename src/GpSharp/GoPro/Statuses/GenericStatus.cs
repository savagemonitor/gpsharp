using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using GpSharp.Core;
using GpSharp.GoPro.Settings;

namespace GpSharp.GoPro.Statuses;

/// <summary>
/// A status read from the Open GoPro HTTP camera-state endpoint.
/// </summary>
internal sealed class GenericStatus : IStatus
{
    private readonly CameraStateClient _stateClient;

    public int Id { get; }

    internal GenericStatus(int id, CameraStateClient stateClient)
    {
        Id = id;
        _stateClient = stateClient;
    }

    public async Task<Result<T>> GetValueAsync<T>(CancellationToken cancellationToken = default)
    {
        var stateResult = await _stateClient.GetAsync(cancellationToken).ConfigureAwait(false);
        if (!stateResult.IsSuccess)
        {
            return Result<T>.Failure(stateResult.Error!);
        }

        if (!stateResult.Value!.Statuses.TryGetValue(Id, out var value))
        {
            return Result<T>.Failure(new KeyNotFoundException($"Status ID {Id} was not returned by the camera."));
        }

        try
        {
            var parsed = value.Deserialize<T>();
            if (parsed is null)
            {
                return Result<T>.Failure(new JsonException($"Status ID {Id} returned a null value."));
            }

            return Result<T>.Success(parsed);
        }
        catch (JsonException error)
        {
            return Result<T>.Failure(error);
        }
        catch (NotSupportedException error)
        {
            return Result<T>.Failure(error);
        }
    }
}
