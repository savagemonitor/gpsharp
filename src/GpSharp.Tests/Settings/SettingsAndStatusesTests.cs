using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GpSharp.GoPro;
using GpSharp.GoPro.Connectors;
using GpSharp.GoPro.Settings;
using GpSharp.GoPro.Statuses;
using Moq;
using Xunit;

namespace GpSharp.Tests.Settings;

public class SettingsAndStatusesTests
{
    private const string CameraState = "{\"status\":{\"10\":1,\"70\":64},\"settings\":{\"2\":9,\"3\":8,\"236\":1}}";

    [Fact]
    public async Task GetSettingValueAsync_ShouldReadNumericSetting()
    {
        var connection = CreateConnection();
        connection.Setup(c => c.GetAsync("/gopro/camera/setting?setting=2", It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"option\":9}");
        var setting = new SettingsContainer(connection.Object).VideoResolution;

        var result = await setting.GetValueAsync<int>();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(9);
        connection.Verify(c => c.GetAsync("/gopro/camera/setting?setting=2", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSettingAsync_ShouldSupportDynamicProtocolIdsAndRawJsonValues()
    {
        var connection = CreateConnection();
        connection.Setup(c => c.GetAsync("/gopro/camera/setting?setting=236", It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"option\":1}");
        var setting = new SettingsContainer(connection.Object).GetSetting(236);

        var result = await setting.GetValueAsync<JsonElement>();

        result.IsSuccess.Should().BeTrue();
        result.Value.GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task StateRead_ShouldPreserveSettingAndStatusIdsAboveByteRange()
    {
        const string stateJson =
            "{\"status\":{\"10\":1,\"270\":2},\"settings\":{\"256\":1,\"267\":2,\"275\":3}}";
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/camera/state", It.IsAny<CancellationToken>()))
            .ReturnsAsync(stateJson);

        var result = await new StatusesContainer(connection.Object).GetCameraStateAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Settings.Keys.Should().ContainInOrder(256, 267, 275);
        result.Value.Settings[256].GetInt32().Should().Be(1);
        result.Value.Settings[267].GetInt32().Should().Be(2);
        result.Value.Settings[275].GetInt32().Should().Be(3);
        result.Value.Statuses[270].GetInt32().Should().Be(2);
    }

    [Fact]
    public async Task GetStatusAsync_ShouldLookUpStatusIdAboveByteRange()
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/camera/state", It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"status\":{\"270\":2},\"settings\":{\"256\":1}}");

        var result = await new StatusesContainer(connection.Object).GetStatus(270).GetValueAsync<int>();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2);
    }

    [Fact]
    public async Task GetSettingAsync_ShouldAcceptProtocolIdAboveByteRange()
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/camera/setting?setting=268", It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"option\":4}");

        var result = await new SettingsContainer(connection.Object).GetSetting(268).GetValueAsync<int>();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(4);
    }

    [Fact]
    public async Task SetSettingValueAsync_ShouldUseOpenGoProSettingAndOptionParameters()
    {
        var connection = new Mock<IHttpConnection>();
        const string path = "/gopro/camera/setting?setting=2&option=9";
        connection.Setup(c => c.GetAsync(path, It.IsAny<CancellationToken>())).ReturnsAsync(string.Empty);
        var setting = new SettingsContainer(connection.Object).VideoResolution;

        var result = await setting.SetValueAsync(9);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(GpSharp.Core.Unit.Value);
        connection.Verify(c => c.GetAsync(path, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetSettingValueAsync_ShouldRejectNonNumericAndNegativeOptionsWithoutRequest()
    {
        var connection = new Mock<IHttpConnection>();
        var setting = new SettingsContainer(connection.Object).VideoResolution;

        var negative = await setting.SetValueAsync(-1);
        var nonNumeric = await setting.SetValueAsync("9");

        negative.IsSuccess.Should().BeFalse();
        negative.Error.Should().BeOfType<ArgumentOutOfRangeException>();
        nonNumeric.IsSuccess.Should().BeFalse();
        nonNumeric.Error.Should().BeOfType<ArgumentException>();
        connection.Verify(c => c.GetAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SettingAndStatusReads_ShouldReturnFailuresForMissingIdsAndMalformedState()
    {
        var missingConnection = new Mock<IHttpConnection>();
        missingConnection.Setup(c => c.GetAsync("/gopro/camera/state", It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"status\":{\"10\":1},\"settings\":{\"3\":8}}");
        var statuses = new StatusesContainer(missingConnection.Object);
        var missing = await statuses.GetStatus(70).GetValueAsync<int>();
        missing.IsSuccess.Should().BeFalse();
        missing.Error.Should().BeOfType<KeyNotFoundException>();

        var malformedConnection = new Mock<IHttpConnection>();
        malformedConnection.Setup(c => c.GetAsync("/gopro/camera/setting?setting=2", It.IsAny<CancellationToken>())).ReturnsAsync("{");
        var malformed = await new SettingsContainer(malformedConnection.Object)
            .VideoResolution.GetValueAsync<int>();
        malformed.IsSuccess.Should().BeFalse();
        malformed.Error.Should().BeAssignableTo<JsonException>();
    }

    [Fact]
    public async Task SettingWrites_ShouldReturnHttpFailures()
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/camera/setting?setting=2&option=9", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Camera unavailable."));

        var result = await new SettingsContainer(connection.Object).VideoResolution.SetValueAsync(9);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeOfType<HttpRequestException>();
    }

    [Fact]
    public async Task SettingWrites_ShouldExposeSupportedOptionsFromCameraRejection()
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync(
                "/gopro/camera/setting?setting=2&option=9", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpResponseException(HttpStatusCode.Forbidden,
                "{\"error\":3,\"setting_id\":2,\"option_id\":9,\"supported_options\":[{\"id\":1,\"display_name\":\"1080p\"},{\"id\":9,\"display_name\":\"4K\"}]}"));

        var result = await new SettingsContainer(connection.Object).VideoResolution.SetValueAsync(9);

        var error = result.Error.Should().BeOfType<SettingRejectedException>().Subject;
        error.ErrorCode.Should().Be(3);
        error.SettingId.Should().Be(2);
        error.OptionId.Should().Be(9);
        error.SupportedOptions.Should().ContainInOrder(
            new SettingOption(1, "1080p"),
            new SettingOption(9, "4K"));
    }

    [Fact]
    public async Task StateRead_ShouldReturnSettingsAndStatusesAsProtocolIdMaps()
    {
        var connection = CreateConnection();

        var result = await new StatusesContainer(connection.Object).GetCameraStateAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Settings[2].GetInt32().Should().Be(9);
        result.Value.Statuses[10].GetInt32().Should().Be(1);
        result.Value.Settings.Should().ContainKey(236);
    }

    [Fact]
    public async Task GetStatusValueAsync_ShouldReadTypedStatusFromCameraState()
    {
        var connection = CreateConnection();

        var result = await new StatusesContainer(connection.Object).IsEncoding.GetValueAsync<int>();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);
        connection.Verify(c => c.GetAsync("/gopro/camera/state", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StateRead_ShouldPropagateCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/camera/state", cancellation.Token))
            .ThrowsAsync(new OperationCanceledException(cancellation.Token));
        var statuses = new StatusesContainer(connection.Object);

        Func<Task> read = async () => await statuses.GetCameraStateAsync(cancellation.Token);

        await read.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task SettingRead_ShouldPropagateCancellationToken()
    {
        using var cancellation = new CancellationTokenSource();
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/camera/state", cancellation.Token))
            .ReturnsAsync(CameraState);
        connection.Setup(c => c.GetAsync("/gopro/camera/setting?setting=2", cancellation.Token))
            .ReturnsAsync("{\"option\":9}");
        var setting = new SettingsContainer(connection.Object).VideoResolution;

        var result = await setting.GetValueAsync<int>(cancellation.Token);

        result.Value.Should().Be(9);
        connection.Verify(c => c.GetAsync("/gopro/camera/setting?setting=2", cancellation.Token), Times.Once);
    }

    [Fact]
    public void Containers_ShouldRejectNegativeSettingOrStatusIds()
    {
        var connection = new Mock<IHttpConnection>().Object;
        var settings = new SettingsContainer(connection);
        var statuses = new StatusesContainer(connection);

        Action setting = () => settings.GetSetting(-1);
        Action status = () => statuses.GetStatus(-1);

        setting.Should().Throw<ArgumentOutOfRangeException>();
        status.Should().Throw<ArgumentOutOfRangeException>();
    }

    private static Mock<IHttpConnection> CreateConnection()
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/camera/state", It.IsAny<CancellationToken>())).ReturnsAsync(CameraState);
        return connection;
    }
}
