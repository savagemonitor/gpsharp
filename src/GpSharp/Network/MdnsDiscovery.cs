using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Core;
using GpSharp.Data;
using Tmds.MDns;
using System.Collections.Concurrent;

namespace GpSharp.Network;

/// <summary>
/// Implementation of Network Discovery using mDNS via the tmds.mdns library.
/// </summary>
public class MdnsDiscovery : IDiscoveryService, IDisposable
{
    private const string UsbServiceType = "_workstation._tcp";

    private ConcurrentBag<DiscoveredDevice> _discoveredDevices = new();
    private ServiceBrowser _serviceBrowser;

    public MdnsDiscovery()
    {
        _serviceBrowser = new ServiceBrowser();
        _serviceBrowser.ServiceAdded += (s, e) =>
        {
            // When a new service is announced, we need to figure out if it's a camera or something else.
            // All GoPros seem to announce with a hostname that starts with "C" and is their serial number.
            // We also know that any GoPros connected over USB will have an IP address of "172.2X.1YZ.51" where XYZ are the last three digits of the serial number.
            // Thus we can see if the Hostname starts with "C" and if the IP address matches the expected pattern for a USB-connected GoPro.
            if(e.Announcement.Type == UsbServiceType &&
               e.Announcement.Hostname.StartsWith("C"))
            {
                string expectedIp = $"172.2{e.Announcement.Hostname[e.Announcement.Hostname.Length-3]}.1{e.Announcement.Hostname.Substring(e.Announcement.Hostname.Length-2, 2)}.51";
                if(e.Announcement.Addresses.Any(addr => addr.ToString() == expectedIp))
                {
                    _discoveredDevices.Add(new DiscoveredDevice(
                        new GoProId(e.Announcement.Hostname.Substring(e.Announcement.Hostname.Length-4, 4)),
                        e.Announcement?.Hostname ?? "Unknown",
                        e.Announcement?.Addresses.FirstOrDefault()?.ToString() ?? string.Empty,
                        e.Announcement?.Port ?? 0,
                        "Unknown", // MAC address not available via mDNS
                        "Unknown",  // Model not available via mDNS
                        ConnectionType.UniversalSerialBus
                    ));
                }
            }
        };
        _serviceBrowser.StartBrowse(UsbServiceType);

    }

    /// <summary>
    /// Scans the network for GoPros by browsing for mDNS service announcements.
    /// </summary>
    public async Task<IEnumerable<DiscoveredDevice>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        return _discoveredDevices.ToList();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && (_serviceBrowser?.IsBrowsing ?? false))
        {
            _serviceBrowser?.StopBrowse();
        }
    }
}
