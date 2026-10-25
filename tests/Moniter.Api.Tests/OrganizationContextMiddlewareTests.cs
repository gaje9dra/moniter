using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moniter.Api.Data.Entities;
using Moniter.Api.Data.Tenancy;
using Moniter.Api.Security;
using Xunit;

namespace Moniter.Api.Tests;

public sealed class OrganizationContextMiddlewareTests
{
    [Fact]
    public async Task Middleware_replaces_inbound_tenant_role_and_mfa_claims_with_resolved_context()
    {
        var expectedOrganizationId = Guid.NewGuid();
        var userIdentityId = Guid.NewGuid();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("sub", "provider-subject"),
            new Claim(MoniterClaimTypes.TrustedIssuer, "https://identity.example.test"),
            new Claim(MoniterClaimTypes.OrganizationId, Guid.NewGuid().ToString("D")),
            new Claim(MoniterClaimTypes.OrganizationRole, "Owner"),
            new Claim(MoniterClaimTypes.UserIdentityId, Guid.NewGuid().ToString("D")),
            new Claim(MoniterClaimTypes.OrganizationContextResolved, "true"),
            new Claim(MoniterClaimTypes.MfaAssurance, "true")
        }, "test-authentication");
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };
        context.Request.Path = "/api/v1/me";
        var configuration = new IdentityProviderConfiguration(
            "https://identity.example.test",
            "moniter-api",
            true);
        var middleware = new OrganizationContextMiddleware(
            _ => Task.CompletedTask,
            configuration,
            NullLogger<OrganizationContextMiddleware>.Instance);
        var resolver = new FixedOrganizationContextResolver(
            OrganizationContextResolution.Resolved(
                userIdentityId,
                expectedOrganizationId,
                MembershipRole.Member));

        await middleware.InvokeAsync(context, resolver);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.Single(context.User.FindAll(MoniterClaimTypes.OrganizationId));
        Assert.Equal(expectedOrganizationId.ToString("D"), context.User.FindFirst(MoniterClaimTypes.OrganizationId)?.Value);
        Assert.Equal("Member", context.User.FindFirst(MoniterClaimTypes.OrganizationRole)?.Value);
        Assert.Equal("true", context.User.FindFirst(MoniterClaimTypes.OrganizationContextResolved)?.Value);
        Assert.Null(context.User.FindFirst(MoniterClaimTypes.MfaAssurance));
        Assert.Equal(
            expectedOrganizationId,
            new HttpTenantContext(new HttpContextAccessor { HttpContext = context }).OrganizationId);
    }

    [Fact]
    public async Task Middleware_rejects_an_unverified_organization_selector()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "provider-subject"),
                new Claim(MoniterClaimTypes.TrustedIssuer, "https://identity.example.test")
            }, "test-authentication"))
        };
        context.Request.Path = "/api/v1/me";
        context.Request.Headers["X-Organization-Id"] = "not-a-guid";
        var middleware = new OrganizationContextMiddleware(
            _ => Task.CompletedTask,
            new IdentityProviderConfiguration("https://identity.example.test", "moniter-api", true),
            NullLogger<OrganizationContextMiddleware>.Instance);

        await middleware.InvokeAsync(context, new FixedOrganizationContextResolver(
            OrganizationContextResolution.Rejected(OrganizationContextFailure.NoActiveMembership)));

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    private sealed class FixedOrganizationContextResolver(OrganizationContextResolution result)
        : IOrganizationContextResolver
    {
        public Task<OrganizationContextResolution> ResolveAsync(
            string issuer,
            string subject,
            Guid? selectedOrganizationId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }
}
