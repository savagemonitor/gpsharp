namespace GpSharp.GoPro.Settings;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Core;
using GpSharp.GoPro.Connectors;

/// <summary>
/// A setting accessed through the Open GoPro HTTP camera-state and setting endpoints.
/// </summary>
internal sealed class GenericSetting : ISetting
{
    private readonly IHttpConnection _connection;

    public int Id { get; }

    internal GenericSetting(int id, IHttpConnection connection)
    {
        Id = id;
        _connection = connection;
    }

    public async Task<Result<T>> GetValueAsync<T>(CancellationToken cancellationToken = default)
    {
        try
        {
            var path = string.Format(CultureInfo.InvariantCulture, "/gopro/camera/setting?setting={0}", Id);
            var json = await _connection.GetAsync(path, cancellationToken).ConfigureAwait(false);
            if (json is null)
            {
                throw new JsonException($"Setting ID {Id} response was null.");
            }

            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object ||
                !document.RootElement.TryGetProperty("option", out var option))
            {
                throw new JsonException($"Setting ID {Id} response did not contain an 'option' value.");
            }

            var parsed = option.Deserialize<T>();
            return parsed is null
                ? Result<T>.Failure(new JsonException($"Setting ID {Id} returned a null option."))
                : Result<T>.Success(parsed);
        }
        catch (HttpRequestException error)
        {
            return Result<T>.Failure(error);
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

    public async Task<Result<Unit>> SetValueAsync<T>(T value, CancellationToken cancellationToken = default)
    {
        if (!TryGetOption(value, out var option, out var validationError))
        {
            return Result<Unit>.Failure(validationError!);
        }

        try
        {
            var path = string.Format(CultureInfo.InvariantCulture,
                "/gopro/camera/setting?setting={0}&option={1}", Id, option);
            await _connection.GetAsync(path, cancellationToken).ConfigureAwait(false);
            return Result<Unit>.Success(Unit.Value);
        }
        catch (HttpResponseException error) when (error.ResponseStatusCode == HttpStatusCode.Forbidden)
        {
            return Result<Unit>.Failure((Exception?)ParseSettingRejected(error) ?? error);
        }
        catch (HttpRequestException error)
        {
            return Result<Unit>.Failure(error);
        }
    }

    private SettingRejectedException? ParseSettingRejected(HttpResponseException error)
    {
        try
        {
            using var document = JsonDocument.Parse(error.ResponseBody);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("error", out var errorCode) ||
                !TryGetInt32(errorCode, out var parsedErrorCode) ||
                !root.TryGetProperty("setting_id", out var settingId) ||
                !TryGetInt32(settingId, out var parsedSettingId) ||
                !root.TryGetProperty("option_id", out var optionId) ||
                !TryGetInt32(optionId, out var parsedOptionId) ||
                !root.TryGetProperty("supported_options", out var options) ||
                options.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var supportedOptions = new List<SettingOption>();
            foreach (var item in options.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object ||
                    !item.TryGetProperty("id", out var id) ||
                    !TryGetInt32(id, out var parsedId) ||
                    !item.TryGetProperty("display_name", out var displayName) ||
                    displayName.ValueKind != JsonValueKind.String)
                {
                    return null;
                }

                supportedOptions.Add(new SettingOption(parsedId, displayName.GetString()!));
            }

            return parsedSettingId == Id
                ? new SettingRejectedException(parsedErrorCode, parsedSettingId, parsedOptionId, supportedOptions)
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool TryGetInt32(JsonElement value, out int result)
    {
        result = default;
        return value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out result);
    }

    private static bool TryGetOption<T>(T value, out long option, out Exception? error)
    {
        try
        {
            option = value switch
            {
                byte number => number,
                sbyte number => number,
                short number => number,
                ushort number => number,
                int number => number,
                uint number => number,
                long number => number,
                ulong number when number <= long.MaxValue => (long)number,
                Enum enumeration => Convert.ToInt64(enumeration, CultureInfo.InvariantCulture),
                JsonElement element when element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var number) => number,
                _ => -1
            };
        }
        catch (OverflowException exception)
        {
            option = -1;
            error = new ArgumentOutOfRangeException(
                nameof(value), value, $"The setting option exceeds the supported integer range: {exception.Message}");
            return false;
        }

        if (option < 0)
        {
            error = value is null || value is string || value is JsonElement { ValueKind: not JsonValueKind.Number }
                ? new ArgumentException("Setting options must be non-negative integer values.", nameof(value))
                : new ArgumentOutOfRangeException(nameof(value), "Setting options must be non-negative integers.");
            return false;
        }

        error = null;
        return true;
    }
}
