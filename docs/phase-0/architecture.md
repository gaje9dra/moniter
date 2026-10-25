# Phase 0.3 — Technical Architecture

**Status:** Approved foundation architecture; full production architecture remains gated on later phase decisions.

## Foundation components

1. **Administration dashboard:** Next.js App Router, React and TypeScript on Node.js 24, in `apps/dashboard`. It provides an accessible shell and a real backend liveness view, with explicit API-unavailable/error states.
2. **Backend API:** ASP.NET Core Web API on .NET 10 in `src/Moniter.Api`. It exposes service information, liveness and readiness, OpenAPI in Development, correlation identifiers, JSON structured logs and a safe exception response.
3. **Windows endpoint agent:** .NET 10 Worker Service in `src/Moniter.Agent`; conditionally hosted as a Windows Service on Windows. This phase covers configuration, version reporting, diagnostics, startup and shutdown only.
4. **Tests:** xUnit API and agent tests in `tests/`; Vitest tests in the dashboard package.
5. **Automation:** GitHub Actions under `.github/workflows/ci.yml`, with Linux checks plus a Windows-specific agent build/test job.

## Planned but not implemented in this foundation

- PostgreSQL and Entity Framework Core migrations.
- Authentication, multi-tenant business models and authorization.
- Agent enrollment, credential issuance and activity-event ingestion.
- SignalR authenticated activity delivery.
- Browser metadata integration, screen viewing, reporting, retention jobs and production deployment.

## Data-flow boundary

The dashboard calls `GET /health/live` on the configured API origin. The API returns a service status and version. If unavailable, dashboard rendering surfaces a clear unavailable state. The agent does not connect to production services and does not collect user or application activity.

## Security principles

- No production credentials are required to run development or CI.
- The API emits only exception type and correlation ID in unexpected failure logs, not exception message/stack trace, to reduce risk of sensitive-value leakage.
- Correlation IDs accept only ASCII letters, digits, underscore and hyphen, length <= 64; invalid values are replaced.
- No employee activity or browser data is written in this phase.
- TLS is mandatory for a production API origin; local HTTP is permitted for development only.
- Later phases must enforce tenant isolation, server-side authorization, secure endpoint credentials, event deduplication, policy enforcement and signed updates before real monitoring is enabled.

## Operational considerations

API liveness is independent of external dependencies. Readiness currently reports the host's health-check pipeline; because no external dependency is connected, it must not be interpreted as database readiness. When PostgreSQL is introduced, database readiness checks must be added and tested against the configured connection.

## Failure recovery

API request correlation is returned in `X-Correlation-ID`; dashboard uses a bounded timeout and graceful error handling. Agent cancellation token drives safe worker shutdown. Durable offline buffering, event retries, realtime reconnection and signed updates are later-phase responsibilities.
