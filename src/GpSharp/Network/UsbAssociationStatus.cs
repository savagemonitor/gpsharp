namespace GpSharp.Network;

/// <summary>Result of associating a Windows USB network device with an mDNS hostname.</summary>
public enum UsbAssociationStatus
{
    Unavailable,
    NoMatch,
    Ambiguous,
    Matched,
    Error
}
