# Design Principles

The GpSharp SDK is designed with several core principles in mind, primarily influenced by the original OpenGoPro Kotlin Multiplatform SDK.

## 1. Architecture Alignment
We maintain a layered architecture that mirrors the Kotlin SDK:
- **OgpSdk**: The single entry point for all users.
- **GoPro**: A device-specific object managing connection state and feature containers.
- **Containers**: Specialized containers for Commands, Settings, Statuses, and Features to organize the API surface area.

## 2. Async-First
Every potentially long-running operation (network I/O, discovery, hardware interaction) is asynchronous and returns a `Task` or implements `IAsyncEnumerable`. We utilize `CancellationToken` extensively to allow users to manage operation lifecycles.

## 3. Result Pattern vs Exceptions
While we use standard .NET exceptions for critical system failures (e.g., network loss), the SDK favors a pattern that avoids unnecessary try-catch blocks for expected logic outcomes, where appropriate, though often leveraging `Task` results directly in modern C#.

## 4. Dependency Injection Friendly
All major components are designed to be easily mocked and injected. This makes the SDK highly testable and allows developers to swap out transport layers (e.m., switching from HTTP to BLE) without changing their high-level application logic.

## 5. Modern C# Idioms
We leverage modern .NET features to provide a clean and expressive API:
- **Records**: For immutable data models.
- **IAsyncEnumerable**: For streaming telemetry and status updates.
- **Nullable Reference Types**: To improve type safety and reduce null-related errors.

## 6. Minimal Dependencies
The SDK aims for minimal external dependencies, relying primarily on the .NET standard library to ensure ease of integration into any modern .NET application.
