# API Overview

## Discover and connect

`OgpSdk.DiscoverAsync` streams devices announced by the Open GoPro mDNS service. Cancelling the
enumeration stops the read; dispose the discovery service to stop its background mDNS browse.

```csharp
using System;
using System.Threading;
using GpSharp.Core;
using GpSharp.GoPro;
using GpSharp.Network;

using var discovery = new MdnsDiscovery();
var sdk = new OgpSdk(discovery);
using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
GoPro? camera = null;
GoProId? cameraId = null;

await foreach (var device in sdk.DiscoverAsync(cancellation.Token))
{
    camera = await sdk.ConnectAsync(device);
    cameraId = device.Id;
    break;
}

if (camera is null)
{
    return;
}

Console.WriteLine($"Created camera client for {cameraId}");
var media = await camera.Commands.GetMediaListAsync();

if (!media.IsSuccess)
{
    Console.Error.WriteLine(media.Error);
}
```

`ConnectAsync` creates an HTTP-backed `GoPro` and stores it by its value-based `GoProId`. It does
not perform a connectivity check, Wi-Fi credential provisioning, or BLE pairing. Discovery uses
the camera's mDNS announcement; it is not guaranteed to find a camera attached via USB. A known
USB camera address can instead be supplied explicitly using `DiscoveredDevice` and `ConnectAsync`.
Each mDNS result represents one address on one receiving interface. `DiscoveryInterface`
records its interface ID, name, IPv4/IPv6 indices, OS interface type, local-link evidence,
USB association status, and any lookup diagnostic; `ServiceInstance` preserves the DNS-SD
instance name. Link-local IPv6 addresses include the interface scope when the OS supplies one;
`ConnectAsync` rejects an unscoped link-local address rather than choosing an interface.

`DiscoveryInterface.LocalLink` is evidence about the discovery interface, **not** the route
selected later by HTTP or the camera's Wi-Fi mode. `Wifi` means the receiving interface reports
802.11; it does not distinguish a camera access point from a shared LAN. On Windows,
`GoProUsbNetwork` is reported only when a physical, nonvirtual USB NCM adapter has a GoPro USB
parent whose full device instance identity uniquely matches the announced hostname. This is
an observed identity correlation, not an authenticated assertion; mDNS is untrusted. Detection
can be unavailable or fail, and other models and platforms can remain unknown. The
`UsbAssociation` and `Diagnostic` fields distinguish lookup outcomes. `ConnectionType` remains
`Unknown` for mDNS results: neither a USB match nor a host Wi-Fi interface proves the outgoing
HTTP route or AP/WLAN topology. A caller that knows the camera topology can provide it explicitly
when constructing `DiscoveredDevice`.

`ConnectAsync` uses the selected address but does not bind to the discovery interface. OS routing
can select another path, especially with multiple interfaces; the actual route is not reported.

## Supported commands

The Phase 1 HTTP command surface matches the Kotlin SDK operations available without BLE:

`GetMediaListAsync` returns a `MediaList` containing directories and their media files. Camera HTTP
and JSON errors are returned through `Result<T>`.

## Protobuf messages

The SDK exposes generated Open GoPro protobuf message types in `GpSharp.Protos.OpenGoPro`.
They cover camera name, last captured media, turbo transfer, COHN, and their shared responses.
They provide serialization only; the SDK does not yet implement the corresponding protobuf
command transport. The schemas and their source attribution are in `src\GpSharp\Protos\OpenGoPro`.

## Deferred containers

`Settings` and `Statuses` remain present as API placeholders, but the Kotlin SDK routes their
operations over BLE. Calls currently throw `NotSupportedException`; these operations are planned
for the BLE phase. The features container and BLE connection state are likewise not implemented in
Phase 1.
