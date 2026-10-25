# Phase 1 — Development Setup

## Prerequisites
- Node.js 24.x and Corepack/pnpm 10.20.0.
- .NET 10 SDK.
- Windows 10/11 for Windows Service runtime verification; Linux can run API and cross-platform tests.
- PostgreSQL is not required in this foundation because no database integration is wired.

## Dashboard
From `apps/dashboard`:
```sh
corepack enable
pnpm install --frozen-lockfile
pnpm lint
pnpm typecheck
pnpm test
pnpm build
pnpm dev
```
Set `MONITER_API_BASE_URL=http://localhost:5080` in a local ignored `.env.local` if overriding the default.

## API and agent
From repository root:
```sh
dotnet restore Moniter.sln
dotnet build Moniter.sln --configuration Release
dotnet test Moniter.sln --configuration Release
dotnet run --project src/Moniter.Api/Moniter.Api.csproj
```
The API uses port 5080 by default through the Development launch profile. The agent has safe Development defaults in `appsettings.json` and idles until stopped; it does not collect employee activity.

## Data and privacy
Use development-only settings and fake credentials for local configuration tests only. The foundation requires no production credentials or real employee devices. Activity-event retention is provisionally 90 days for development/test planning only; no event data is persisted by this foundation. Production retention is a legal/organizational release gate.
