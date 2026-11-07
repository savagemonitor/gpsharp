namespace GpSharp.Core;

/// <summary>
/// Represents a successful operation that does not return a value.
/// </summary>
public sealed class Unit
{
    public static Unit Value { get; } = new();

    private Unit()
    {
    }
}
