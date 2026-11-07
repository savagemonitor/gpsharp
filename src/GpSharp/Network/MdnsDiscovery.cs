using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using GpSharp.Core;
using GpSharp.Data;
using Tmds.MDns;

namespace GpSharp.Network;

/// <summary>
/// Implementation of Network Discovery using mDNS via the tmds.mdns library.
/// </summary>
public class MdnsDiscovery : IDiscoveryService, IDisposable
{
    private const string GoProServiceType = "_gopro-web._tcp";

    private readonly Channel<DiscoveredDevice> _discoveredDevices = Channel.CreateUnbounded<DiscoveredDevice>();
    private readonly Dictionary<(string Host, string Instance, string InterfaceId, string IpAddress, int Port), DiscoveryInterfaceEvidence?> _seenDevices = new();
    private readonly object _gate = new();
    private readonly ServiceBrowser _serviceBrowser;

    public MdnsDiscovery() : this(startBrowse: true)
    {
    }

    internal MdnsDiscovery(bool startBrowse)
    {
        _serviceBrowser = new ServiceBrowser();
        _serviceBrowser.ServiceAdded += (s, e) => HandleAnnouncement(e.Announcement);
        _serviceBrowser.ServiceChanged += (s, e) => HandleAnnouncement(e.Announcement);
        _serviceBrowser.ServiceRemoved += (s, e) =>
        {
            var announcement = e.Announcement;
            lock (_gate)
            {
                foreach (var entry in _seenDevices.Keys.Where(entry =>
                    entry.Host == announcement.Hostname &&
                    entry.Instance == (announcement.Instance ?? string.Empty) &&
                    entry.InterfaceId == (announcement.NetworkInterface?.Id ?? string.Empty)).ToArray())
                {
                    _seenDevices.Remove(entry);
                }
            }
        };
        if (startBrowse)
        {
            _serviceBrowser.StartBrowse(GoProServiceType);
        }
    }

    private void HandleAnnouncement(ServiceAnnouncement announcement)
    {
        if (!string.Equals(
                announcement.Type.TrimEnd('.'),
                GoProServiceType,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var hostname = announcement.Hostname;
        var hostLabel = hostname.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        if (hostLabel is null || hostLabel.Length < 4)
        {
            return;
        }

        DiscoveryInterfaceEvidence? evidence;
        try
        {
            evidence = TransportEvidenceResolver.Resolve(announcement.NetworkInterface, hostLabel);
        }
        catch (NetworkInformationException error)
        {
            _discoveredDevices.Writer.TryComplete(error);
            return;
        }

        var interfaceId = evidence?.InterfaceId ?? string.Empty;
        var addresses = announcement.Addresses
            .Select(address => ScopeAddress(address, evidence?.IPv6Index))
            .Select(address => address.ToString())
            .ToHashSet(StringComparer.Ordinal);

        lock (_gate)
        {
            foreach (var old in _seenDevices.Keys.Where(entry =>
                entry.Host == hostname && entry.Instance == (announcement.Instance ?? string.Empty) &&
                entry.InterfaceId == interfaceId &&
                (entry.Port != announcement.Port || !addresses.Contains(entry.IpAddress))).ToArray())
            {
                _seenDevices.Remove(old);
            }

            foreach (var address in addresses)
            {
                var key = (hostname, announcement.Instance ?? string.Empty, interfaceId, address, (int)announcement.Port);
                if (_seenDevices.TryGetValue(key, out var previousEvidence) && Equals(previousEvidence, evidence))
                {
                    continue;
                }

                _seenDevices[key] = evidence;
                _discoveredDevices.Writer.TryWrite(new DiscoveredDevice(
                    new GoProId(hostLabel[^4..]),
                    hostname,
                    address,
                    announcement.Port,
                    "Unknown",
                    "Unknown",
                    ConnectionType.Unknown,
                    evidence,
                    announcement.Instance));
            }
        }
    }

    internal static IPAddress ScopeAddress(IPAddress address, int? interfaceIndex) =>
        address.AddressFamily == AddressFamily.InterNetworkV6 && address.IsIPv6LinkLocal &&
        interfaceIndex is > 0
            ? new IPAddress(address.GetAddressBytes(), interfaceIndex.Value)
            : address;

    /// <summary>
    /// Streams GoPros announced through the Open GoPro mDNS service.
    /// </summary>
    public async IAsyncEnumerable<DiscoveredDevice> DiscoverAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var device in _discoveredDevices.Reader.ReadAllAsync(cancellationToken))
        {
            yield return device;
        }
    }

    public void Dispose()
    {
        if (_serviceBrowser.IsBrowsing)
        {
            _serviceBrowser.StopBrowse();
        }

        _discoveredDevices.Writer.TryComplete();
    }
}
