namespace GpSharp.Data;

using System;
using GpSharp.Core;

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

    public DiscoveredDevice(GoProId id, string name, string ipAddress, int port, string macAddress, string model)
    {
        Id = id;
        Name = name;
        IpAddress = ipAddress;
        Port = port;
        MacAddress = macAddress;
        Model = model;
    }
}

/// <summary>
/// A data packet representing heartbeats or system updates from the camera.
/// </summary>
public class CameraHeartbeat
{
    public DateTime Timestamp { get; }
    public bool IsActive { get; }

    public CameraHeartbeat(DateTime timestamp, bool isActive)
    {
        Timestamp = timestamp;
        IsActive = isActive;
    }
}
