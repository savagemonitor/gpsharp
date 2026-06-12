namespace GpSharp.GoPro.Settings;

using System;
using System.Threading.Tasks;

public interface ISetting
{
    object Value { get; }
    Task<T> GetValueAsync<T>();
    Task SetValueAsync<T>(T value);
}
