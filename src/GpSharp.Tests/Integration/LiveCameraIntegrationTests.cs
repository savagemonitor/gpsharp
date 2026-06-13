using System;
using System.Collections.Generic;
using System.Linq;
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
        var discoveryService = new MdnsDiscovery();
        var sdk = new OgpSdk(discoveryService);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15)); // Limit search time

        // Act
        IEnumerable<DiscoveredDevice> discoveredDevices = Enumerable.Empty<DiscoveredDevice>();

        try
        {
            while (!cts.IsCancellationRequested)
            {
                discoveredDevices = await sdk.DiscoverAsync(cts.Token);
                if (discoveredDevices.Any())
                {
                    break;
                }
                await Task.Delay(2000, cts.Token); // Poll every 2 seconds
            }
        }
        catch (OperationCanceledException)
        {
             // Expected on timeout
        }

        if (!discoveredDevices.Any())
        {
            return;
        }

        var device = discoveredDevices.First();
        var goPro = await sdk.ConnectAsync(device);

        // Assert
        goPro.Should().NotBeNull();
        goPro.Commands.Should().NotBeNull();

        // Attempt a basic command that is likely to work if connected (e.g., get media list)
        var mediaList = await goPro.Commands.GetMediaListAsync();
        mediaList.Should().NotBeNull();
    }
}
