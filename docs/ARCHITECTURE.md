# Architecture Overview

## Overview
The GpSharp SDK provides a high-level C# interface for interacting with GoPro cameras. It mirrors the architecture of the original OpenGoPro Kotlin Multiplatform SDK while adapting to idiomatic .NET patterns.

## Layered Architecture
1. **Core**: Contains the primary entry points (`OgpSdk`), value objects (`GoProId`, `NetworkType`), and result handling logic.
2. **Hardware/Domain**: The `GoPro` class acts as the primary controller for a specific device, coordinating features like commands, settings, and status streams.
3. **Connectivity (Network)**: Abstractions for different transport layers (HTTP, BLE). Currently, HTTP is the primary implementation.
4. **Data Model**: Plain objects representing telemetry, configuration items, and state information.

## Kotlin vs. .NET Design Decisions
- **Result Handling**: Replaced Kotlin's `Result<T>` with a custom `.NET` Result type or `Task<T>` for asynchronous operations.
- **Streaming**: Mapped Kotlin `Flow<T>` to C# `IAsyncEnumerable<T>`.
- **Concurrency**: Utilized `async/await` and `CancellationToken` instead of Coroutines.

## .NET Specifics
- **Multi-targeting**: Supported via `.NET Standard 2.1`, `net6.0`, and `net8.0`.
- **Async-First**: All network operations are non-blocking.
- **Data Binding**: Leverages modern C# records for immutable data models.
