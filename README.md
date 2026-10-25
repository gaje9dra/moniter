# Moniter

Moniter is the foundation for a disclosed, policy-governed employee monitoring and endpoint management platform for authorized company-managed Windows endpoints.

## Current scope

This repository currently provides an application foundation only: a Next.js administration shell, an ASP.NET Core health API, a .NET Windows-agent service scaffold, configuration/error/logging conventions, tests, and CI. Employee activity collection, browser history collection, screen viewing, enrollment, authentication, tenant business models, and reporting are not implemented.

## Approved baseline

- Node.js 24 LTS, pnpm, Next.js App Router, React, TypeScript
- .NET 10 LTS, ASP.NET Core Web API, C#
- PostgreSQL planned for later phases; no database integration/migration is included in the foundation
- SignalR planned for authenticated realtime delivery; no monitoring stream is exposed in this foundation
- xUnit, frontend tests, and GitHub Actions

## Prerequisites

- Node.js 24.x and Corepack/pnpm
- .NET 10 SDK
- For Windows Service hosting and agent CI: Windows 10/11 or a supported Windows runner; Linux can compile the generic worker project, but the Windows-specific service host is conditionally built only on Windows.

## Local development

API:
```sh
dotnet restore src/Moniter.Api/Moniter.Api.csproj
dotnet run --project src/Moniter.Api/Moniter.Api.csproj
```
API health: `GET http://localhost:5080/health/live`, readiness: `GET http://localhost:5080/health/ready`, OpenAPI: `/openapi/v1.json` in Development.

Agent:
```sh
dotnet restore src/Moniter.Agent/Moniter.Agent.csproj
dotnet test tests/Moniter.Agent.Tests/Moniter.Agent.Tests.csproj
dotnet run --project src/Moniter.Agent/Moniter.Agent.csproj
```
The agent only validates configuration, emits version/diagnostic information, and starts/stops safely. It does not collect employee activity.

Dashboard:
```sh
cd apps/dashboard
pnpm install --frozen-lockfile
pnpm lint
pnpm typecheck
pnpm test
pnpm build
pnpm dev
```
Set `MONITER_API_BASE_URL` (server-side) to the API origin; default is `http://localhost:5080`. The dashboard health view calls the real API and shows unavailable/error states when the API cannot be reached.

## Configuration and privacy

See `docs/phase-1/development-setup.md`, `docs/phase-1/configuration-reference.md`, and `docs/phase-1/security-baseline.md`. Do not use production credentials or real employee devices for development. Activity-event retention is provisionally configured at 90 days for development/test design purposes only; this foundation does not persist activity events. Production retention requires jurisdiction-specific legal and organizational approval.
