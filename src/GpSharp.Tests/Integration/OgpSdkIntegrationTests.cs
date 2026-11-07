using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            .Returns(DiscoverOneAsync(device));

        var sdk = new OgpSdk(mockDiscovery.Object);

        // Act
        var discovered = new List<DiscoveredDevice>();
        await foreach (var foundDevice in sdk.DiscoverAsync())
        {
            discovered.Add(foundDevice);
        }
        var goPro = await sdk.ConnectAsync(device);

        // Assert
        goPro.Should().NotBeNull();
        goPro.Commands.Should().NotBeNull();
        
        var retrievedGoPro = sdk.GetGoPro(device.Id);
        retrievedGoPro.Should().Be(goPro);
        sdk.GetGoPro(new GoProId(device.Id.PartialSerial)).Should().Be(goPro);
    }

    [Fact]
    public async Task ConnectAsync_ShouldPreserveIpv6ScopeForHttp()
    {
        var sdk = new OgpSdk(new Mock<IDiscoveryService>().Object);
        var device = new DiscoveredDevice(new GoProId("1234"), "camera", "fe80::1234%42", 80, "Unknown", "Unknown");
        var camera = await sdk.ConnectAsync(device);
        var connection = typeof(GpSharp.GoPro.GoPro).GetField("_connection", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(camera)!;
        var client = (HttpClient)connection.GetType().GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(connection)!;

        client.BaseAddress!.DnsSafeHost.Should().Be("fe80::1234%42");
    }

    [Fact]
    public async Task ConnectAsync_ShouldRejectLinkLocalIpv6WithoutScope()
    {
        var sdk = new OgpSdk(new Mock<IDiscoveryService>().Object);
        var device = new DiscoveredDevice(new GoProId("1234"), "camera", "fe80::1234", 80, "Unknown", "Unknown");

        Func<Task> connect = async () => await sdk.ConnectAsync(device);
        await connect.Should().ThrowAsync<ArgumentException>().WithMessage("*requires an interface scope*");
    }

    private static async IAsyncEnumerable<DiscoveredDevice> DiscoverOneAsync(
        DiscoveredDevice device,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        yield return device;
        await Task.CompletedTask;
    }
}