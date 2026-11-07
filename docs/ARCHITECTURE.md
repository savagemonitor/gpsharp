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
service browses `_gopro-web._tcp.` and streams unique camera announcements. The HTTP surface
implements shutter start/stop, media-list retrieval, setting reads/writes, and camera-state/status
reads using the Open GoPro endpoint paths and response shapes. The setting/status HTTP subset is
based on the Open GoPro protocol; the Kotlin WSDK currently routes its generic setting/status APIs
through BLE rather than HTTP.

Settings use `/gopro/camera/setting` for direct reads and writes. Statuses and a full settings
snapshot use `/gopro/camera/state`. Each call is a request/response operation: there is no HTTP
capability stream or automatic update stream. BLE-specific setting/status behavior, proactive
capability discovery, live state streams, and advanced features remain later-phase work.

## .NET conventions

Network operations use `Task` and cancellation-aware discovery uses `IAsyncEnumerable`. The
`HttpClient` implementation propagates HTTP failures; it does not hide them behind default values.
