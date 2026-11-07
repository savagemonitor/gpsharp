using System;
using System.Net.NetworkInformation;

namespace GpSharp.Network;

internal static class TransportEvidenceResolver
{
    internal static LocalLinkType ClassifyLocalLink(NetworkInterfaceType type) =>
        type == NetworkInterfaceType.Wireless80211 ? LocalLinkType.Wifi : LocalLinkType.Unknown;

    internal static (UsbAssociationStatus Status, string? Diagnostic) ResolveUsbAssociation(
        bool isWindows, string? interfaceId, string hostname)
    {
        if (!isWindows || interfaceId is null)
        {
            return (UsbAssociationStatus.Unavailable, null);
        }

        if (!OperatingSystem.IsWindows())
        {
            return (UsbAssociationStatus.Unavailable, "Windows device metadata is unavailable on this platform.");
        }

        return WindowsUsbDeviceResolver.Resolve(interfaceId, hostname);
    }

    internal static DiscoveryInterfaceEvidence? Resolve(NetworkInterface? networkInterface, string hostname)
    {
        if (networkInterface is null)
        {
            return null;
        }

        var properties = networkInterface.GetIPProperties();
        var association = ResolveUsbAssociation(OperatingSystem.IsWindows(), networkInterface.Id, hostname);
        var localLink = association.Status == UsbAssociationStatus.Matched
            ? LocalLinkType.GoProUsbNetwork
            : ClassifyLocalLink(networkInterface.NetworkInterfaceType);
        return new DiscoveryInterfaceEvidence(
            networkInterface.Id,
            networkInterface.Name,
            properties.GetIPv4Properties()?.Index,
            properties.GetIPv6Properties()?.Index,
            networkInterface.NetworkInterfaceType,
            localLink,
            association.Status,
            association.Diagnostic);
    }
}
