namespace GpSharp.Network;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GpSharp.Data;

/// <summary>
/// Interface for finding GoPros on the local network.
/// </summary>
public interface INetworkDiscovery
{
    /// <summary>
    /// Begins scanning and returns discovered devices.
    /// </summary>
    Task<IEnumerable<DiscoveredDevice>> DiscoverAsync();
}

/// <summary>
/// Implementation of Network Discovery using mDNS (Simplified for now).
/// </summary>
public class NetworkDiscovery : INetworkDiscovery
{
    public async Task<IEnumerable<DiscoveredDevice>> DiscoverAsync()
    {
        // This will be implemented with full mDNS support in a later phase or via a specific library.
        // Currently returns an empty list as a placeholder.
        return await Task.FromResult(Array.Empty<DiscoveredDevice>());
    }
}
