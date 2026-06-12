using System;
using System.Threading.Tasks;

namespace GpSharp.Network;

/// <summary>
/// Interface for HTTP communication with a GoPro device.
/// </summary>
public interface IHttpConnection : IDisposable
{
    Task<string> GetAsync(string path);
    Task<string> PostAsync(string path, object content);
    Task<string> PutAsync<T>(string path, T content);
}