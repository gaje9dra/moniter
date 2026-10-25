# Phase 1 — Testing Guide

## Dashboard
Run from `apps/dashboard`:
- `pnpm lint`
- `pnpm typecheck`
- `pnpm test`
- `pnpm build`

Vitest covers API origin validation. Production build verifies Next.js App Router compilation. Test dashboard unavailable behavior by running without the API and loading the root page; the backend connection panel should display Unavailable.

## .NET
From repository root:
- `dotnet restore Moniter.sln`
- `dotnet format Moniter.sln --verify-no-changes --no-restore`
- `dotnet build Moniter.sln --configuration Release`
- `dotnet test Moniter.sln --configuration Release`
- `dotnet list Moniter.sln package --vulnerable --include-transitive`

xUnit tests validate liveness response/version and correlation handling, plus agent configuration and version logic.

## Windows
CI builds and tests the agent on `windows-latest`; this is required evidence for Windows-specific hosting compilation. Windows Service install/start/stop integration and supported OS edition matrix are not yet end-to-end tested.

## Test boundaries
No tests assert that employee monitoring works because no monitoring functionality is implemented. PostgreSQL/EF migrations, SignalR, auth, and Playwright E2E are deferred until their actual functionality exists.
