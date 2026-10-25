# Phase 1 — Configuration Reference

## Dashboard
| Variable | Required | Default | Validation |
|---|---|---|---|
| `MONITER_API_BASE_URL` | No, local development | `http://localhost:5080` | Absolute HTTP(S) URL; HTTPS required in Production. |

Dashboard API calls use a 2.5-second timeout. The setting is server-side only and must not use a `NEXT_PUBLIC_` prefix.

## API
- `ASPNETCORE_ENVIRONMENT`: standard ASP.NET Core environment; Development exposes the foundation's OpenAPI document at `/openapi`.
- `ASPNETCORE_URLS`: optional host URL override; the launch profile defaults to `http://localhost:5080`.
- No database connection string is required. No PostgreSQL provider is wired yet.

## Agent
- `Moniter:Environment`: default `Development`; 1–40 characters.
- `Moniter:DiagnosticLevel`: default `Information`; one of Trace, Debug, Information, Warning, Error, Critical, None.
- Environment variable mapping uses double underscore, e.g. `Moniter__Environment`. Invalid settings fail with an actionable startup error.

## Secrets
Never commit production config, tokens, signing keys, database credentials or `.env.local`. API clients receive a generic unexpected-error response and correlation ID; internal error logging excludes exception messages and stack traces.
