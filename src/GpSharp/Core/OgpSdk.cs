namespace GpSharp.Core;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Data;
using GpSharp.GoPro;
using GpSharp.GoPro.Connectors;

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
    public IAsyncEnumerable<DiscoveredDevice> DiscoverAsync(CancellationToken cancellationToken = default) =>
        _discoveryService.DiscoverAsync(cancellationToken);

    /// <summary>
    /// Connects to a discovered GoPro device.
    /// </summary>
    /// <param name="device">The discovered device.</param>
    /// <returns>A connected GoPro instance.</returns>
    public Task<GoPro> ConnectAsync(DiscoveredDevice device)
    {
        if (IPAddress.TryParse(device.IpAddress, out var address) &&
            address.AddressFamily == AddressFamily.InterNetworkV6 && address.IsIPv6LinkLocal && address.ScopeId == 0)
        {
            throw new ArgumentException("A link-local IPv6 camera address requires an interface scope.", nameof(device));
        }

        var baseUrl = new UriBuilder(Uri.UriSchemeHttp, device.IpAddress, device.Port).Uri.OriginalString;
        var connection = new HttpConnection(baseUrl);
        var goPro = new GoPro(connection);

        _connectedGoPros[device.Id] = goPro;
        return Task.FromResult(goPro);
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

// IDiscoveryService moved to GpSharp.Interfaces\n}
