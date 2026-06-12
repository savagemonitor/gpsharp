namespace GpSharp.Core;

using System;

/// <summary>
/// Represents a GoPro identification based on the last four characters of its serial number.
/// </summary>
public class GoProId
{
    public string Value { get; }

    public GoProId(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 4)
        {
            throw new ArgumentException("GoProId must be exactly 4 characters.", nameof(value));
        }
        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(GoProId id) => id.Value;
}
