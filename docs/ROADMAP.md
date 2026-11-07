# Roadmap

The roadmap tracks the supported scope of GpSharp against the OpenGoPro Kotlin Multiplatform SDK.
The Kotlin reference source is in `gopro/OpenGoPro/demos/kotlin/kmp_sdk/wsdk`.

## Phase 1: Foundation and HTTP

Phase 1 delivers the supported HTTP camera surface and foundations:

- [x] Project setup, identity types, and result/error foundations
- [x] Open GoPro mDNS discovery via `_gopro-web._tcp.`
- [x] Async discovery stream (`IAsyncEnumerable<DiscoveredDevice>`)
- [x] HTTP connection and camera command container
- [x] `SetShutterAsync` using the camera shutter start/stop endpoints
- [x] `GetMediaListAsync` with the Open GoPro media-list response model
- [x] Setting reads and writes by protocol ID using `/gopro/camera/setting`
- [x] Bulk settings/status snapshots and typed status reads using `/gopro/camera/state`
- [x] API and HTTP protocol documentation
- [x] Build and automated test verification

The settings/status HTTP operations implement the Open GoPro HTTP protocol subset; the Kotlin WSDK
routes its generic settings/status APIs through BLE, so these do not claim Kotlin transport parity.
GpSharp exposes named setting IDs 2 and 3 and named status IDs 10 and 70, as well as generic
protocol-ID access and raw state snapshots. Only integer setting options are written. A camera
rejection may expose `supported_options`, but there is no proactive HTTP capability endpoint.
BLE-specific settings/status behavior, capability discovery, update streams, and live connection
state remain outstanding. Automated tests use mocked responses; physical-camera verification is
still outstanding. Media download is explicitly out of scope.

## Phase 2: BLE and device state

- [ ] Bluetooth Low Energy discovery and connection management
- [ ] BLE commands
- [ ] BLE-backed Kotlin settings/status parity, capability discovery, and update streams
- [ ] Live readiness, encoding, and disconnect state
- [ ] Keep-alive and reconnect management

## Phase 3: Advanced features and polish

- [ ] Access point, COHN, livestream, and other feature implementations
- [ ] Media download and streaming support
- [ ] Sample application
- [ ] Diagnostics and logging
