namespace GpSharp.GoPro.Status;

using System;
using System.Threading.Tasks;

/// <summary>
/// Represents a status metric from the camera.
/// </summary>
public interface IStatus
{
    object Value { get; }
    Task<T> GetValueAsync<T>();
}
