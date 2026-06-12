# Testing Strategy

The GpSharp SDK follows a **Test-Driven Development (TDD)** approach to ensure high reliability and prevent regressions during the implementation of new features.

## Testing Layers

### 1. Unit Tests
These tests focus on individual components in isolation, using mocks for all external dependencies (like network I/O).
- **Location**: `src/GpSharp.Tests/Core/`, `src/GpSharp.Tests/Network/`, etc.
- **Tooling**: xUnit, Moq, FluentAssertions.
- **Goal**: Verify logic correctness of types like `GoProId`, `Result`, and container parsing logic.

### 2. Integration Tests
These tests verify the interaction between different layers of the SDK (e.g., `OgpSdk` interacting with `IDiscoveryService`).
- **Location**: `src/GpSharp.Tests/Integration/`.
- **Goal**: Ensure that the orchestration of components works as expected, particularly during device discovery and connection flows.

## Testing Principles

- **Isolation**: Always mock network interfaces (`IHttpConnection`, `IDiscoveryService`) to ensure tests are fast and deterministic.
- **Coverage**: Aim for >80% code coverage on all production code.
- **Automation**: All tests are part of the build process and should be run before every commit/PR.

## How to Run Tests

Simply execute the following command from the repository root:

```bash
dotnet test
```
