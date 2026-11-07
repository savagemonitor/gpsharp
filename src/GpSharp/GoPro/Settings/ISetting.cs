namespace GpSharp.GoPro.Settings;

using System;
using System.Threading.Tasks;

public interface ISetting
{
    Task<T> GetValueAsync<T>();
    Task SetValueAsync<T>(T value);
}
