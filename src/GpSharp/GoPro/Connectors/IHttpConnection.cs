using System;
using System.Threading;
using System.Threading.Tasks;

namespace GpSharp.GoPro.Connectors;

/// <summary>
/// Interface for HTTP communication with a GoPro device.
/// </summary>
public interface IHttpConnection : IDisposable
{
    Task<string> GetAsync(string path);

    async Task<string> GetAsync(string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await GetAsync(path).ConfigureAwait(false);
    }

    Task<string> PostAsync(string path, object content);
    Task<string> PutAsync<T>(string path, T content);
}
