using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GpSharp.Network;
using Moq;
using Tmds.MDns;
using Xunit;

namespace GpSharp.Tests.Network;

public class MdnsDiscoveryTests
{
    [Fact]
    public void Discovery_ShouldStartBrowseWithServiceTypeExpectedByTmds()
    {
        using var discovery = new MdnsDiscovery();
        // Tmds.MDns 0.8 appends ".local." to each registered service type.
        var browser = typeof(MdnsDiscovery)
            .GetField("_serviceBrowser", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(discovery).Should().BeOfType<ServiceBrowser>().Subject;
        var serviceTypes = typeof(ServiceBrowser)
            .GetField("_serviceTypes", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(browser).Should().BeAssignableTo<IEnumerable<string>>().Subject;

        serviceTypes.Should().ContainSingle().Which.Should().Be("_gopro-web._tcp");
    }

    [Fact]
    public async Task Discovery_ShouldNotMislabelAnMdnsAnnouncementAsWifi()
    {
        using var discovery = new MdnsDiscovery(startBrowse: false);
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var browser = typeof(MdnsDiscovery)
            .GetField("_serviceBrowser", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(discovery).Should().BeOfType<ServiceBrowser>().Subject;
        var announcement = new ServiceAnnouncement();
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Type), "_gopro-web._tcp");
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Hostname), "camera1234");
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Port), (ushort)80);
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Addresses), new List<IPAddress> { IPAddress.Parse("192.0.2.1") });
        var added = typeof(ServiceBrowser)
            .GetField("ServiceAdded", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(browser).Should().BeOfType<EventHandler<ServiceAnnouncementEventArgs>>().Subject;

        added(browser, new ServiceAnnouncementEventArgs(announcement));

        await using var devices = discovery.DiscoverAsync(cancellation.Token).GetAsyncEnumerator();
        (await devices.MoveNextAsync()).Should().BeTrue();
        devices.Current.Id.PartialSerial.Should().Be("1234");
        devices.Current.ConnectionType.Should().Be(ConnectionType.Unknown);
    }

    [Fact]
    public async Task Discovery_ShouldPreserveAddressesAndInterfacesWithoutInferringHttpRoute()
    {
        using var discovery = new MdnsDiscovery(startBrowse: false);
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var browser = typeof(MdnsDiscovery)
            .GetField("_serviceBrowser", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(discovery).Should().BeOfType<ServiceBrowser>().Subject;
        var added = typeof(ServiceBrowser)
            .GetField("ServiceAdded", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(browser).Should().BeOfType<EventHandler<ServiceAnnouncementEventArgs>>().Subject;

        var firstInterface = CreateInterface("{33333333-3333-3333-3333-333333333333}", 42);
        var secondInterface = CreateInterface("{44444444-4444-4444-4444-444444444444}", 43);
        var first = CreateAnnouncement(firstInterface);
        var second = CreateAnnouncement(secondInterface);
        added(browser, new ServiceAnnouncementEventArgs(first));
        added(browser, new ServiceAnnouncementEventArgs(first));
        added(browser, new ServiceAnnouncementEventArgs(second));

        await using var devices = discovery.DiscoverAsync(cancellation.Token).GetAsyncEnumerator();
        var found = new List<GpSharp.Data.DiscoveredDevice>();
        for (var index = 0; index < 4; index++)
        {
            (await devices.MoveNextAsync()).Should().BeTrue();
            found.Add(devices.Current);
        }

        found.Should().OnlyContain(device => device.ConnectionType == ConnectionType.Unknown);
        found.Should().ContainSingle(device => device.IpAddress == "fe80::1234%42" && device.DiscoveryInterface!.InterfaceId == firstInterface.Id);
        found.Should().ContainSingle(device => device.IpAddress == "fe80::1234%43" && device.DiscoveryInterface!.InterfaceId == secondInterface.Id);
        found.Should().ContainSingle(device => device.IpAddress == "192.0.2.1" && device.DiscoveryInterface!.InterfaceId == firstInterface.Id);
        found.Should().ContainSingle(device => device.IpAddress == "192.0.2.1" && device.DiscoveryInterface!.InterfaceId == secondInterface.Id);
        found.Should().OnlyContain(device => device.DiscoveryInterface!.LocalLink == LocalLinkType.Wifi);
    }

    [Fact]
    public async Task Discovery_ShouldEmitChangedAndReappearingEndpoints()
    {
        using var discovery = new MdnsDiscovery(startBrowse: false);
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var browser = typeof(MdnsDiscovery)
            .GetField("_serviceBrowser", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(discovery).Should().BeOfType<ServiceBrowser>().Subject;
        var added = GetHandler(browser, "ServiceAdded");
        var changed = GetHandler(browser, "ServiceChanged");
        var removed = GetHandler(browser, "ServiceRemoved");
        var announcement = CreateAnnouncement(CreateInterface("{33333333-3333-3333-3333-333333333333}", 42));
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Addresses), new List<IPAddress> { IPAddress.Parse("192.0.2.1") });
        added(browser, new ServiceAnnouncementEventArgs(announcement));
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Addresses), new List<IPAddress> { IPAddress.Parse("192.0.2.2") });
        changed(browser, new ServiceAnnouncementEventArgs(announcement));
        removed(browser, new ServiceAnnouncementEventArgs(announcement));
        added(browser, new ServiceAnnouncementEventArgs(announcement));

        await using var devices = discovery.DiscoverAsync(cancellation.Token).GetAsyncEnumerator();
        var addresses = new List<string>();
        for (var index = 0; index < 3; index++)
        {
            (await devices.MoveNextAsync()).Should().BeTrue();
            addresses.Add(devices.Current.IpAddress);
        }

        addresses.Should().Equal("192.0.2.1", "192.0.2.2", "192.0.2.2");
    }

    private static EventHandler<ServiceAnnouncementEventArgs> GetHandler(ServiceBrowser browser, string name) =>
        typeof(ServiceBrowser).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(browser).Should().BeOfType<EventHandler<ServiceAnnouncementEventArgs>>().Subject;

    private static NetworkInterface CreateInterface(string id, int index)
    {
        var ipv6 = new Mock<IPv6InterfaceProperties>();
        ipv6.SetupGet(properties => properties.Index).Returns(index);
        var ipProperties = new Mock<IPInterfaceProperties>();
        ipProperties.Setup(properties => properties.GetIPv6Properties()).Returns(ipv6.Object);
        var network = new Mock<NetworkInterface>();
        network.SetupGet(value => value.Id).Returns(id);
        network.SetupGet(value => value.Name).Returns("test");
        network.SetupGet(value => value.NetworkInterfaceType).Returns(NetworkInterfaceType.Wireless80211);
        network.Setup(value => value.GetIPProperties()).Returns(ipProperties.Object);
        return network.Object;
    }

    private static ServiceAnnouncement CreateAnnouncement(NetworkInterface network)
    {
        var announcement = new ServiceAnnouncement();
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Type), "_gopro-web._tcp");
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Instance), "camera");
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Hostname), "camera1234");
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Port), (ushort)80);
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.NetworkInterface), network);
        SetAnnouncementProperty(announcement, nameof(ServiceAnnouncement.Addresses), new List<IPAddress> { IPAddress.Parse("192.0.2.1"), IPAddress.Parse("fe80::1234") });
        return announcement;
    }

    private static void SetAnnouncementProperty(ServiceAnnouncement announcement, string name, object value) =>
        typeof(ServiceAnnouncement).GetProperty(name)!.SetValue(announcement, value);
}
