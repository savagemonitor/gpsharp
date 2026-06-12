using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Core;
using GpSharp.Data;
using GpSharp.GoPro;
using GpSharp.Network;
using Moq;
using Xunit;
using FluentAssertions;

namespace GpSharp.Tests.Integration;

public class OgpSdkIntegrationTests
{
    [Fact]
    public async Task ConnectAsync_WithDiscoveredDevice_ShouldReturnConnectedGoPro()
    {
        // Arrange
        var mockDiscovery = new Mock<IDiscoveryService>();
        var device = new DiscoveredDevice(
            new GoProId("1234"),
            "Test GoPro",
            "192.168.1.50",
            443,
            "00:11:22:33:44:55",
            "Hero12"
        );

        mockDiscovery.Setup(d => d.DiscoverAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiscoveredDevice> { device });

        var sdk = new OgpSdk(mockDiscovery.Object);

        // Act
        var discovered = await sdk.DiscoverAsync();
        var goPro = await sdk.ConnectAsync(device);

        // Assert
        goPro.Should().NotBeNull();
        goPro.Commands.Should().NotBeNull();
        
        var retrievedGoPro = sdk.GetGoPro(device.Id);
        retrievedGoPro.Should().Be(goPro);
    }
}