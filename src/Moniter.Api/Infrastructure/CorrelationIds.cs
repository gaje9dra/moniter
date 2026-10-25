namespace Moniter.Api.Infrastructure;

public static class CorrelationIds
{
    public static string? TryNormalize(string? candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate) || candidate.Length > 64)
            return null;

        if (candidate.Any(ch => !char.IsAsciiLetterOrDigit(ch) && ch is not '-' and not '_'))
            return null;

        return candidate;
    }
}
