using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Core;
using GpSharp.Data;
using GpSharp.Network;
using Xunit;
using FluentAssertions;

namespace GpSharp.Tests.Integration;

/// <summary>
/// Integration tests that attempt to interact with real hardware or network services.
/// These tests are intended to be run in an environment where a GoPro is available.
/// </summary>
public class LiveCameraIntegrationTests
{
    [Fact]
    public async Task DiscoverAndConnect_WithRealCamera_ShouldSucceed()
    {
        // Arrange
        using var discoveryService = new MdnsDiscovery();
        var sdk = new OgpSdk(discoveryService);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15)); // Limit search time

        // Act
        var discoveredDevices = new List<DiscoveredDevice>();

        try
        {
            await foreach (var discoveredDevice in sdk.DiscoverAsync(cts.Token))
            {
                discoveredDevices.Add(discoveredDevice);
                break;
            }
        }
        catch (OperationCanceledException)
        {
             // Expected on timeout
        }

        if (discoveredDevices.Count == 0)
        {
            return;
        }

        var device = discoveredDevices[0];
        var goPro = await sdk.ConnectAsync(device);

        // Assert
        goPro.Should().NotBeNull();
        goPro.Commands.Should().NotBeNull();

        // Attempt a basic command that is likely to work if connected (e.g., get media list)
        var mediaList = await goPro.Commands.GetMediaListAsync();
        mediaList.IsSuccess.Should().BeTrue();
    }
}
