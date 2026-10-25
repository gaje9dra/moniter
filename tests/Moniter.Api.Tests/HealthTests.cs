using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
namespace Moniter.Api.Tests;
public sealed class HealthTests : IClassFixture<WebApplicationFactory<Program>> {
 readonly HttpClient client;
 public HealthTests(WebApplicationFactory<Program> factory)=>client=factory.WithWebHostBuilder(b=>b.UseSetting("ASPNETCORE_ENVIRONMENT","Development")).CreateClient();
 [Fact] public async Task Liveness_returns_status_and_version() { var r=await client.GetAsync("/health/live"); Assert.Equal(HttpStatusCode.OK,r.StatusCode); var body=await r.Content.ReadFromJsonAsync<HealthBody>(); Assert.NotNull(body); Assert.Equal("ok",body.Status); Assert.Equal("Moniter.Api",body.Service); Assert.False(string.IsNullOrWhiteSpace(body.Version)); }
 [Fact] public async Task Safe_correlation_id_is_returned() { using var q=new HttpRequestMessage(HttpMethod.Get,"/health/live"); q.Headers.Add("X-Correlation-ID","test-correlation_01"); var r=await client.SendAsync(q); Assert.Equal("test-correlation_01",r.Headers.GetValues("X-Correlation-ID").Single()); }
 [Fact] public async Task Invalid_correlation_id_is_replaced() { using var q=new HttpRequestMessage(HttpMethod.Get,"/health/live"); q.Headers.Add("X-Correlation-ID","bad id"); var r=await client.SendAsync(q); Assert.NotEqual("bad id",r.Headers.GetValues("X-Correlation-ID").Single()); }
 private sealed record HealthBody(string Status,string Service,string Version);
}
