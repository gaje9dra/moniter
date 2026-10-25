# Phase 0.3 — Technology Decisions

**Decision status:** Approved foundation baseline for Phase 0/Phase 1. Final version compatibility is enforced by pinned direct dependencies, committed lockfiles, and CI. Production acceptance remains gated on jurisdictional and organizational approvals.

## Approved stack

| Area | Approved baseline | Compatibility / scope |
|---|---|---|
| Administration dashboard | Node.js 24 LTS, pnpm 10.20.0, Next.js 15.5.6 App Router, React 19.1.1, TypeScript 5.9.3 | Exact frontend dependency versions in `apps/dashboard/package.json`; frozen lockfile installs required. Accessible responsive UI. |
| Backend | ASP.NET Core Web API, C#, .NET 10 LTS, nullable reference types, built-in OpenAPI | `net10.0`, warnings treated as errors, health endpoints and development API description. |
| Windows endpoint agent | C#/.NET 10 Worker Service, Windows Service integration on Windows | Configuration validation, version and diagnostics, safe startup/shutdown only; no endpoint activity collection. |
| Database | PostgreSQL | Planned for a later phase with Entity Framework Core, reviewed versioned migrations and migration tests. Not connected by the foundation. |
| Real-time | SignalR | Planned for authenticated, authorized live updates; no monitoring stream in the foundation. |
| Tests | xUnit for .NET; Vitest for frontend; Playwright for browser E2E when real user flows exist | No fake monitoring flows or tests against nonexistent business features. |
| CI and security | GitHub Actions on Linux and Windows; Gitleaks; .NET vulnerability auditing | Checks fail the workflow on errors; Windows target is built and tested on a Windows runner. |

## Version and upgrade policy

Direct frontend packages are exactly pinned; pnpm 10.20.0 and Node.js 24 are configured in CI. `apps/dashboard/pnpm-lock.yaml` must be a complete lockfile generated with pnpm 10.20.0 and accepted only with `pnpm install --frozen-lockfile`. .NET projects target `net10.0` and package references are explicit. `global.json` pins SDK 10.0.100 with `latestFeature` roll-forward; the actual SDK patch used by hosted CI must be read from job logs. Review dependency upgrades through pull requests and rerun all checks.

## Deferred decisions and gates

OIDC identity provider, PostgreSQL hosting, EF Core schema/migrations, authenticated SignalR, production agent signing/distribution, data retention enforcement, and deployment infrastructure are not implemented by the foundation. Windows 10/11 editions/builds and lifecycle status must be confirmed against the target fleet before releases. Jurisdiction-specific legal review, employee notice, production retention periods, and organizational authorization are release gates.

These decisions approve the foundation technology choices; they do not imply product or production readiness.
