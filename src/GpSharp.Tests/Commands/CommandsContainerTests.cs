using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GpSharp.Core;
using GpSharp.GoPro;
using GpSharp.GoPro.Commands;
using GpSharp.GoPro.Connectors;
using Moq;
using Xunit;

namespace GpSharp.Tests.Commands;

public class CommandsContainerTests
{
    [Theory]
    [InlineData(true, "/gopro/camera/shutter/start")]
    [InlineData(false, "/gopro/camera/shutter/stop")]
    public async Task SetShutterAsync_ShouldUseOpenGoProHttpEndpoint(bool on, string expectedPath)
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync(expectedPath)).ReturnsAsync(string.Empty);
        var commands = new CommandsContainer(connection.Object);

        var result = await commands.SetShutterAsync(on);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(Unit.Value);
        connection.Verify(c => c.GetAsync(expectedPath), Times.Once);
    }

    [Fact]
    public async Task GetMediaListAsync_ShouldDeserializeOpenGoProMediaList()
    {
        const string response = """
            {
              "media": [
                {
                  "d": "100GOPRO",
                  "fs": [
                    {
                      "n": "GOPR0001.MP4",
                      "cre": "2024-01-02 03:04:05",
                      "mod": "2024-01-02 03:04:05",
                      "s": 1024,
                      "m": 0
                    }
                  ]
                }
              ]
            }
            """;
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/media/list")).ReturnsAsync(response);
        var commands = new CommandsContainer(connection.Object);

        var result = await commands.GetMediaListAsync();

        result.IsSuccess.Should().BeTrue();
        var mediaList = result.Value!;
        mediaList.Media.Should().ContainSingle();
        mediaList.Media[0].Directory.Should().Be("100GOPRO");
        mediaList.Media[0].Files.Should().ContainSingle();
        mediaList.Media[0].Files[0].Name.Should().Be("GOPR0001.MP4");
        mediaList.Media[0].Files[0].Size.Should().Be(1024);
        mediaList.Media[0].Files[0].AdditionalProperties!["m"].GetInt32().Should().Be(0);
        connection.Verify(c => c.GetAsync("/gopro/media/list"), Times.Once);
    }

    [Fact]
    public async Task GetMediaListAsync_ShouldSurfaceMalformedJson()
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/media/list")).ReturnsAsync("{");
        var commands = new CommandsContainer(connection.Object);

        var result = await commands.GetMediaListAsync();

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeOfType<JsonException>();
    }

    [Theory]
    [InlineData("1024")]
    [InlineData("\"1024\"")]
    public async Task GetMediaListAsync_ShouldReadNumberOrNumericStringSize(string jsonSize)
    {
        var response = "{\"media\":[{\"d\":\"100GOPRO\",\"fs\":[{\"n\":\"GOPR0001.MP4\",\"s\":" + jsonSize + "}]}]}";
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/media/list")).ReturnsAsync(response);

        var result = await new CommandsContainer(connection.Object).GetMediaListAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Media[0].Files[0].Size.Should().Be(1024);
    }

    [Theory]
    [InlineData("\"not-a-number\"")]
    [InlineData("\"9223372036854775808\"")]
    public async Task GetMediaListAsync_ShouldRejectInvalidSize(string jsonSize)
    {
        var response = "{\"media\":[{\"d\":\"100GOPRO\",\"fs\":[{\"n\":\"GOPR0001.MP4\",\"s\":" + jsonSize + "}]}]}";
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/media/list")).ReturnsAsync(response);

        var result = await new CommandsContainer(connection.Object).GetMediaListAsync();

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeOfType<JsonException>();
    }

    [Fact]
    public async Task GetMediaListAsync_ShouldReturnFailureWhenHttpRequestFails()
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/media/list"))
            .ThrowsAsync(new HttpRequestException("Camera unavailable."));
        var commands = new CommandsContainer(connection.Object);

        var result = await commands.GetMediaListAsync();

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeOfType<HttpRequestException>();
    }

    [Fact]
    public async Task SetShutterAsync_ShouldReturnFailureWhenHttpRequestFails()
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/camera/shutter/start"))
            .ThrowsAsync(new HttpRequestException("Camera unavailable."));
        var commands = new CommandsContainer(connection.Object);

        var result = await commands.SetShutterAsync(true);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeOfType<HttpRequestException>();
    }

    [Fact]
    public async Task SettingsAndStatuses_ShouldExplainThatTheyRequireBle()
    {
        var goPro = new GpSharp.GoPro.GoPro(new Mock<IHttpConnection>().Object);

        Func<Task> readSetting = async () =>
            await goPro.Settings.VideoResolution.GetValueAsync<int>();
        Func<Task> readStatus = async () =>
            await goPro.Statuses.InternalBatteryPercentage.GetValueAsync<int>();

        await readSetting.Should().ThrowAsync<NotSupportedException>()
            .WithMessage("*requires BLE support*");
        await readStatus.Should().ThrowAsync<NotSupportedException>()
            .WithMessage("*requires BLE support*");
    }
}
