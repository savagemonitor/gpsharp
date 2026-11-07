using System;
using System.Collections.Generic;
using System.Linq;

namespace GpSharp.Network;

internal static class WindowsUsbAssociation
{
    internal static bool Matches(bool hardware, bool virtualAdapter, string? service, string? childId, string? parentId, string hostname)
    {
        if (!hardware || virtualAdapter || !string.Equals(service, "UsbNcm", StringComparison.OrdinalIgnoreCase) ||
            childId is null || parentId is null || hostname.Length < 8)
        {
            return false;
        }

        var child = childId.Split('\\');
        var parent = parentId.Split('\\');
        if (child.Length != 3 || parent.Length != 3 ||
            !string.Equals(child[0], "USB", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(parent[0], "USB", StringComparison.OrdinalIgnoreCase) ||
            !parent[1].StartsWith("VID_2672&PID_", StringComparison.OrdinalIgnoreCase) ||
            parent[1].Length != "VID_2672&PID_005A".Length ||
            !child[1].StartsWith(parent[1] + "&MI_", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return string.Equals(parent[2], hostname, StringComparison.OrdinalIgnoreCase);
    }

    internal static UsbAssociationStatus Select(IEnumerable<Guid> matchingInterfaces, Guid expectedInterface)
    {
        var matches = matchingInterfaces.ToArray();
        return matches.Length switch
        {
            > 1 => UsbAssociationStatus.Ambiguous,
            1 when matches[0] == expectedInterface => UsbAssociationStatus.Matched,
            _ => UsbAssociationStatus.NoMatch
        };
    }
}
