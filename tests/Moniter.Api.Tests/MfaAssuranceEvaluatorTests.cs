using System.Security.Claims;
using Moniter.Api.Security;
using Xunit;

namespace Moniter.Api.Tests;

public sealed class MfaAssuranceEvaluatorTests
{
    private static readonly IdentityProviderConfiguration Configuration = new(
        "https://identity.example.test",
        "moniter-api",
        true,
        new HashSet<string>(StringComparer.Ordinal) { "mfa" },
        new HashSet<string>(StringComparer.Ordinal) { "urn:example:loa:2fa" });

    [Fact]
    public void Accepts_an_explicit_configured_amr_value()
    {
        var principal = Principal(new Claim("amr", "mfa"));
        Assert.True(MfaAssuranceEvaluator.IsSatisfied(principal, Configuration));
    }

    [Fact]
    public void Accepts_an_explicit_configured_acr_value()
    {
        var principal = Principal(new Claim("acr", "urn:example:loa:2fa"));
        Assert.True(MfaAssuranceEvaluator.IsSatisfied(principal, Configuration));
    }

    [Fact]
    public void Rejects_missing_or_unconfigured_mfa_evidence()
    {
        var principal = Principal(
            new Claim("amr", "pwd"),
            new Claim(MoniterClaimTypes.MfaAssurance, "true"));
        Assert.False(MfaAssuranceEvaluator.IsSatisfied(principal, Configuration));
    }

    [Fact]
    public void Does_not_treat_a_client_controlled_internal_mfa_marker_as_evidence()
    {
        var principal = Principal(new Claim(MoniterClaimTypes.MfaAssurance, "true"));
        Assert.False(MfaAssuranceEvaluator.IsSatisfied(principal, Configuration));
    }

    private static ClaimsPrincipal Principal(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, "test-authentication"));
}
