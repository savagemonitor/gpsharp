# Roadmap

The roadmap tracks the supported scope of GpSharp against the OpenGoPro Kotlin Multiplatform SDK.
The Kotlin reference source is in `gopro/OpenGoPro/demos/kotlin/kmp_sdk/wsdk`.

## Phase 1: Foundation and HTTP

Phase 1 delivers the HTTP-only surface that the Kotlin SDK supports without BLE:

- [x] Project setup, identity types, and result/error foundations
- [x] Open GoPro mDNS discovery via `_gopro-web._tcp.`
- [x] Async discovery stream (`IAsyncEnumerable<DiscoveredDevice>`)
- [x] HTTP connection and camera command container
- [x] `SetShutterAsync` using the camera shutter start/stop endpoints
- [x] `GetMediaListAsync` with the Open GoPro media-list response model
- [x] API and HTTP protocol documentation
- [x] Build and automated test verification

Settings, statuses, and BLE-specific connection state are not Phase 1 HTTP features: the Kotlin
SDK routes those setting/status operations through BLE. GpSharp keeps the planned surface visible,
but calls currently fail explicitly instead of returning placeholder values.

## Phase 2: BLE and device state

- [ ] Bluetooth Low Energy discovery and connection management
- [ ] BLE commands
- [ ] Settings and status reads/writes, capabilities, and update streams
- [ ] Live readiness, encoding, and disconnect state
- [ ] Keep-alive and reconnect management

## Phase 3: Advanced features and polish

- [ ] Access point, COHN, livestream, and other feature implementations
- [ ] Media download and streaming support
- [ ] Sample application
- [ ] Diagnostics and logging
