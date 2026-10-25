# Phase 2 — Multi-Tenant Backend and Database

**Status:** IN PROGRESS — implemented on a dedicated branch; not merged and not production-ready.

## Scope delivered in this draft

| Work item | Draft implementation | Required evidence |
|---|---|---|
| 2.1 PostgreSQL/EF Core integration | Npgsql EF Core provider and configurable DbContext | Build plus live PostgreSQL CI |
| 2.2 Initial tenant schema | Organizations, external identity references, memberships, departments | Migration applies to a clean PostgreSQL database |
| 2.3 Tenant-scope enforcement | Authenticated-principal tenant context, query filters, tenant-owned write guard | Cross-tenant read/write and missing-scope tests |
| 2.4 Tenant repositories | Current-organization, membership and department queries; department parent validation | PostgreSQL integration tests |
| 2.5 Readiness | Liveness remains dependency-independent; readiness reports database connectivity | Missing/down/up database health checks |
| 2.6 Developer guidance | Connection string and migration instructions | Documentation review |

## Explicitly not implemented

- No login, OIDC provider, session, MFA, or authorization endpoint.
- No public organization, membership, or department business APIs.
- No device enrollment, activity ingestion, browser data, screen viewing, or employee-monitoring behavior.
- No automated production migration on application startup.
- No production database credentials or hosting configuration.

## Required security contract

The organization claim is a server-side integration contract, not a client-controlled input. Phase 3 must create that claim only after authenticating the identity and resolving an active membership for the selected organization. Do not map a request header, query parameter, route parameter, or unvalidated upstream claim directly into `ITenantContext`.

Tenant-owned writes require an authenticated organization scope and reject mismatched organization IDs. Repository reads require the scoped tenant and include explicit organization predicates; EF query filters and composite foreign keys provide additional defense in depth.

## Gates before merge or production use

1. Run the complete repository CI and the PostgreSQL integration workflow; all jobs must pass on the exact PR head.
2. Confirm migration apply/reapply behavior and cross-tenant isolation on PostgreSQL.
3. Review EF migrations, package advisories, and the tenant-claim trust boundary.
4. Enable `main` branch protection and required CI checks; the repository currently reports `main` as unprotected.
5. Keep authentication and activity collection out of scope until Phase 3 and the relevant privacy/security gates are approved.
