# Phase 1 Kotlin/C# Alignment

The Kotlin reference is `gopro/OpenGoPro/demos/kotlin/kmp_sdk/wsdk`. The similarly named
`savagemonitor/ogpmcpsharp` repository is a .NET MCP server, not the Kotlin SDK.

## Implemented HTTP parity

| Kotlin SDK capability | GpSharp Phase 1 |
|---|---|
| `OgpSdk.discover(): Flow<ScanResult>` | `OgpSdk.DiscoverAsync(): IAsyncEnumerable<DiscoveredDevice>` |
| `_gopro-web._tcp.` DNS discovery | `MdnsDiscovery` browses the same service type |
| `commands.setShutter(Boolean)` | `SetShutterAsync(Boolean)` with the matching GET start/stop paths |
| `commands.getMediaList()` | `GetMediaListAsync()` reads `/gopro/media/list` into typed response models |
| GoPro ID lookup | `GoProId` uses value equality for connected-device lookup |

## Explicitly deferred

The Kotlin implementation routes setting reads/writes/capabilities/updates and status reads/updates
through BLE. GpSharp does not claim HTTP parity for those features; its placeholder operations throw
`NotSupportedException` until the BLE phase. BLE discovery, pairing, keep-alive, live state streams,
advanced features, and media download are also outside Phase 1.

HTTP errors and malformed media JSON propagate to callers. The SDK no longer converts failures to
empty media collections or default setting/status values.

## Verification

See `docs/ROADMAP.md` for current phase completion and validation status.
