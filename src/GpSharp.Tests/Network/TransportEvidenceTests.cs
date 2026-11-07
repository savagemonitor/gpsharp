using System;
using System.Net;
using System.Net.NetworkInformation;
using FluentAssertions;
using GpSharp.Network;
using Xunit;

namespace GpSharp.Tests.Network;

public class TransportEvidenceTests
{
    [Theory]
    [InlineData(true, false, "UsbNcm", "USB\\VID_2672&PID_005A&MI_00\\child", "USB\\VID_2672&PID_005A\\C1234567891234", "C1234567891234", true)]
    [InlineData(true, false, "UsbNcm", "USB\\VID_2672&PID_005A&MI_00\\child", "USB\\VID_2672&PID_005A\\C1234567891234", "c1234567891234", true)]
    [InlineData(true, false, "UsbNcm", "USB\\VID_2672&PID_005A&MI_00\\child", "USB\\VID_2672&PID_005A\\C1234567891234", "other1234", false)]
    [InlineData(true, false, "UsbNcm", "USB\\VID_1234&PID_005A&MI_00\\child", "USB\\VID_1234&PID_005A\\C1234567891234", "C1234567891234", false)]
    [InlineData(true, false, "UsbNcm", "USB\\VID_2672&PID_005B&MI_00\\child", "USB\\VID_2672&PID_005A\\C1234567891234", "C1234567891234", false)]
    [InlineData(true, false, "Other", "USB\\VID_2672&PID_005A&MI_00\\child", "USB\\VID_2672&PID_005A\\C1234567891234", "C1234567891234", false)]
    [InlineData(true, true, "UsbNcm", "USB\\VID_2672&PID_005A&MI_00\\child", "USB\\VID_2672&PID_005A\\C1234567891234", "C1234567891234", false)]
    [InlineData(false, false, "UsbNcm", "USB\\VID_2672&PID_005A&MI_00\\child", "USB\\VID_2672&PID_005A\\C1234567891234", "C1234567891234", false)]
    [InlineData(true, false, "UsbNcm", "USB\\VID_2672&PID_005A&MI_00\\child", "USB\\VID_2672&PID_005A\\1234", "1234", false)]
    public void DirectUsbAssociation_RequiresStrongIdentityEvidence(
        bool hardware, bool virtualAdapter, string service, string child, string parent, string hostname, bool expected)
    {
        WindowsUsbAssociation.Matches(hardware, virtualAdapter, service, child, parent, hostname).Should().Be(expected);
    }

    [Fact]
    public void DirectUsbAssociation_DoesNotAcceptMultipleMatchingAdapters()
    {
        var expected = Guid.NewGuid();
        var other = Guid.NewGuid();
        WindowsUsbAssociation.Select(new[] { expected, other }, expected).Should().Be(UsbAssociationStatus.Ambiguous);
        WindowsUsbAssociation.Select(new[] { expected }, expected).Should().Be(UsbAssociationStatus.Matched);
        WindowsUsbAssociation.Select(new[] { other }, expected).Should().Be(UsbAssociationStatus.NoMatch);
    }

    [Fact]
    public void LocalLinkClassification_DoesNotInferCameraTopology()
    {
        var result = TransportEvidenceResolver.ClassifyLocalLink(NetworkInterfaceType.Wireless80211);
        result.Should().Be(LocalLinkType.Wifi);
        TransportEvidenceResolver.ClassifyLocalLink(NetworkInterfaceType.Ethernet).Should().Be(LocalLinkType.Unknown);
    }

    [Fact]
    public void IPv6LinkLocalAddress_UsesDiscoveryInterfaceScope()
    {
        var address = IPAddress.Parse("fe80::1234");
        MdnsDiscovery.ScopeAddress(address, 42).ToString().Should().Be("fe80::1234%42");
        MdnsDiscovery.ScopeAddress(IPAddress.Parse("fe80::1234%99"), 42).ToString().Should().Be("fe80::1234%42");
        MdnsDiscovery.ScopeAddress(IPAddress.Parse("192.0.2.1"), 42).ToString().Should().Be("192.0.2.1");
    }

    [Fact]
    public void NonWindows_UsesUnknownUsbAssociation()
    {
        TransportEvidenceResolver.ResolveUsbAssociation(false, null, "C1234567891234").Status
            .Should().Be(UsbAssociationStatus.Unavailable);
    }

    [Fact]
    public void Windows_InvalidInterfaceIdentifier_ReportsDiagnostic()
    {
        if (!OperatingSystem.IsWindows()) return;

        var result = TransportEvidenceResolver.ResolveUsbAssociation(true, "not-a-guid", "C1234567891234");
        result.Status.Should().Be(UsbAssociationStatus.Error);
        result.Diagnostic.Should().NotBeNullOrWhiteSpace();
    }
}
