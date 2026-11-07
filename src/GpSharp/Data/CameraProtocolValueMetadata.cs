namespace GpSharp.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

/// <summary>
/// Describes a camera protocol setting or status without changing its raw state value.
/// </summary>
public sealed class CameraProtocolValueMetadata
{
    /// <summary>
    /// Gets the numeric protocol identifier.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the canonical human-readable name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a protocol description when one is published.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the documented wire value kind when known.
    /// </summary>
    public string? ValueKind { get; }

    /// <summary>
    /// Gets the CLR type used to decode this value when its protocol type is documented.
    /// Enum values decode to <see cref="CameraProtocolOption"/>; the raw JSON remains unchanged.
    /// </summary>
    public Type? ClrType { get; }

    /// <summary>
    /// Gets the authoritative Open GoPro documentation URL for this identifier.
    /// </summary>
    public string DocumentationUrl { get; }

    /// <summary>
    /// Gets the pinned OpenGoPro Kotlin source URL when the SDK publishes this identifier there.
    /// </summary>
    public string? SdkSourceUrl { get; }

    /// <summary>
    /// Gets the pinned OpenGoPro Python parser source for the documented wire value type.
    /// </summary>
    public string? TypeSourceUrl { get; }

    /// <summary>
    /// Gets documented enum options. The camera's original value remains available in CameraState.
    /// </summary>
    public IReadOnlyList<CameraProtocolOption> Options { get; }

    internal CameraProtocolValueMetadata(
        int id,
        string name,
        string? description,
        string? valueKind,
        string documentationUrl,
        string? sdkSourceUrl = null,
        string? typeSourceUrl = null,
        IReadOnlyList<CameraProtocolOption>? options = null)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        ValueKind = valueKind;
        ClrType = GetClrType(valueKind);
        DocumentationUrl = documentationUrl ?? throw new ArgumentNullException(nameof(documentationUrl));
        SdkSourceUrl = sdkSourceUrl;
        TypeSourceUrl = typeSourceUrl;
        Options = options ?? Array.Empty<CameraProtocolOption>();
    }

    /// <summary>
    /// Finds the documented enum option corresponding to a raw JSON integer, if one is defined.
    /// </summary>
    public CameraProtocolOption? FindOption(JsonElement rawValue)
    {
        if (rawValue.ValueKind != JsonValueKind.Number || !rawValue.TryGetInt64(out var value))
        {
            return null;
        }

        return Options.FirstOrDefault(option => option.Value == value);
    }

    /// <summary>
    /// Tries to decode a raw JSON value to the documented CLR type. Unknown IDs, unknown enum
    /// options, and values with a mismatched JSON shape return false without changing the raw value.
    /// </summary>
    public bool TryGetTypedValue(JsonElement rawValue, out object? value)
    {
        value = null;
        if (ClrType == typeof(bool) && rawValue.ValueKind == JsonValueKind.True)
        {
            value = true;
            return true;
        }

        if (ClrType == typeof(bool) && rawValue.ValueKind == JsonValueKind.False)
        {
            value = false;
            return true;
        }

        if (ClrType == typeof(bool) && rawValue.ValueKind == JsonValueKind.Number &&
            rawValue.TryGetByte(out var booleanValue) && booleanValue <= 1)
        {
            value = booleanValue == 1;
            return true;
        }

        if (ClrType == typeof(byte) && rawValue.ValueKind == JsonValueKind.Number &&
            rawValue.TryGetByte(out var byteValue))
        {
            value = byteValue;
            return true;
        }

        if (ClrType == typeof(uint) && rawValue.ValueKind == JsonValueKind.Number &&
            rawValue.TryGetUInt32(out var uintValue))
        {
            value = uintValue;
            return true;
        }

        if (ClrType == typeof(ulong) && rawValue.ValueKind == JsonValueKind.Number &&
            rawValue.TryGetUInt64(out var ulongValue))
        {
            value = ulongValue;
            return true;
        }

        if (ClrType == typeof(string) && rawValue.ValueKind == JsonValueKind.String)
        {
            value = rawValue.GetString();
            return value is not null;
        }

        if (ClrType == typeof(CameraProtocolOption))
        {
            value = FindOption(rawValue);
            return value is not null;
        }

        return false;
    }

    private static Type? GetClrType(string? valueKind)
    {
        if (valueKind is null)
        {
            return null;
        }

        return valueKind switch
        {
            "Boolean" => typeof(bool),
            "Unsigned 8-bit integer" => typeof(byte),
            "Unsigned 32-bit integer" => typeof(uint),
            "Unsigned 64-bit integer" => typeof(ulong),
            "String" or "UTF-8 string" => typeof(string),
            "Enum" or "Bitfield enum" => typeof(CameraProtocolOption),
            _ when valueKind.StartsWith("Integer ", StringComparison.Ordinal) => typeof(int),
            _ => null,
        };
    }
}
