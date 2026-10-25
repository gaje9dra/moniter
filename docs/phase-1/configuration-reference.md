# Phase 1 — Configuration Reference

## Dashboard
| Variable | Required | Default | Validation |
|---|---|---|---|
| `MONITER_API_BASE_URL` | No, local development | `http://localhost:5080` | Absolute HTTP(S) URL; HTTPS required in Production. |

Dashboard API calls use a 2.5 second timeout and do not cache liveness. The configuration is server-side and must not be exposed via a `NEXT_PUBLIC_` variable.

## API
- `ASPNETCORE_ENVIRONMENT`: standard ASP.NET Core environment; Development enables OpenAPI document endpoint.
- `ASPNETCORE_URLS`: optional host URL override; launch profile defaults to `http://localhost:5080`.
- No database connection string is required in this foundation. Do not add production secrets to tracked settings.

## Agent
- `Moniter:Environment`: default `Development`; 1-40 characters.
- `Moniter:DiagnosticLevel`: default `Information`; one of Trace, Debug, Information, Warning, Error, Critical, None.
- Values can be set via .NET configuration sources/standard environment variable mapping (e.g. `Moniter__Environment`). Invalid configuration causes an actionable startup error.

## Secret handling
- Never commit `.env.local`, production config files, tokens, signing keys, or database credentials.
- Keep CI independent of production services and credentials.
- API error responses contain a generic title and correlation ID; sensitive exception message/stack is not returned to clients.
