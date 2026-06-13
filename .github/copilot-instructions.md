# Copilot Instructions for GpSharp

## Build, Test, and Lint
- **Build the main project:** `dotnet build src\GpSharp\GpSharp.csproj`
- **Run all tests:** `dotnet test src\GpSharp.Tests\GpSharp.Tests.csproj`
- **Run a specific test:** `dotnet test src\GpSharp.Tests\GpSharp.Tests.csproj --filter Name~"ClassName"`
- **Linting/Analysis:** Analysis is integrated into the build process via .NET Analyzers and StyleCop.

## High-Level Architecture
The SDK follows a layered architecture inspired by the OpenGoPro Kotlin Multiplatform SDK:
- **Entry Point (`OgpSdk`):** Handles initialization, device discovery (malleable for HTTP/BLE), and connection management.
- **Device Interface (`GoPro`):** The primary interface representing the camera hardware.
- **Containerized Features:** Instead of a flat API, capabilities are grouped into specialized containers to maintain modularity:
    - `Commands`: Action-oriented operations (e.g., `CapturePhotoAsync`).
    - `Settings`: Configuration management (e.g., `VideoResolution`).
    - `Statuses`: Real-time telemetry/state (e.g., `IsEncoding`).
    - `Features`: Advanced, protocol-specific capabilities (e.g., `Livestream`).
- **Network Layer:** Decoupled transport implementation (currently HTTP via `HttpConnection`, with BLE planned).

## Key Conventions
- **Result Pattern:** Use `Result<T>` for domain-level errors and expected failures (e.s., device not found, command rejected). Do not use exceptions for recoverable logic errors.
- **Async-First:** All I/O and network operations must be asynchronous using `Task` or `IAsyncEnumerable`.
- **Single Class per File:** Adhere to the C# convention of having one primary class or interface per file.
- **TDD Pattern:** Follow a Test-Driven Development approach. Write the xstring xUnit test in the `GpSharp.Tests` project before implementing the corresponding logic in `GpSharp`. Verify that tests fail first, then implement the minimum code to pass the test, and finally refactor as needed while keeping tests green.  
- **Interface-Driven Design:** Always implement features against their respective interfaces (`ICommandsContainer`, etc.) to support multi-transport (HTTP/BLE) and easier mocking with Moq.
- **Nullability:** Nullable reference types are enabled. Pay close attention to nullability annotations to ensure API safety.

## Key Considerations
- The intent is to remain as close to the original Kotlin design as possible while adhering to C# conventions and idioms. This means some structural differences (e.g., no sealed classes, use of interfaces and classes instead) but the overall architecture and API design should feel familiar to users of the OpenGoPro Kotlin SDK.
- The Kotlin SDK can be found at github.com/gopro/OpenGoPro/tree/main/demos/kotlin/kmp_sdk. Reference back to it when designing features to maintain consistency in API design and behavior.
- User commits should be seen as authoritative over AI commits. If there are discrepancies between the two, prioritize the user commits as they reflect the intended design and implementation decisions. AI commits should be used as suggestions or starting points, but the final code should align with the user's vision and requirements. Prompt the user if there's a detected discrepancy to clarify which direction to take.