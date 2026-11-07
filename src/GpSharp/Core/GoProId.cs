namespace GpSharp.Core;

using System;

/// <summary>
/// Represents a GoPro identification based on the last four characters of its serial number.
/// </summary>
public sealed record GoProId
{
    public string PartialSerial { get; }

    public GoProId(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 4)
        {
            throw new ArgumentException("GoProId must be exactly 4 characters.", nameof(value));
        }
        PartialSerial = value;
    }

    public override string ToString() => PartialSerial;

    public static implicit operator string(GoProId id) => id.PartialSerial;
}
