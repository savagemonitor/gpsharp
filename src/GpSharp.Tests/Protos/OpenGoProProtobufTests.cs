using FluentAssertions;
using Google.Protobuf;
using GpSharp.Protos.OpenGoPro;
using Xunit;

namespace GpSharp.Tests.Protos;

public class OpenGoProProtobufTests
{
    [Fact]
    public void Requests_ShouldSerializeUsingOpenGoProWireFormat()
    {
        new RequestSetCameraName { Name = "GoPro" }.ToByteArray()
            .Should().Equal(0x0a, 0x05, 0x47, 0x6f, 0x50, 0x72, 0x6f);
        new RequestSetTurboActive { Active = true }.ToByteArray().Should().Equal(0x08, 0x01);
        new RequestGetCOHNStatus { RegisterCohnStatus = true }.ToByteArray().Should().Equal(0x08, 0x01);
        new RequestCreateCOHNCert { Override = true }.ToByteArray().Should().Equal(0x08, 0x01);
        new RequestClearCOHNCert().ToByteArray().Should().BeEmpty();
        new RequestGetLastCapturedMedia().ToByteArray().Should().BeEmpty();
    }

    [Fact]
    public void Responses_ShouldParseOpenGoProWireFormat()
    {
        var media = ResponseLastCapturedMedia.Parser.ParseFrom(
            new byte[] { 0x08, 0x01, 0x12, 0x0b, 0x0a, 0x03, 0x31, 0x30, 0x30, 0x12, 0x04, 0x61, 0x2e, 0x6d, 0x70 });
        media.Result.Should().Be(EnumResultGeneric.ResultSuccess);
        media.Media.Folder.Should().Be("100");
        media.Media.File.Should().Be("a.mp");

        var status = NotifyCOHNStatus.Parser.ParseFrom(new byte[] { 0x08, 0x01, 0x30, 0x01 });
        status.Status.Should().Be(EnumCOHNStatus.CohnProvisioned);
        status.Enabled.Should().BeTrue();

        var response = ResponseGeneric.Parser.ParseFrom(new byte[] { 0x08, 0x01 });
        response.Result.Should().Be(EnumResultGeneric.ResultSuccess);
    }
}
