using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Moniter.Api.Tests;

public sealed class HealthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
            builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Development")).CreateClient();
    }

    [Fact]
    public async Task Liveness_returns_status_and_version()
    {
        var response = await _client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthBody>();
        Assert.NotNull(body);
        Assert.Equal("ok", body.Status);
        Assert.Equal("Moniter.Api", body.Service);
        Assert.False(string.IsNullOrWhiteSpace(body.Version));
    }

    [Fact]
    public async Task Response_contains_safe_correlation_id()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        request.Headers.Add("X-Correlation-ID", "test-correlation_01");
        var response = await _client.SendAsync(request);

        Assert.Equal("test-correlation_01", response.Headers.GetValues("X-Correlation-ID").Single());
    }

    [Fact]
    public async Task Invalid_correlation_id_is_replaced()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        request.Headers.Add("X-Correlation-ID", "bad id");
        var response = await _client.SendAsync(request);

        Assert.NotEqual("bad id", response.Headers.GetValues("X-Correlation-ID").Single());
    }

    private sealed record HealthBody(string Status, string Service, string Version);
}
