using Xunit;

namespace Moniter.Api.Tests;

[AttributeUsage(AttributeTargets.Method)]
public sealed class PostgreSqlFactAttribute : FactAttribute
{
    public PostgreSqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MONITER_TEST_DATABASE")))
        {
            Skip = "Set MONITER_TEST_DATABASE to run PostgreSQL integration tests.";
        }
    }
}
