# Kotlin SDK Parity: Phase 1

The Kotlin reference is `gopro/OpenGoPro/demos/kotlin/kmp_sdk/wsdk` in the OpenGoPro repository.
`savagemonitor/ogpmcpsharp` is a separate .NET MCP server and is not the Kotlin SDK.

The Kotlin SDK does not label a subset as "Phase 1", so GpSharp applies the repository's existing
HTTP-first Phase 1 boundary. Settings/status HTTP endpoints are supported by the Open GoPro
protocol, but the Kotlin WSDK routes its generic setting/status API through BLE; GpSharp's HTTP
subset is therefore a protocol extension, not transport parity for those Kotlin APIs.

| Kotlin API/protocol | GpSharp mapping | Status |
|---|---|---|
| `discover(NetworkType.WIFI_WLAN)` returns a `Flow` | mDNS `_gopro-web._tcp.` returns `IAsyncEnumerable` | Implemented |
| `setShutter(true/false)` | `SetShutterAsync` sends GET to `/gopro/camera/shutter/start` or `/stop` | Implemented |
| `getMediaList()` | `GetMediaListAsync` GETs `/gopro/media/list` | Implemented |
| Setting read/write | `ISetting` uses `/gopro/camera/setting` with setting and option IDs | HTTP subset implemented; Kotlin WSDK uses BLE |
| Status read/state snapshot | `IStatus` and `GetCameraStateAsync` use `/gopro/camera/state` | HTTP subset implemented; Kotlin WSDK uses BLE |
| `GoProId` | Value-based C# record key | Implemented |
| Proactive setting capabilities and status update streams | Kotlin BLE behavior | Deferred to BLE phase |
| BLE commands, pairing, keep-alive, and feature APIs | Kotlin BLE transport | Deferred |

`MediaList`, `MediaDirectory`, and `MediaFile` map the HTTP JSON fields and retain unrecognized
fields. Network and JSON errors are surfaced rather than hidden behind success-shaped defaults.
Setting option writes accept non-negative integers/enums; named setting IDs are 2 (video resolution)
and 3 (video frame rate), and named status IDs are 10 (encoding) and 70 (internal battery
percentage). Generic IDs are non-negative integers, including protocol IDs above 255, and the bulk
snapshot preserves values as `JsonElement`.
Camera-rejected options can return supported-option metadata, but there is no proactive HTTP
capability endpoint or update stream. Automated tests use mocks; physical setting writes are not
performed.
GpSharp does not currently implement Kotlin's credential-backed HTTPS provisioning, BLE transport,
live `isReady`/`isEncoding` streams, or the full Kotlin command/feature inventory.

See `docs/HTTP_PROTOCOL.md`, `docs/API_OVERVIEW.md`, and `docs/ROADMAP.md` for the supported API
and phase boundary.
