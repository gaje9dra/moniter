# Phase 1 — Testing Guide

## Dashboard
From `apps/dashboard`:
- `npm install --global pnpm@10.20.0` (or enable Corepack using the local Node installation)
- `pnpm install --frozen-lockfile`
- `pnpm lint`
- `pnpm typecheck`
- `pnpm test`
- `pnpm build`

Vitest covers API-origin validation. Build checks the Next.js App Router compile. To verify backend-unavailable behavior, run the dashboard while the API is stopped; the health panel should show Unavailable instead of fake status.

## .NET
From repository root:
- `dotnet restore Moniter.sln`
- `dotnet format Moniter.sln --verify-no-changes --no-restore`
- `dotnet build Moniter.sln --configuration Release`
- `dotnet test Moniter.sln --configuration Release`
- `dotnet list Moniter.sln package --vulnerable --include-transitive`

xUnit tests cover API liveness payload and correlation ID handling, plus agent configuration validation and version reporting.

## Windows
CI builds and tests the agent on `windows-latest`, which is required evidence for Windows-specific compilation. Windows Service install/uninstall lifecycle and the exact Windows 10/11 edition/build support matrix are not end-to-end verified yet.

## Scope boundaries
No tests assert that monitoring is operational because activity collection has not been implemented. PostgreSQL/EF Core, SignalR, auth and Playwright end-to-end tests are deferred until their functionality exists.
