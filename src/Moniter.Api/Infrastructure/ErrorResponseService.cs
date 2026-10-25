namespace Moniter.Api.Infrastructure;

public sealed class ErrorResponseService
{
    public object CreateValidationError(string correlationId, IReadOnlyDictionary<string, string[]> errors) =>
        new
        {
            type = "https://httpstatuses.com/400",
            title = "Request validation failed.",
            status = 400,
            correlationId,
            errors
        };

    public object CreateInternalError(string correlationId) =>
        new
        {
            type = "https://httpstatuses.com/500",
            title = "An unexpected error occurred.",
            status = 500,
            correlationId
        };
}
