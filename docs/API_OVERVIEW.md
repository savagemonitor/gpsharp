# API Overview

## Getting Started
To begin using GpSharp, instantiate the `OgpSdk` with a discovery service. The SDK provides a high-level interface for interacting with GoPro devices over WiFi and Bluetooth.

```csharp
var sdk = new OgpSdk(new NetworkDiscovery());
var devices = await sdk.DiscoverAsync();

if (devices.Any())
{
    var camera = sdk.GetGoPro(devices.First().Id);
}
```

## Key Components
- **OgpSdk**: Entry point for the entire library.
- **GoPro**: Primary object representing a connected camera. Provides access to `Commands`, `Settings`, and `Statuses`.
- **NetworkDiscovery**: Helper to find devices on the local network via mDNS.

## HTTP Protocol Details
Currently, GpSharp supports interaction over standard HTTP protocols for WiFi connections. All commands are executed asynchronously.

## Error Handling
The SDK uses a result pattern or exceptions based on severity:
1.  **Handled Errors**: Return `Result<T>` objects for expected logic failures (e.g., device not found).
2.  **System Exceptions**: Critical failures like connection drops will throw standard .NET exceptions.

## Examples
### Setting a Property
```csharp
var settings = camera.Settings;
await settings.VideoResolution.SetValueAsync(Resolution.4K);
```

### Reading Statuses
```csharp
var status = await camera.Status.GetBatteryAsync();
Console.WriteLine($"Battery: {status}%");
```
