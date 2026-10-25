using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Moniter.Api.Data;
using Moniter.Api.Data.Health;
using Moniter.Api.Data.Repositories;
using Moniter.Api.Data.Tenancy;
using Moniter.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
builder.Services.AddDbContext<MoniterDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = DatabaseConnectionString.TryResolve(configuration);
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "PostgreSQL is not configured. Set ConnectionStrings:Moniter or MONITER_DATABASE_URL.");
    }

    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IOrganizationRepository, EfOrganizationRepository>();
builder.Services.AddScoped<IOrganizationMembershipRepository, EfOrganizationMembershipRepository>();
builder.Services.AddScoped<IDepartmentRepository, EfDepartmentRepository>();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" });

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
    app.MapGet("/openapi", () => Results.Ok(new
    {
        openapi = "3.0.3",
        info = new { title = "Moniter API", version = "v1" },
        paths = new Dictionary<string, object>
        {
            ["/"] = new { get = new { summary = "Service information" } },
            ["/health/live"] = new { get = new { summary = "Liveness health check" } },
            ["/health/ready"] = new { get = new { summary = "Database readiness health check" } }
        }
    })).WithName("DevelopmentOpenApi").WithTags("Foundation");
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

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
})
    .WithName("Readiness")
    .WithTags("Health");

app.Run();

public sealed record HealthResponse(string Status, string Service, string Version);

public partial class Program;
