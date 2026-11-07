using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GpSharp.Data;
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
    public void ProtocolCatalog_ShouldMapNamesAndDocumentedOptionsWhileKeepingUnknownsExplicit()
    {
        var imageTuning = OpenGoProProtocolCatalog.GetSetting(256);
        var videoResolution = OpenGoProProtocolCatalog.GetSetting(2);
        using var knownValue = JsonDocument.Parse("9");
        using var unknownValue = JsonDocument.Parse("999");
        using var stringValue = JsonDocument.Parse("\"9\"");

        imageTuning.Should().NotBeNull();
        imageTuning!.Name.Should().Be("Image Tuning");
        imageTuning.DocumentationUrl.Should().Contain("#setting-256");
        videoResolution.Should().NotBeNull();
        videoResolution!.SdkSourceUrl.Should().Contain("0f963572611c4410a15678531e9681a6ff874edb");
        videoResolution.FindOption(knownValue.RootElement)!.Name.Should().Be("NUM_1080");
        knownValue.RootElement.GetInt32().Should().Be(9);
        videoResolution.FindOption(unknownValue.RootElement).Should().BeNull();
        videoResolution.FindOption(stringValue.RootElement).Should().BeNull();
        using var easyModeValue = JsonDocument.Parse("159");
        OpenGoProProtocolCatalog.GetSetting(176)!.FindOption(easyModeValue.RootElement)!
            .Name.Should().Be("NUM_100_4X_SUPER_SLO_MO_SPEED_2_7K_4_3_V2_");
        OpenGoProProtocolCatalog.GetSetting(257)!.Name.Should().Be("Subject Tracking");
        OpenGoProProtocolCatalog.GetSetting(273)!.Name.Should().Be("Output");
        OpenGoProProtocolCatalog.GetSetting(258).Should().BeNull();
        OpenGoProProtocolCatalog.GetSettingDisplayName(258).Should().Be("Uncatalogued setting (ID 258)");
        OpenGoProProtocolCatalog.GetStatus(70)!.ValueKind.Should().Be("Unsigned 8-bit integer");
        OpenGoProProtocolCatalog.GetStatus(91).Should().BeNull();
    }

    [Fact]
    public void ProtocolCatalog_ShouldCoverPublishedIdsInSanitizedCameraSnapshot()
    {
        int[] documentedSettings =
        [
            2, 3, 5, 43, 59, 83, 88, 91, 108, 121, 122, 123, 125, 134, 135, 156, 157,
            167, 168, 171, 172, 178, 179, 182, 183, 189, 192, 216, 223, 239, 256, 257, 273,
        ];
        int[] undocumentedSettings =
        [
            6, 13, 41, 42, 47, 54, 62, 64, 84, 86, 103, 105, 111, 112, 115, 116, 117, 118,
            126, 129, 130, 131, 132, 139, 145, 146, 147, 153, 154, 159, 198, 199, 203, 205,
            206, 208, 209, 210, 211, 212, 213, 214, 218, 221, 222, 226, 258, 259, 260, 261,
            262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 274, 275,
        ];
        int[] documentedStatuses =
        [
            1, 2, 6, 8, 9, 10, 11, 13, 17, 19, 20, 22, 23, 24, 27, 29, 30, 31, 33, 34, 35,
            38, 39, 41, 42, 45, 49, 54, 55, 56, 58, 59, 60, 65, 66, 67, 68, 69, 70, 74, 75,
            76, 77, 81, 82, 83, 85, 86, 88, 89, 96, 97, 98, 101, 102, 103, 106, 107, 108,
            110, 111, 112, 113, 114, 115, 116, 117, 118, 122, 123,
        ];
        int[] undocumentedStatuses = [3, 4, 14, 61, 90, 91, 109, 119];

        foreach (var id in documentedSettings)
        {
            OpenGoProProtocolCatalog.GetSetting(id).Should().NotBeNull($"setting {id} is documented");
        }

        foreach (var id in undocumentedSettings)
        {
            OpenGoProProtocolCatalog.GetSetting(id).Should().BeNull($"setting {id} is not in the published catalog");
        }

        foreach (var id in documentedStatuses)
        {
            OpenGoProProtocolCatalog.GetStatus(id).Should().NotBeNull($"status {id} is documented");
        }

        foreach (var id in undocumentedStatuses)
        {
            OpenGoProProtocolCatalog.GetStatus(id).Should().BeNull($"status {id} is not in the published catalog");
        }
    }

    [Fact]
    public async Task UnknownCameraStateIds_ShouldPreserveRawValuesAndUseExplicitFallbackNames()
    {
        var connection = new Mock<IHttpConnection>();
        connection.Setup(c => c.GetAsync("/gopro/camera/state", It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"status\":{\"91\":\"future-value\"},\"settings\":{\"258\":999}}");

        var result = await new StatusesContainer(connection.Object).GetCameraStateAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Settings[258].GetInt32().Should().Be(999);
        result.Value.Statuses[91].GetString().Should().Be("future-value");
        OpenGoProProtocolCatalog.GetSettingDisplayName(258).Should().Be("Uncatalogued setting (ID 258)");
        OpenGoProProtocolCatalog.GetStatusDisplayName(91).Should().Be("Uncatalogued status (ID 91)");
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
