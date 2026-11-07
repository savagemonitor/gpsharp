using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GpSharp.Data;

/// <summary>
/// Media list returned by the Open GoPro HTTP API.
/// </summary>
public sealed class MediaList
{
    [JsonPropertyName("media")]
    public IReadOnlyList<MediaDirectory> Media { get; init; } = [];

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; init; }
}

/// <summary>
/// A directory containing camera media files.
/// </summary>
public sealed class MediaDirectory
{
    [JsonPropertyName("d")]
    public string Directory { get; init; } = string.Empty;

    [JsonPropertyName("fs")]
    public IReadOnlyList<MediaFile> Files { get; init; } = [];

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; init; }
}

/// <summary>
/// A media file entry returned by the camera.
/// </summary>
public sealed class MediaFile
{
    [JsonPropertyName("n")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("cre")]
    public string? CreatedAt { get; init; }

    [JsonPropertyName("mod")]
    public string? ModifiedAt { get; init; }

    [JsonPropertyName("s")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long Size { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; init; }
}
