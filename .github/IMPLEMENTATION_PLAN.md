# GpSharp Implementation Plan

This plan tracks parity with the HTTP-capable subset of the OpenGoPro Kotlin Multiplatform SDK.
The Kotlin reference source is `gopro/OpenGoPro/demos/kotlin/kmp_sdk/wsdk`. That SDK does not
define a separate "Phase 1"; GpSharp uses the repository roadmap's HTTP-first phases.

## Phase 1: Foundation and HTTP

- [x] Project setup, nullable types, and test project
- [x] Value-based four-character `GoProId`
- [x] Open GoPro mDNS discovery for `_gopro-web._tcp.`
- [x] Kotlin `Flow`-style asynchronous discovery using `IAsyncEnumerable`
- [x] HTTP connection to discovered camera addresses
- [x] Shutter start/stop via `GET /gopro/camera/shutter/{start|stop}`
- [x] Media-list retrieval via `GET /gopro/media/list`
- [x] Setting reads/writes via `GET /gopro/camera/setting` (protocol IDs 0..255; integer option values)
- [x] Status reads and bulk state snapshot via `GET /gopro/camera/state`
- [x] Typed media-list models that retain unrecognized response fields
- [x] Raw JSON state values keyed by setting/status protocol ID
- [x] Cancellation and `Result<T>` handling for settings/status requests
- [x] Build and automated test verification
- [ ] Validate discovery and commands against a physical camera
- [ ] Validate setting/status reads against a physical camera (no physical writes performed)

### Phase 1 boundary

Setting and status calls use the documented Open GoPro HTTP endpoints; this extends the Kotlin
WSDK's currently BLE-backed generic API and is not transport parity with Kotlin. GpSharp supports
named setting IDs 2/3 and status IDs 10/70, generic ID access, request/response reads, integer
option writes, and error metadata when a write response supplies supported options. It does not
provide proactive HTTP capability discovery, update streams, BLE settings/status behavior, or live
connection state. Those remain Phase 2 work. No physical-camera setting writes were used in
validation.

## Phase 2: BLE and state

- BLE discovery and connection management
- BLE command operations
- Kotlin BLE-backed settings/status behavior, capabilities, and update streams
- Live readiness, encoding, and disconnect state
- Keep-alive and reconnect management

## Phase 3: Advanced capabilities

- Access point, COHN, livestream, and other features
- Media downloads and streaming
- Sample application, diagnostics, and logging

## Validation commands

Run the test project with:

```powershell
dotnet test src\GpSharp.Tests\GpSharp.Tests.csproj
```

Build the SDK with:

```powershell
dotnet build src\GpSharp\GpSharp.csproj
```
