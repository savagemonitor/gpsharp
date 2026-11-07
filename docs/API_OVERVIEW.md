# API Overview

## Discover and connect

`OgpSdk.DiscoverAsync` streams devices announced by the Open GoPro mDNS service. Cancelling the
enumeration stops the read; dispose the discovery service to stop its background mDNS browse.
Each enumeration has an independent stream, starts by replaying the currently known endpoints, and
then receives new or changed announcements. When mDNS reports a service removal, that endpoint is
removed from the replay cache; a later announcement makes it discoverable again.

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

## Settings and statuses

The settings container supports direct reads and writes through Open GoPro HTTP. The named
`VideoResolution` and `VideoFrameRate` settings use protocol IDs 2 and 3. Other non-negative
integer IDs can be accessed with `camera.Settings.GetSetting(id)`. For example:

```csharp
var resolution = await camera.Settings.VideoResolution.GetValueAsync<int>();
if (resolution.IsSuccess)
{
    Console.WriteLine($"Resolution option: {resolution.Value}");
}

// Setting writes change camera configuration; the camera validates the option.
var update = await camera.Settings.VideoResolution.SetValueAsync(9);
```

Writes accept non-negative integer values or enums and return a failed `Result<Unit>` for
validation, transport, or camera rejection. A rejected HTTP 403 can include
`SettingRejectedException.SupportedOptions`. This metadata is available only when the camera
includes it in the rejection response; GpSharp does not query capabilities proactively.

`camera.Statuses.GetStatus(id).GetValueAsync<T>()` reads from a fresh camera-state response. Named
statuses are `IsEncoding` (ID 10) and `InternalBatteryPercentage` (ID 70).
`GetCameraStateAsync()` returns the full `CameraState` snapshot, with setting and status values
preserved as `JsonElement` and keyed by non-negative integer protocol ID (including IDs above 255).
Cancellation is propagated; missing IDs,
invalid JSON, unsupported conversions, and HTTP failures are returned as failed results.

`OpenGoProProtocolCatalog.GetSetting(id)` and `GetStatus(id)` provide the documented canonical
name, description, value kind, source URLs, and static enum options where published. They return
`null` for IDs absent from the catalog. For numeric enum values,
`metadata.FindOption(rawJsonValue)` returns the canonical option name or `null` for an unmapped
value; it does not alter the `JsonElement` stored in `CameraState`. Static enum options describe
the published superset, not the capabilities of a particular camera or current mode, so camera
validation remains authoritative. The catalog cites the current [Open GoPro setting
documentation](https://gopro.github.io/OpenGoPro/docs/ble/settings) and [status
documentation](https://gopro.github.io/OpenGoPro/docs/ble/statuses), with pinned Kotlin SDK source
links for identifiers/options present in that SDK revision
([settings](https://github.com/gopro/OpenGoPro/blob/0f963572611c4410a15678531e9681a6ff874edb/demos/kotlin/kmp_sdk/wsdk/src/commonMain/kotlin/com/gopro/open_gopro/entity/queries/Settings.kt),
[statuses](https://github.com/gopro/OpenGoPro/blob/0f963572611c4410a15678531e9681a6ff874edb/demos/kotlin/kmp_sdk/wsdk/src/commonMain/kotlin/com/gopro/open_gopro/entity/queries/Statuses.kt)).

Against the sanitized MISSION 1 PRO identifier lists, the public catalog maps 33 of 96 setting
IDs and 70 of 78 status IDs. The remaining identifiers are deliberately uncatalogued rather than
assigned guessed names: settings `6, 13, 41, 42, 47, 54, 62, 64, 84, 86, 103, 105, 111, 112,
115, 116, 117, 118, 126, 129-132, 139, 145-147, 153, 154, 159, 198, 199, 203, 205, 206,
208-214, 218, 221, 222, 226, 258-272, 274, 275`; statuses `3, 4, 14, 61, 90, 91, 109, 119`.
These gaps mean only that those IDs were not found in the cited public catalog, not that the
camera itself rejects or does not implement them. Current Open GoPro docs include settings
239, 256, 257, and 273, and status 123, even though those IDs are not all present in the pinned
Kotlin `UByte` ID enums.

These are request/response HTTP operations, not Kotlin WSDK BLE parity. BLE-backed settings/status
semantics, capability discovery and update streams remain later-phase work. `Features` and live
connection state are also not implemented in Phase 1.
