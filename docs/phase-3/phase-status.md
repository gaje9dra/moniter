# Phase 3 — Authentication and Authorization

**Status:** Implementation submitted for CI verification. Production identity-provider integration and independent security review remain outstanding.

## Delivered in this branch

| Requirement | Implementation | Evidence still required |
|---|---|---|
| Trusted bearer authentication | ASP.NET Core JWT bearer validation against a configured OIDC authority and API audience; issuer, audience, signature, expiry, and subject claims validated | Configure the real provider and test real access tokens |
| Persisted identity mapping | Match validated issuer + subject to an active, pre-provisioned UserIdentity; no account auto-linking or claim-based provisioning | Securely provision test and production identities |
| Tenant selection | Optional X-Organization-Id selector; server resolves active membership and active organization before creating tenant claims | PostgreSQL integration tests and security review |
| Tenant trust boundary | Discard inbound organization, role, user-identity, tenant-context, and Moniter MFA-marker claims; ITenantContext requires the server-resolved context marker | Review token/provider claim behavior |
| Organization role model | Central server-side policies for member, manager, administrator, and administrator-with-MFA | Add policy coverage as future protected operations are added |
| Privileged MFA | Owner, Administrator, and Manager API access requires provider-signed amr or acr value matching explicit configuration; administrator membership listing has a dedicated MFA policy | Confirm configured assurance values against the real identity provider |
| Audit events | Structured security audit logs for tenant selection/denial, privileged MFA denial, authorization challenge/denial, and privileged membership reads | Configure production log retention, access controls, alerting, and export |
| Readiness | /health/ready requires both PostgreSQL and complete identity/MFA configuration | Validate deployment configuration and provider metadata |
| Migrations | No schema changes required; existing issuer/subject identity and membership schema is reused | No schema migration is required for this phase |

## Protected API surface

- GET /api/v1/me — authenticated organization member.
- GET /api/v1/organizations/current — authenticated organization member.
- GET /api/v1/organization-memberships — Owner or Administrator with verified MFA assertion.

The API authorization fallback requires authentication on endpoints unless they explicitly declare anonymous access. The root, liveness, readiness, and development-only OpenAPI endpoints are explicitly anonymous; liveness and root are not evidence of production readiness.

## Required release blockers

1. Configure MONITER_AUTHORITY, MONITER_AUDIENCE, and provider-specific MFA assurance values.
2. Provision UserIdentity and active OrganizationMembership rows through a separately authorized administrative process. This phase intentionally does not implement self-service account linking or membership grants.
3. Validate real access-token signature, issuer, audience, expiry, and MFA claims against the actual provider.
4. Review tenant selection, membership queries, role policy coverage, logging/retention, and failure behavior independently.
5. Require full repository CI and PostgreSQL migration/tenant-isolation CI to pass on the exact PR head.
6. Configure and enforce main branch protection and review requirements before subsequent merges.

No login UI, identity-provider tenant, client secret, membership-management API, employee monitoring, activity collection, or screen viewing was added.
