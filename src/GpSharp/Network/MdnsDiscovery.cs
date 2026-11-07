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

    private readonly Dictionary<(string Host, string Instance, string InterfaceId, string IpAddress, int Port), DiscoveredDevice> _discoveredDevices = new();
    private readonly HashSet<Channel<DiscoveredDevice>> _subscribers = new();
    private readonly object _gate = new();
    private readonly ServiceBrowser _serviceBrowser;
    private bool _disposed;

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
            var interfaceId = announcement.NetworkInterface?.Id;
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }

                foreach (var entry in _discoveredDevices.Keys.Where(entry =>
                    entry.Host == announcement.Hostname &&
                    entry.Instance == (announcement.Instance ?? string.Empty) &&
                    (interfaceId is null || entry.InterfaceId == interfaceId)).ToArray())
                {
                    _discoveredDevices.Remove(entry);
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
            CompleteSubscribers(error);
            return;
        }

        var interfaceId = evidence?.InterfaceId ?? string.Empty;
        var addresses = announcement.Addresses
            .Select(address => ScopeAddress(address, evidence?.IPv6Index))
            .Select(address => address.ToString())
            .ToHashSet(StringComparer.Ordinal);

        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            foreach (var old in _discoveredDevices.Keys.Where(entry =>
                entry.Host == hostname && entry.Instance == (announcement.Instance ?? string.Empty) &&
                entry.InterfaceId == interfaceId &&
                (entry.Port != announcement.Port || !addresses.Contains(entry.IpAddress))).ToArray())
            {
                _discoveredDevices.Remove(old);
            }

            foreach (var address in addresses)
            {
                var key = (hostname, announcement.Instance ?? string.Empty, interfaceId, address, (int)announcement.Port);
                var device = new DiscoveredDevice(
                    new GoProId(hostLabel[^4..]),
                    hostname,
                    address,
                    announcement.Port,
                    "Unknown",
                    "Unknown",
                    ConnectionType.Unknown,
                    evidence,
                    announcement.Instance);
                if (_discoveredDevices.TryGetValue(key, out var previous) &&
                    Equals(previous.DiscoveryInterface, evidence))
                {
                    continue;
                }

                _discoveredDevices[key] = device;
                foreach (var subscriber in _subscribers)
                {
                    subscriber.Writer.TryWrite(device);
                }
            }
        }
    }

    private void CompleteSubscribers(Exception error)
    {
        lock (_gate)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Writer.TryComplete(error);
            }

            _subscribers.Clear();
        }
    }

    internal static IPAddress ScopeAddress(IPAddress address, int? interfaceIndex) =>
        address.AddressFamily == AddressFamily.InterNetworkV6 && address.IsIPv6LinkLocal &&
        interfaceIndex is > 0
            ? new IPAddress(address.GetAddressBytes(), interfaceIndex.Value)
            : address;

    /// <summary>
    /// Streams GoPros announced through the Open GoPro mDNS service. Each enumeration receives
    /// its own stream, beginning with the currently cached endpoints before live announcements.
    /// </summary>
    public async IAsyncEnumerable<DiscoveredDevice> DiscoverAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<DiscoveredDevice>();
        lock (_gate)
        {
            foreach (var device in _discoveredDevices.Values)
            {
                channel.Writer.TryWrite(device);
            }

            if (_disposed)
            {
                channel.Writer.TryComplete();
            }
            else
            {
                _subscribers.Add(channel);
            }
        }

        try
        {
            await foreach (var device in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return device;
            }
        }
        finally
        {
            lock (_gate)
            {
                _subscribers.Remove(channel);
            }

            channel.Writer.TryComplete();
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            foreach (var subscriber in _subscribers)
            {
                subscriber.Writer.TryComplete();
            }

            _subscribers.Clear();
        }

        if (_serviceBrowser.IsBrowsing)
        {
            _serviceBrowser.StopBrowse();
        }
    }
}
