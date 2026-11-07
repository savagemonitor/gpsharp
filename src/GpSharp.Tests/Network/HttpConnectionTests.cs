using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.GoPro.Connectors;
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
        using var handler = new MockHttpMessageHandler(() => new HttpResponseMessage
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
        handler.SendCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAsync_ShouldRetainForbiddenResponseBodyWithoutRetrying()
    {
        const string body = "{\"error\":3,\"supported_options\":[]}";
        var baseUrl = "http://192.168.1.100:8080/";
        using var handler = new MockHttpMessageHandler(() => new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Forbidden,
            Content = new StringContent(body)
        });
        var client = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        var connection = new HttpConnection(baseUrl, client);

        Func<Task> act = async () => await connection.GetAsync("setting");

        var error = await act.Should().ThrowAsync<HttpResponseException>();
        error.Which.ResponseStatusCode.Should().Be(HttpStatusCode.Forbidden);
        error.Which.ResponseBody.Should().Be(body);
        handler.SendCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAsync_WithCancellationToken_ShouldCancelPendingHttpRequest()
    {
        using var cancellation = new CancellationTokenSource();
        var baseUrl = "http://192.168.1.100:8080/";
        using var handler = new CancellationHttpMessageHandler();
        var client = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        var connection = new HttpConnection(baseUrl, client);

        var request = connection.GetAsync("setting", cancellation.Token);
        await handler.RequestStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancellation.Cancel();
        Func<Task> act = async () => await request;

        await act.Should().ThrowAsync<OperationCanceledException>();
        handler.CancellationObserved.Should().BeTrue();
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpResponseMessage> _responseFactory;
    public int SendCount { get; private set; }
    public MockHttpMessageHandler(HttpResponseMessage response) => _responseFactory = () => response;
    public MockHttpMessageHandler(Func<HttpResponseMessage> responseFactory) => _responseFactory = responseFactory;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        SendCount++;
        return Task.FromResult(_responseFactory());
    }
}

public class CancellationHttpMessageHandler : HttpMessageHandler
{
    public TaskCompletionSource RequestStarted { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    public bool CancellationObserved { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        RequestStarted.TrySetResult();
        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            throw new InvalidOperationException("The HTTP request was expected to be cancelled.");
        }
        catch (OperationCanceledException)
        {
            CancellationObserved = cancellationToken.IsCancellationRequested;
            throw;
        }
    }
}
