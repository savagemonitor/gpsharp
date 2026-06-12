# GpSharp C# SDK Implementation Plan

## Overview

Implement a C# port of the OpenGoPro Kotlin Multiplatform SDK with similar interfaces and architecture. The SDK will provide a high-level API for discovering, connecting, and controlling GoPro cameras via HTTP and BLE protocols. Start with HTTP implementation first (simpler), then add BLE in a later phase.

**Key Principles:**
- Multi-target: .NET Standard 2.1 + .NET 6/8/10
- TDD approach: Unit tests written before product code
- Async-first: Task<T> for single operations, IAsyncEnumerable for streams
- Mirror Kotlin design: OgpSdk → GoPro → Commands/Settings/Statuses/Features
- Replace Kotlin stdlib with .NET equivalents (Task vs Future, etc.)
- Self-contained: Only repo code is tested (external dependencies presumed correct)
- Each logical unit = separate commit with tests + code
- Documentation in separate commits, stored in .github folder

## Project Structure

```
GpSharp/
├── src/
│   ├── GpSharp/                           # Main SDK library
│   │   ├── Core/
│   │   │   ├── OgpSdk.cs                 # SDK entry point
│   │   │   ├── GoProId.cs                # GoPro identifier (last 4 serial digits)
│   │   │   └── NetworkType.cs            # Enum: BLE, WiFi, USB
│   │   ├── GoPro/
│   │   │   ├── GoPro.cs                  # Main device interface
│   │   │   ├── IConnectable.cs           # Connection interface
│   │   │   ├── Commands/
│   │   │   │   ├── ICommandsContainer.cs
│   │   │   │   └── CommandsContainer.cs
│   │   │   ├── Settings/
│   │   │   │   ├── ISetting.cs
│   │   │   │   ├── ISettingsContainer.cs
│   │   │   │   └── SettingsContainer.cs
│   │   │   ├── Statuses/
│   │   │   │   ├── IStatus.cs
│   │   │   │   ├── IStatusesContainer.cs
│   │   │   │   └── StatusesContainer.cs
│   │   │   └── Features/
│   │   │       ├── IFeaturesContainer.cs
│   │   │       └── FeaturesContainer.cs
│   │   ├── Network/
│   │   │   ├── INetworkConnection.cs
│   │   │   ├── HttpConnection.cs
│   │   │   ├── HttpClient.cs
│   │   │   └── NetworkDiscovery.cs
│   │   ├── Data/
│   │   │   ├── DiscoveredDevice.cs
│   │   │   └── ... (data models from OpenGoPro API)
│   │   └── GpSharp.csproj
│   └── GpSharp.Tests/
│       ├── Core/
│       ├── Network/
│       ├── Commands/
│       └── GpSharp.Tests.csproj
├── docs/
│   ├── ARCHITECTURE.md                   # High-level design overview
│   ├── DESIGN_PRINCIPLES.md              # Kotlin-to-C# design decisions
│   ├── API_OVERVIEW.md                   # Getting started and usage guide
│   ├── HTTP_PROTOCOL.md                  # HTTP specifics and reference
│   ├── BUILD_INSTRUCTIONS.md
│   ├── TESTING_STRATEGY.md
│   ├── ROADMAP.md
│   └── CONTRIBUTING.md
├── .github/
│   └── workflows/                        # CI/CD workflows (future)
├── GpSharp.sln
└── Directory.Build.props                 # Shared build settings
```

## Implementation Phases

### Phase 1: Foundation & HTTP (Priority: HIGH)

**Core Structure**
- [x] Set up solution, projects, and build configuration
  - Create GpSharp.sln with SDK and Tests projects
  - Multi-target to .NET Standard 2.1 + .NET 6/8/10
  - Configure xUnit and test runner
  - Set up analyzer and linting rules

- [x] Implement GoProId and NetworkType
  - GoProId: Last 4 digits of serial number
  - NetworkType enum: WiFi, BLE, USB

- [x] Design base interfaces for Results/Errors
  - Equivalent to Kotlin Result<T>
  - Error types and handling

**OgpSdk Core**
- [x] Implement OgpSdk entry point
  - Initialization with app context
  - Device discovery interface
  - Device connection management
  - GoPro retrieval by GoProId

**HTTP Network Layer**
- [x] Implement HttpConnection class
  - Establish HTTP connection to GoPro
  - Handle base URL construction
  - Request/response handling
  - Connection lifecycle (open/close)

- [ ] Implement NetworkDiscovery
  - Discover GoPro on local network (mDNS)
  - Return DiscoveredDevice objects

**GoPro Main Interface**
- [x] Implement GoPro class
  - Hold reference to active connections
  - Provide Commands/Settings/Statuses/Features containers
  - Connection state management
  - Ready/encoding state tracking
  - Connection drop detection (IObservable<NetworkType>)

**Commands Container (HTTP-only subset)**
- [x] Implement ICommandsContainer interface
- [x] Implement basic commands (HTTP only first)
  - setShutter(bool)
  - getMediaList()
  - capturePhoto()
  - startVideo()
  - stopVideo()

**Settings Container (HTTP-only subset)**
- [x] Implement ISetting interface
  - getValue<T>()
  - getCapabilities()
  - setValue<T>()
  - registerValueUpdates() → IAsyncEnumerable
  - registerCapabilityUpdates() → IAsyncEnumerable
- [x] Implement ISettingsContainer
  - videoResolution
  - videoFrameRate
  - (and other common settings)

**Statuses Container (HTTP-only subset)**
- [x] Implement IStatus interface
  - getValue<T>()
  - registerValueUpdates() → IAsyncEnumerable
- [x] Implement IStatusesContainer
  - internalBatteryPercentage
  - isEncoding
  - (and other common statuses)

**Documentation (Phase 1 commits)**
- [x] Create ARCHITECTURE.md in docs/ folder
  - High-level architecture overview
  - Comparison with Kotlin design
  - .NET-specific decisions
  - Performance considerations
- [x] Create API_OVERMSVIEW.md in docs/
  - Basic SDK usage examples
  - HTTP protocol specifics
  - Error handling

### Phase 2: BLE Implementation (Priority: MEDIUM)

- [ ] Implement BLE discovery
- [ ] Implement BLE connection
- [ ] Implement BLE command/setting/status operations
- [ ] Feature parity with HTTP where applicable

### Phase 3: Advanced Features & Polish (Priority: MEDIUM)

- [ ] Features container
  - AccessPointFeature
  - LivestreamFeature
- [ ] Media file access and streaming
- [ ] Keep-alive signal management
- [ ] Full documentation
- [ ] Sample demo application
- [ ] Logging and diagnostics

## Build & Testing Strategy

**TDD Approach:**
1. Write unit tests for each feature BEFORE implementation
2. Tests should be isolated (mock external dependencies)
3. Use xUnit with Moq for mocking
4. Aim for >80% code coverage on product code

**Build Requirements:**
- `dotnet build` → Full solution
- `dotnet test` → Run all unit tests
- `dotnet pack` → Create NuGet package
- Document build steps in docs/BUILD_INSTRUCTIONS.md

**Linting & Code Style:**
- StyleCop analyzers for C# conventions
- EditorConfig for consistency across IDEs
- Nullable reference types enabled

## Testing Framework

**Tools:**
- xUnit: Test framework
- Moq: Mocking library
- FluentAssertions: Better test assertions

**Test Structure:**
- Unit tests only (no integration tests initially)
- Mock all network I/O
- Test categories:
  - Core (SDK initialization, GoProId, NetworkType)
  - Network (HTTP client, discovery)
  - Containers (Commands, Settings, Statuses)
  - Data models (serialization/deserialization)

## Documentation Structure

**In `docs/` folder (as requested):**
- ARCHITECTURE.md: Overall design philosophy and component layout
- DESIGN_PRINCIPLES.md: C# vs Kotlin design trade-offs and decisions
- API_OVERVIEW.md: Getting started guide and usage examples
- HTTP_PROTOCOL.md: HTTP specifics and reference
- BUILD_INSTRUCTIONS.md: How to build and test
- TESTING_STRATEGY.md: Testing approach and coverage goals
- ROADMAP.md: Current and future phases
- CONTRIBUTING.md: Developer contribution guidelines

**In `.github/` folder:**
- workflows/: CI/CD workflows (future phase)

**In code:**
- XML doc comments on all public API surfaces
- Examples in doc comments where helpful

## Commit Strategy

Each commit should:
1. Address one logical unit of work
2. Include unit tests + product code together
3. Include documentation if relevant
4. Have clear commit message with context
5. Be buildable and testable on its own

Example commits:
- `feat: Initialize solution and project structure`
- `test: Add tests for OgpSdk.discover()`
- `feat: Implement OgpSdk discovery and connection`
- `test: Add tests for HttpConnection`
- `feat: Implement HTTP network layer`
- `docs: Add architecture documentation`
- etc.

## Dependencies

**Target dependencies (minimal):**
- For HTTP: HttpClient (built-in to .NET)
- For async streaming: IAsyncEnumerable (built-in)
- For testing: xUnit, Moq, FluentAssertions
- For BLE: Platform-specific (handled in Phase 2)

**No heavy external dependencies** — leverage .NET standard library

## Success Criteria

Phase 1 completion:
- [x] Full HTTP protocol support working
- [x] All Commands/Settings/Statuses accessible
- [x] Unit test coverage >80%
- [x] Can discover and connect to GoPro via HTTP
- [x] Can get/set settings and read statuses
- [x] Can execute commands
- [x] Documentation complete in docs folder

## Notes

- **Design Philosophy Alignment:** This plan mirrors the Kotlin SDK's layered architecture (OgpSdk → GoPro → Containers) while using C#/.NET idioms (Task, IAsyncEnumerable, CancellationToken)
- **C# vs Kotlin Trade-offs:** Result<T> in Kotlin maps to Task<T> in C#; Flow<T> maps to IAsyncEnumerable<T>
- **Self-contained testing:** Only this repo's code is unit tested; external APIs and libraries are assumed correct
- **Future: BLE will require platform-specific bindings** (InTheHand.BluetoothLE for cross-platform, or platform-specific in Phase 2)
