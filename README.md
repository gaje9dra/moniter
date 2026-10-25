# Moniter

Moniter is the foundation for a disclosed, policy-governed employee monitoring and endpoint-management platform for authorized company-managed Windows endpoints.

## Current scope

The merged foundation provides a Next.js administration shell, ASP.NET Core health API, .NET Windows-agent scaffold, configuration/error/logging conventions, tests and CI. Phase 2 adds tenant-aware PostgreSQL persistence without exposing business APIs. No employee activity collection is implemented.

## Approved baseline

- Node.js 24 LTS, pnpm 10.20.0, Next.js App Router, React and TypeScript
- .NET 10 LTS, ASP.NET Core Web API and C#
- PostgreSQL and Entity Framework Core for tenant-aware persistence
- SignalR planned for later authenticated real-time updates
- xUnit, frontend tests and GitHub Actions with Linux and Windows checks

## Prerequisites

- Node.js 24.x and Corepack/pnpm
- .NET 10 SDK
- PostgreSQL 16 for database integration tests and local persistence development

## Local development

API:
```sh
dotnet restore src/Moniter.Api/Moniter.Api.csproj
dotnet run --project src/Moniter.Api/Moniter.Api.csproj
```

Configure `ConnectionStrings__Moniter` or `MONITER_DATABASE_URL` with a PostgreSQL ADO.NET connection string. The process liveness endpoint `GET /health/live` does not depend on the database; `GET /health/ready` verifies database connectivity and returns unhealthy if PostgreSQL is unavailable or not configured.

Dashboard:
```sh
cd apps/dashboard
pnpm install --frozen-lockfile
pnpm lint
pnpm typecheck
pnpm test
pnpm build
```

Set `MONITER_API_BASE_URL` (server-side) to the API origin; the default is `http://localhost:5080`. The dashboard health view calls the real API and shows unavailable/error states if the API cannot be reached.

## Migrations and tenant safety

See `docs/phase-2/database-and-tenancy.md`. Database migrations are reviewed and run explicitly; the API does not apply them automatically at startup. Tenant-owned repository access requires a trusted organization context. Authentication and active-membership validation are not implemented yet, and no business endpoints expose tenant data.

## Privacy and deployment boundaries

This foundation does not collect employee activity, browser history, private messages, form contents, credentials or screen content. Do not use production credentials or real employee devices for development. Production monitoring requires jurisdiction-specific privacy/legal approval, authentication and authorization, tenant-isolation verification, retention/deletion enforcement and a reviewed release process.
