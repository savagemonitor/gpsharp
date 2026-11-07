namespace GpSharp.Data;

using System;
using GpSharp.Core;
using GpSharp.Network;

/// <summary>
/// Represents a device discovered on the network.
/// </summary>
public class DiscoveredDevice
{
    public GoProId Id { get; }
    public string Name { get; }
    public string IpAddress { get; }
    public int Port { get; }
    public string MacAddress { get; }
    public string Model { get; }

    public ConnectionType ConnectionType { get; }
    public DiscoveryInterfaceEvidence? DiscoveryInterface { get; }
    public string? ServiceInstance { get; }

    public DiscoveredDevice(GoProId id, string name, string ipAddress, int port, string macAddress, string model, ConnectionType connectionType = ConnectionType.Unknown, DiscoveryInterfaceEvidence? discoveryInterface = null, string? serviceInstance = null)
    {
        Id = id;
        Name = name;
        IpAddress = ipAddress;
        Port = port;
        MacAddress = macAddress;
        Model = model;
        ConnectionType = connectionType;
        DiscoveryInterface = discoveryInterface;
        ServiceInstance = serviceInstance;
    }
}
