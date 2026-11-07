using System.Net.NetworkInformation;

namespace GpSharp.Network;

/// <summary>Describes the interface receiving an mDNS announcement, not the HTTP route.</summary>
public sealed record DiscoveryInterfaceEvidence(
    string InterfaceId,
    string InterfaceName,
    int? IPv4Index,
    int? IPv6Index,
    NetworkInterfaceType InterfaceType,
    LocalLinkType LocalLink,
    UsbAssociationStatus UsbAssociation,
    string? Diagnostic);
