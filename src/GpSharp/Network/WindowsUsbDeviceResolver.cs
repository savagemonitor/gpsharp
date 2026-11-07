using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace GpSharp.Network;

[SupportedOSPlatform("windows")]
internal static class WindowsUsbDeviceResolver
{
    internal static (UsbAssociationStatus Status, string? Diagnostic) Resolve(string interfaceId, string hostname)
    {
        if (!Guid.TryParse(interfaceId, out var expectedGuid))
        {
            return (UsbAssociationStatus.Error, "The discovery interface ID is not a Windows interface GUID.");
        }

        try
        {
            using var searcher = new ManagementObjectSearcher(
                @"root\StandardCimv2",
                "SELECT InterfaceGuid, PnPDeviceID, HardwareInterface, Virtual FROM MSFT_NetAdapter");
            searcher.Options.Timeout = TimeSpan.FromSeconds(3);
            using var results = searcher.Get();
            var matches = new List<Guid>();
            foreach (ManagementObject adapter in results)
            {
                using (adapter)
                {
                    if (!Guid.TryParse(adapter["InterfaceGuid"] as string, out var guid) ||
                        adapter["PnPDeviceID"] is not string child ||
                        adapter["HardwareInterface"] is not bool hardware ||
                        adapter["Virtual"] is not bool virtualAdapter ||
                        !hardware || virtualAdapter ||
                        !child.StartsWith(@"USB\VID_2672&PID_", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var (service, parent) = GetDeviceProperties(child);
                    if (WindowsUsbAssociation.Matches(hardware, virtualAdapter, service, child, parent, hostname))
                    {
                        matches.Add(guid);
                    }
                }
            }

            var status = WindowsUsbAssociation.Select(matches, expectedGuid);
            return (status, status == UsbAssociationStatus.Ambiguous
                ? "Multiple USB network interfaces match this hostname."
                : null);
        }
        catch (Exception error) when (error is ManagementException or UnauthorizedAccessException or COMException or InvalidOperationException)
        {
            return (UsbAssociationStatus.Error, $"Windows network device lookup failed: {error.Message}");
        }
    }

    private static (string Service, string Parent) GetDeviceProperties(string child)
    {
        Check(CM_Locate_DevNode(out var device, child, 0), "locating USB network device");
        var serviceBuffer = new byte[1024];
        uint size = (uint)serviceBuffer.Length;
        Check(CM_Get_DevNode_Registry_Property(device, 5, out var type, serviceBuffer, ref size, 0), "reading USB network service");
        if (type != 1 || size < 2 || size > serviceBuffer.Length)
        {
            throw new InvalidOperationException("Windows returned an invalid USB network service property.");
        }

        var service = Encoding.Unicode.GetString(serviceBuffer, 0, (int)size).TrimEnd('\0');
        Check(CM_Get_Parent(out var parent, device, 0), "locating USB device parent");
        Check(CM_Get_Device_ID_Size(out var length, parent, 0), "reading USB parent identity length");
        var parentId = new StringBuilder(checked((int)length + 1));
        Check(CM_Get_Device_ID(parent, parentId, (uint)parentId.Capacity, 0), "reading USB parent identity");
        return (service, parentId.ToString());
    }

    private static void Check(uint result, string operation)
    {
        if (result != 0)
        {
            throw new InvalidOperationException($"Configuration Manager error 0x{result:X} while {operation}.");
        }
    }

    [DllImport("cfgmgr32.dll", EntryPoint = "CM_Locate_DevNodeW", CharSet = CharSet.Unicode)]
    private static extern uint CM_Locate_DevNode(out uint device, string deviceId, uint flags);

    [DllImport("cfgmgr32.dll", EntryPoint = "CM_Get_DevNode_Registry_PropertyW")]
    private static extern uint CM_Get_DevNode_Registry_Property(uint device, uint property, out uint type, byte[] buffer, ref uint length, uint flags);

    [DllImport("cfgmgr32.dll")]
    private static extern uint CM_Get_Parent(out uint parent, uint device, uint flags);

    [DllImport("cfgmgr32.dll")]
    private static extern uint CM_Get_Device_ID_Size(out uint length, uint device, uint flags);

    [DllImport("cfgmgr32.dll", EntryPoint = "CM_Get_Device_IDW", CharSet = CharSet.Unicode)]
    private static extern uint CM_Get_Device_ID(uint device, StringBuilder buffer, uint length, uint flags);
}
