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
    /// Gets the authoritative Open GoPro documentation URL for this identifier.
    /// </summary>
    public string DocumentationUrl { get; }

    /// <summary>
    /// Gets the pinned OpenGoPro Kotlin source URL when the SDK publishes this identifier there.
    /// </summary>
    public string? SdkSourceUrl { get; }

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
        IReadOnlyList<CameraProtocolOption>? options = null)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        ValueKind = valueKind;
        DocumentationUrl = documentationUrl ?? throw new ArgumentNullException(nameof(documentationUrl));
        SdkSourceUrl = sdkSourceUrl;
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
}
