using System.Diagnostics;
using System.Text.Json;
using Moniter.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var suppliedId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
    var correlationId = CorrelationIds.TryNormalize(suppliedId) ?? ActivityTraceId.CreateRandom().ToString();
    context.TraceIdentifier = correlationId;
    context.Response.Headers["X-Correlation-ID"] = correlationId;

    using (app.Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            app.Logger.LogError(
                "Unhandled request failure. CorrelationId={CorrelationId}, ExceptionType={ExceptionType}",
                correlationId,
                ex.GetType().Name);
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                new
                {
                    type = "https://httpstatuses.com/500",
                    title = "An unexpected error occurred.",
                    status = 500,
                    correlationId
                });
        }
    }
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new
{
    service = "Moniter.Api",
    version = AppVersion.Value,
    status = "ok"
}))
.WithName("ServiceInfo")
.WithTags("Foundation");

app.MapGet("/health/live", () => Results.Ok(new HealthResponse("ok", "Moniter.Api", AppVersion.Value)))
    .WithName("Liveness")
    .WithTags("Health");

app.MapHealthChecks("/health/ready")
    .WithName("Readiness")
    .WithTags("Health");

app.Run();

public sealed record HealthResponse(string Status, string Service, string Version);

public partial class Program;
