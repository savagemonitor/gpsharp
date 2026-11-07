namespace GpSharp.GoPro.Settings;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Core;
using GpSharp.Data;
using GpSharp.GoPro.Connectors;

internal sealed class CameraStateClient
{
    private readonly IHttpConnection _connection;

    internal CameraStateClient(IHttpConnection connection)
    {
        _connection = connection;
    }

    internal async Task<Result<CameraState>> GetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var json = await _connection.GetAsync("/gopro/camera/state", cancellationToken).ConfigureAwait(false);
            if (json is null)
            {
                throw new JsonException("The camera state response was null.");
            }

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException("The camera state response must be a JSON object.");
            }

            var settings = ParseValues(root, "settings");
            var statuses = ParseValues(root, "status");
            return Result<CameraState>.Success(new CameraState(settings, statuses));
        }
        catch (HttpRequestException error)
        {
            return Result<CameraState>.Failure(error);
        }
        catch (JsonException error)
        {
            return Result<CameraState>.Failure(error);
        }
    }

    private static Dictionary<byte, JsonElement> ParseValues(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var values) || values.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException($"The camera state response must contain a '{propertyName}' object.");
        }

        var parsed = new Dictionary<byte, JsonElement>();
        foreach (var property in values.EnumerateObject())
        {
            if (!byte.TryParse(property.Name, NumberStyles.None, CultureInfo.InvariantCulture, out var id))
            {
                throw new JsonException($"The camera state contains an invalid {propertyName} ID '{property.Name}'.");
            }

            if (!parsed.TryAdd(id, property.Value.Clone()))
            {
                throw new JsonException($"The camera state contains duplicate {propertyName} ID '{property.Name}'.");
            }
        }

        return parsed;
    }
}
