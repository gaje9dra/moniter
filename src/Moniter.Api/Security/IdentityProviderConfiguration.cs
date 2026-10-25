using System.Security.Claims;

namespace Moniter.Api.Security;

public sealed class IdentityProviderConfiguration
{
    public IdentityProviderConfiguration(
        string? authority,
        string? audience,
        bool requireHttpsMetadata,
        IReadOnlySet<string>? mfaAmrValues = null,
        IReadOnlySet<string>? mfaAcrValues = null)
    {
        Authority = authority;
        Audience = audience;
        RequireHttpsMetadata = requireHttpsMetadata;
        MfaAmrValues = mfaAmrValues ?? new HashSet<string>(StringComparer.Ordinal);
        MfaAcrValues = mfaAcrValues ?? new HashSet<string>(StringComparer.Ordinal);
    }

    public string? Authority { get; }
    public string? Audience { get; }
    public bool RequireHttpsMetadata { get; }
    public IReadOnlySet<string> MfaAmrValues { get; }
    public IReadOnlySet<string> MfaAcrValues { get; }

    public static IdentityProviderConfiguration FromConfiguration(IConfiguration configuration)
    {
        var requireHttpsText =
            configuration["MONITER_AUTH_REQUIRE_HTTPS_METADATA"] ??
            configuration["Moniter:Authentication:RequireHttpsMetadata"];
        var requireHttps = true;
        if (!string.IsNullOrWhiteSpace(requireHttpsText) &&
            bool.TryParse(requireHttpsText, out var parsedRequireHttps))
        {
            requireHttps = parsedRequireHttps;
        }

        return new IdentityProviderConfiguration(
            configuration["MONITER_AUTHORITY"] ?? configuration["Moniter:Authentication:Authority"],
            configuration["MONITER_AUDIENCE"] ?? configuration["Moniter:Authentication:Audience"],
            requireHttps,
            ParseValues(configuration["MONITER_MFA_AMR_VALUES"] ??
                        configuration["Moniter:Authentication:MfaAmrValues"]),
            ParseValues(configuration["MONITER_MFA_ACR_VALUES"] ??
                        configuration["Moniter:Authentication:MfaAcrValues"]));
    }

    public string? GetReadinessProblem(IHostEnvironment environment)
    {
        if (string.IsNullOrWhiteSpace(Authority) ||
            !Uri.TryCreate(Authority, UriKind.Absolute, out var authorityUri))
        {
            return "An absolute identity-provider authority URL is required.";
        }

        if (authorityUri.Scheme != Uri.UriSchemeHttps &&
            !(environment.IsDevelopment() &&
              !RequireHttpsMetadata &&
              authorityUri.Scheme == Uri.UriSchemeHttp &&
              authorityUri.IsLoopback))
        {
            return "The identity-provider authority must use HTTPS.";
        }

        if (environment.IsProduction() && !RequireHttpsMetadata)
        {
            return "HTTPS metadata validation cannot be disabled in production.";
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            return "The API audience is required.";
        }

        if (MfaAmrValues.Count == 0 && MfaAcrValues.Count == 0)
        {
            return "At least one provider-specific MFA assurance value must be configured.";
        }

        return null;
    }

    private static IReadOnlySet<string> ParseValues(string? configuredValues)
    {
        if (string.IsNullOrWhiteSpace(configuredValues))
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        return configuredValues
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(value => value.Length > 0)
            .ToHashSet(StringComparer.Ordinal);
    }
}
