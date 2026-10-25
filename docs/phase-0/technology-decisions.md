# Phase 0.3 — Technology Decisions

**Decision status:** Approved foundation baseline for Phase 1 as of 2026-10-25. Compatibility and build verification are delegated to CI and must be evidenced before sign-off.

| Area | Approved baseline | Scope notes |
|---|---|---|
| Dashboard | Node.js 24 LTS, pnpm 10.20.0, Next.js 15.5.6 App Router, React 19.1.1, TypeScript 5.9.3 | Exact package versions; lockfile required. UI uses accessible responsive patterns. |
| API | ASP.NET Core Web API, C#, .NET 10 LTS, nullable reference types, built-in OpenAPI | Minimal REST endpoints in Phase 1. Business APIs are deferred. |
| Windows agent | C#, .NET 10 Worker Service; Windows Service host when running on Windows | Foundation only: config validation, version, diagnostics and lifecycle. No activity collection. |
| Database | PostgreSQL for future business-data persistence | No DB schema/provider/migrations added in Phase 1. |
| Realtime | SignalR for future authenticated updates | No monitoring stream or realtime activity collection implemented in Phase 1. |
| Tests | xUnit; Vitest for frontend unit tests; Playwright reserved for actual browser E2E once deployed UI flow exists | Do not add E2E tests that only simulate non-existent business features. |
| CI | GitHub Actions with Linux dashboard/API checks and a Windows agent build/test job | Secret scanning and .NET vulnerability listing included. |

## Version policy

Direct frontend dependencies are exactly pinned in `apps/dashboard/package.json`; the pnpm lockfile must be committed and CI must use `pnpm install --frozen-lockfile`. .NET targets `net10.0`; package references use explicit versions and the SDK baseline is declared in `global.json`. CI installs the latest available .NET 10 feature-band SDK, so the exact patch installed by hosted runners is reported by workflow logs. Review dependency upgrades through pull requests.

## Deferred decisions

OIDC provider, PostgreSQL hosting, EF Core data model/migrations, authenticated SignalR authorization, retention enforcement in a persistent event store, agent code signing, deployment infrastructure, and jurisdiction-specific production retention remain future phase/release gates. The foundation must not imply they are implemented.

The approved baseline is a foundation choice, not a production readiness statement. Revisit only for a concrete compatibility, security, maintenance or operational blocker documented with evidence.
