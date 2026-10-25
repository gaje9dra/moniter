using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moniter.Api.Data;
using Moniter.Api.Data.Health;
using Moniter.Api.Data.Repositories;
using Moniter.Api.Data.Tenancy;
using Moniter.Api.Infrastructure;
using Moniter.Api.Security;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

var identityProvider = IdentityProviderConfiguration.FromConfiguration(builder.Configuration);
if (builder.Environment.IsProduction() && !identityProvider.RequireHttpsMetadata)
{
    throw new InvalidOperationException(
        "HTTPS metadata validation cannot be disabled in production.");
}

builder.Services.AddSingleton(identityProvider);
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = identityProvider.Authority;
        options.Audience = identityProvider.Audience;
        options.RequireHttpsMetadata = identityProvider.RequireHttpsMetadata;
        options.MapInboundClaims = false;
        options.IncludeErrorDetails = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = tokenContext =>
            {
                var principal = tokenContext.Principal;
                var issuer = tokenContext.SecurityToken?.Issuer;
                var subjects = principal?.FindAll("sub")
                    .Select(claim => claim.Value)
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .ToArray() ?? Array.Empty<string>();
                var authenticatedIdentity = principal?.Identities.FirstOrDefault(identity => identity.IsAuthenticated);

                if (principal is null ||
                    authenticatedIdentity is null ||
                    subjects.Length != 1 ||
                    subjects[0].Length > 255 ||
                    string.IsNullOrWhiteSpace(issuer) ||
                    issuer.Length > 255)
                {
                    tokenContext.Fail("The access token is missing required identity claims.");
                    return Task.CompletedTask;
                }

                MoniterClaimTypes.RemoveServerManagedClaims(principal);
                authenticatedIdentity.AddClaim(new System.Security.Claims.Claim(
                    MoniterClaimTypes.TrustedIssuer,
                    issuer));

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(MoniterAuthorizationPolicies.Configure);
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, SecurityAuditAuthorizationResultHandler>();

builder.Services.AddScoped<IOrganizationContextResolver, EfOrganizationContextResolver>();
builder.Services.AddScoped<IOrganizationRepository, EfOrganizationRepository>();
builder.Services.AddScoped<IOrganizationMembershipRepository, EfOrganizationMembershipRepository>();
builder.Services.AddScoped<IDepartmentRepository, EfDepartmentRepository>();

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" })
    .AddCheck<IdentityProviderHealthCheck>("identity-provider-configuration", tags: new[] { "ready" });

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
            if (app.Environment.IsDevelopment())
            {
                // Preserve exception details only in development diagnostics. Production
                // logs keep the established sanitized record and never return internals.
                app.Logger.LogError(
                    ex,
                    "Unhandled development request failure. CorrelationId={CorrelationId}",
                    correlationId);
            }
            else
            {
                app.Logger.LogError(
                    "Unhandled request failure. CorrelationId={CorrelationId}, ExceptionType={ExceptionType}",
                    correlationId,
                    ex.GetType().Name);
            }
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

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<OrganizationContextMiddleware>();
app.UseAuthorization();

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
            ["/health/ready"] = new { get = new { summary = "Database and authentication readiness check" } },
            ["/api/v1/me"] = new { get = new { summary = "Authenticated organization context" } },
            ["/api/v1/organizations/current"] = new { get = new { summary = "Current authorized organization" } },
            ["/api/v1/organization-memberships"] = new { get = new { summary = "Privileged organization membership listing" } }
        }
    })).WithName("DevelopmentOpenApi").WithTags("Foundation").AllowAnonymous();
}

app.MapGet("/", () => Results.Ok(new
{
    service = "Moniter.Api",
    version = AppVersion.Value,
    status = "ok"
}))
.WithName("ServiceInfo")
.WithTags("Foundation")
.AllowAnonymous();

app.MapGet("/health/live", () => Results.Ok(new HealthResponse("ok", "Moniter.Api", AppVersion.Value)))
    .WithName("Liveness")
    .WithTags("Health")
    .AllowAnonymous();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
})
    .WithName("Readiness")
    .WithTags("Health")
    .AllowAnonymous();

app.MapGet("/api/v1/me", (HttpContext context) =>
{
    var userIdentityId = context.User.FindFirst(MoniterClaimTypes.UserIdentityId)?.Value;
    var organizationId = context.User.FindFirst(MoniterClaimTypes.OrganizationId)?.Value;
    var role = context.User.FindFirst(MoniterClaimTypes.OrganizationRole)?.Value;

    return Results.Ok(new
    {
        userIdentityId,
        organizationId,
        role,
        multiFactorAuthenticated = context.User.HasClaim(MoniterClaimTypes.MfaAssurance, "true")
    });
})
    .RequireAuthorization(MoniterAuthorizationPolicies.OrganizationMember)
    .WithName("CurrentIdentity")
    .WithTags("Identity");

app.MapGet("/api/v1/organizations/current", async Task<IResult> (
    IOrganizationRepository organizationRepository,
    CancellationToken cancellationToken) =>
{
    var organization = await organizationRepository.GetCurrentAsync(cancellationToken);
    if (organization is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new
    {
        id = organization.Id,
        name = organization.Name,
        slug = organization.Slug,
        status = organization.Status.ToString()
    });
})
    .RequireAuthorization(MoniterAuthorizationPolicies.OrganizationMember)
    .WithName("CurrentOrganization")
    .WithTags("Organizations");

app.MapGet("/api/v1/organization-memberships", async Task<IResult> (
    HttpContext context,
    IOrganizationMembershipRepository membershipRepository,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    var organizationId = context.User.FindFirst(MoniterClaimTypes.OrganizationId)?.Value;
    var userIdentityId = context.User.FindFirst(MoniterClaimTypes.UserIdentityId)?.Value;
    logger.LogInformation(
        "security_audit event=organization_memberships_read organization_id={OrganizationId} user_identity_id={UserIdentityId} correlation_id={CorrelationId}",
        organizationId,
        userIdentityId,
        context.TraceIdentifier);

    var memberships = await membershipRepository.ListCurrentAsync(cancellationToken);
    return Results.Ok(memberships.Select(membership => new
    {
        id = membership.Id,
        userIdentityId = membership.UserIdentityId,
        email = membership.UserIdentity.Email,
        displayName = membership.UserIdentity.DisplayName,
        role = membership.Role.ToString(),
        status = membership.Status.ToString(),
        createdAt = membership.CreatedAt
    }));
})
    .RequireAuthorization(MoniterAuthorizationPolicies.OrganizationAdministratorWithMfa)
    .WithName("ListOrganizationMemberships")
    .WithTags("Organizations");

app.Run();

public sealed record HealthResponse(string Status, string Service, string Version);

public partial class Program;
