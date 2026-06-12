namespace GpSharp.Core;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GpSharp.Data;
using GpSharp.GoPro;

/// <summary>
/// The main entry point for the GpSharp SDK.
/// </summary>
public class OgpSdk
{
    private readonly IDiscoveryService _discoveryService;

    public OgpSdk(IDiscoveryService discoveryService)
    {
        _discoveryService = discoveryService;
    }

    /// <summary>
    /// Discovers available GoPros on the network.
    /// </summary>
    /// <returns>A list of discovered devices.</returns>
    public async Task<IEnumerable<DiscoveredDevice>> DiscoverAsync()
    {
        return await _discoveryService.DiscoverAsync();
    }

    /// <summary>
    /// Gets a GoPro by its ID.
    /// </summary>
    /// <param name="id">The GoPro ID.</param>
    /// <returns>The GoPro device or null if not found.</returns>
    public GoPro? GetGoPro(GoProId id)
    {
        // Implementation to be completed.
        return null;
    }
}

public interface IDiscoveryService
{
    Task<IEnumerable<DiscoveredDevice>> DiscoverAsync();
}
