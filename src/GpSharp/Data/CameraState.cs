namespace GpSharp.Data;

using System.Collections.Generic;
using System.Text.Json;

/// <summary>
/// Snapshot returned by the Open GoPro HTTP camera-state endpoint.
/// </summary>
public sealed class CameraState
{
    /// <summary>
    /// Gets the setting values keyed by their Open GoPro setting IDs.
    /// Values retain their original JSON types.
    /// </summary>
    public IReadOnlyDictionary<int, JsonElement> Settings { get; }

    /// <summary>
    /// Gets the status values keyed by their Open GoPro status IDs.
    /// Values retain their original JSON types.
    /// </summary>
    public IReadOnlyDictionary<int, JsonElement> Statuses { get; }

    internal CameraState(
        IReadOnlyDictionary<int, JsonElement> settings,
        IReadOnlyDictionary<int, JsonElement> statuses)
    {
        Settings = settings;
        Statuses = statuses;
    }
}
