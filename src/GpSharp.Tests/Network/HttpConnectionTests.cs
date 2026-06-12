using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Network;
using Xunit;
using FluentAssertions;

namespace GpSharp.Tests.Network;

public class HttpConnectionTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnContent_WhenSuccessful()
    {
        // Arrange
        var baseUrl = "http://192.168.1.100:8080/";
        using var handler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"status\":\"ok\"}")
        });
        var client = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        var connection = new HttpConnection(baseUrl, client);

        // Act
        var result = await connection.GetAsync("some/path");

        // Assert
        result.Should().Be("{\"status\":\"ok\"}");
    }

    [Fact]
    public async Task PostAsync_ShouldReturnContent_WhenSuccessful()
    {
        // Arrange
        var baseUrl = "http://192.168.1.100:8080/";
        using var handler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"status\":\"ok\"}")
        });
        var client = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        var connection = new HttpConnection(baseUrl, client);

        // Act
        var result = await connection.PostAsync("some/path", new { key = "value" });

        // Assert
        result.Should().Be("{\"status\":\"ok\"}");
    }

    [Fact]
    public async Task PutAsync_ShouldReturnContent_WhenSuccessful()
    {
        // Arrange
        var baseUrl = "http://192.168.1.100:8080/";
        using var handler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"status\":\"ok\"}")
        });
        var client = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        var connection = new HttpConnection(baseUrl, client);

        // Act
        var result = await connection.PutAsync("some/path", new { key = "value" });

        // Assert
        result.Should().Be("{\"status\":\"ok\"}");
    }

    [Fact]
    public async Task GetAsync_ShouldThrowException_WhenFailed()
    {
        // Arrange
        var baseUrl = "http://192.168.1.100:8080/";
        using var handler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("Error")
        });
        var client = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        var connection = new HttpConnection(baseUrl, client);

        // Act
        Func<Task> act = async () => await connection.GetAsync("some/path");

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;
    public MockHttpMessageHandler(HttpResponseMessage response) => _response = response;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_response);
    }
}
