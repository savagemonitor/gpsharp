# Kotlin SDK Parity: Phase 1

The Kotlin reference is `gopro/OpenGoPro/demos/kotlin/kmp_sdk/wsdk` in the OpenGoPro repository.
`savagemonitor/ogpmcpsharp` is a separate .NET MCP server and is not the Kotlin SDK.

The Kotlin SDK does not label a subset as "Phase 1", so GpSharp applies the repository's existing
HTTP-first Phase 1 boundary and compares only operations with Kotlin HTTP implementations.

| Kotlin API/protocol | GpSharp mapping | Status |
|---|---|---|
| `discover(NetworkType.WIFI_WLAN)` returns a `Flow` | mDNS `_gopro-web._tcp.` returns `IAsyncEnumerable` | Implemented |
| `setShutter(true/false)` | `SetShutterAsync` sends GET to `/gopro/camera/shutter/start` or `/stop` | Implemented |
| `getMediaList()` | `GetMediaListAsync` GETs `/gopro/media/list` | Implemented |
| `GoProId` | Value-based C# record key | Implemented |
| Setting operations and status streams | Kotlin explicitly selects BLE | Deferred to BLE phase |
| BLE commands, pairing, keep-alive, and feature APIs | Kotlin BLE transport | Deferred |

`MediaList`, `MediaDirectory`, and `MediaFile` map the HTTP JSON fields and retain unrecognized
fields. Network and JSON errors are surfaced rather than hidden behind success-shaped defaults.
GpSharp does not currently implement Kotlin's credential-backed HTTPS provisioning, BLE transport,
live `isReady`/`isEncoding` streams, or the full Kotlin command/feature inventory.

See `docs/HTTP_PROTOCOL.md`, `docs/API_OVERVIEW.md`, and `docs/ROADMAP.md` for the supported API
and phase boundary.
