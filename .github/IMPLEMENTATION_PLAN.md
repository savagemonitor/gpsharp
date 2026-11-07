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
- [x] Typed media-list models that retain unrecognized response fields
- [x] Build and automated test verification
- [ ] Validate discovery and commands against a physical camera

### Phase 1 boundary

The Kotlin SDK's settings, statuses, update streams, BLE command transport, pairing, and live
connection state are BLE-backed. They are intentionally deferred; the GpSharp placeholders throw
`NotSupportedException` rather than issue invented HTTP requests or return default values. The
Phase 1 command API is limited to operations with an HTTP implementation in the Kotlin SDK; it does
not invent separate photo/video methods.

## Phase 2: BLE and state

- BLE discovery and connection management
- BLE command operations
- Typed settings and status queries, writes, capabilities, and update streams
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
