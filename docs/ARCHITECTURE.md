# Architecture Overview

GpSharp is a .NET adaptation of the OpenGoPro Kotlin Multiplatform SDK. The Kotlin reference
implementation is under `demos/kotlin/kmp_sdk/wsdk` in `gopro/OpenGoPro`.

## Layers

- **Core:** `OgpSdk`, value-based `GoProId`, discovery orchestration, and `Result<T>`.
- **Device:** `GoPro` exposes command, settings, status, and feature containers for a connected
  camera.
- **Network:** `IDiscoveryService` and `IHttpConnection` isolate mDNS and HTTP from device APIs.
- **Data:** Discovered devices and typed Open GoPro HTTP response models.

## Phase 1 transport/API boundary

Kotlin `Flow<ScanResult>` maps to `IAsyncEnumerable<DiscoveredDevice>`. The Phase 1 discovery
service browses `_gopro-web._tcp.` and streams unique camera announcements. The HTTP command subset
implements shutter start/stop and media-list retrieval with the Open GoPro endpoint paths and JSON
shape.

Kotlin settings and status operations explicitly select BLE communicators. They are therefore
outside GpSharp's HTTP-only Phase 1; the current placeholders fail explicitly rather than
pretending to read or write camera state. BLE, live state streams, and advanced features remain
later-phase work.

## .NET conventions

Network operations use `Task` and cancellation-aware discovery uses `IAsyncEnumerable`. The
`HttpClient` implementation propagates HTTP failures; it does not hide them behind default values.
