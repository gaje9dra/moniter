# Phase 2 — Database and Tenant Isolation

## Data model

The initial schema contains four tables:

- `organizations`: tenant root with unique slug and lifecycle status.
- `user_identities`: stable issuer/subject references to a future trusted identity provider. No passwords or authentication secrets are stored.
- `organization_memberships`: organization-scoped role/status and a unique organization/user pair.
- `departments`: organization-scoped hierarchy with a composite parent foreign key that prevents a department from referencing another tenant's parent.

Activity events, browser data, enrollment tokens, device credentials and screen content are intentionally not part of this migration.

## Configuration

Set either `ConnectionStrings__Moniter` or `MONITER_DATABASE_URL` to a PostgreSQL ADO.NET connection string. For example, in a local PowerShell terminal:

```powershell
$env:MONITER_DATABASE_URL = "Host=localhost;Port=5432;Database=moniter_dev;Username=moniter_app;Password=<local-secret>"
```

Do not commit credentials. Use a local development database and separate least-privilege credentials per environment.

`GET /health/live` reports process liveness and does not depend on PostgreSQL. `GET /health/ready` checks configured database connectivity and returns unhealthy if the connection string is missing or PostgreSQL cannot be reached. The health response does not disclose the connection string or provider error.

## Migrations

Migrations are explicit and must be reviewed. The API does not apply migrations automatically at startup.

With the .NET 10 SDK installed and a local connection string configured:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.13
dotnet ef migrations list --project src/Moniter.Api/Moniter.Api.csproj
dotnet ef database update --project src/Moniter.Api/Moniter.Api.csproj
```

The design-time factory uses the same connection-string configuration keys. Test runs apply migrations to an ephemeral PostgreSQL 16 service in GitHub Actions.

## Tenant boundary

- Tenant context exists only for an authenticated principal with exactly one valid organization claim.
- Later authentication code must issue that claim only after server-side active-membership validation.
- Repositories derive the organization from `ITenantContext`; caller-provided tenant IDs are not repository parameters.
- EF filters scope organization, membership and department reads. A tenant-owned write must match the current scope.
- PostgreSQL constraints enforce uniqueness, membership relationships and same-organization department parentage.
- No API route exposes these repositories in Phase 2. Authentication and authorization are separate Phase 3 scope.

## Test coverage

The PostgreSQL integration test runs the actual EF migration, creates two tenant records, verifies tenant-scoped repository results, rejects missing tenant context for writes, and proves the database rejects a cross-tenant department-parent relationship. The integration test intentionally skips in developer test runs that do not set `MONITER_TEST_DATABASE`; the Phase 2 workflow always supplies an ephemeral PostgreSQL connection so the check is mandatory in CI.
