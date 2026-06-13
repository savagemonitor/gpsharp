namespace GpSharp.Core;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Data;

public interface IDiscoveryService
{
    Task<IEnumerable<DiscoveredDevice>> DiscoverAsync(CancellationToken cancellationToken = default);
}
