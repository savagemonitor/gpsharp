namespace GpSharp.Core;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Data;
using GpSharp.GoPro;
using GpSharp.Network;

/// <summary>
/// The main entry point for the GpSharp SDK.
/// </summary>
public class OgpSdk
{
    private readonly IDiscoveryService _discoveryService;
    private readonly Dictionary<GoProId, GoPro> _connectedGoPros = new();

    public OgpSdk(IDiscoveryService discoveryService)
    {
    _discoveryService = discoveryService;
    }

    /// <summary>
    /// Discovers available GoPros on the network.
    /// </summary>
    public async Task<IEnumerable<DiscoveredDevice>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
    return await _discoveryService.DiscoverAsync(cancellationToken);
    }

    /// <summary>
    /// Connects to a discovered GoPro device.
    /// </summary>
    /// <param name="device">The discovered device.</param>
    /// <returns>A connected GoPro instance.</returns>
    public async Task<GoPro> ConnectAsync(DiscoveredDevice device)
    {
    var baseUrl = $"http://{device.IpAddress}:{device.Port}";
    var connection = new HttpConnection(baseUrl);
    var goPro = new GoPro(connection);

    _connectedGoPros[device.Id] = goPro;
    return await Task.FromResult(goPro); // Wrapping in Task to maintain async signature if needed, though it's sync here
    }

    /// <summary>
    /// Gets a GoPro by its ID.
    /// </summary>
    /// <param name="id">The GoPro ID.</param>
    /// <returns>The GoPro device or null if not found.</returns>
    public GoPro? GetGoPro(GoProId id)
    {
    return _connectedGoPros.TryGetValue(id, out var goPro) ? goPro : null;
    }
}

public interface IDiscoveryService
{
    Task<IEnumerable<DiscoveredDevice>> DiscoverAsync(CancellationToken cancellationToken = default);
}
