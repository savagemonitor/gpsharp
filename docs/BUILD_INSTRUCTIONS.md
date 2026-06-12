# Build Instructions

This document provides instructions on how to build the GpSharp SDK and its test suite.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (Version 6, 8, or 10 recommended)
- An IDE like Visual Studio, VS Code, or JetBrains Rider.

## Building the Project

From the root of the repository, run the following command:

```bash
dotnet build
```

This will restore dependencies and compile both the `GpSharp` library and the `GpSharp.Tests` project.

## Running Tests

To execute all unit and integration tests in the suite, use:

```bash
dotnet test
```

All tests are designed to be platform-independent and should pass on any standard .NET environment.

## Packaging

To create a NuGet package for distribution, run:

```bash
dotnet pack
```

The resulting `.nupkg` file will be found in the `src/GpSharp/bin/Release/` directory after running a release build.
