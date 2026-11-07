namespace GpSharp.Core;

using System.Collections.Generic;
using System.Threading;
using GpSharp.Data;

public interface IDiscoveryService
{
    IAsyncEnumerable<DiscoveredDevice> DiscoverAsync(CancellationToken cancellationToken = default);
}
