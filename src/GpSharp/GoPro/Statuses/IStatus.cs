namespace GpSharp.GoPro.Statuses;

using System;
using System.Threading.Tasks;

/// <summary>
/// Represents a status metric from the camera.
/// </summary>
public interface IStatus
{
    Task<T> GetValueAsync<T>();
}
