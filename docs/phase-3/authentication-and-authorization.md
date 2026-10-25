# Phase 3 — Authentication, Organization Authorization, and MFA

## Identity-provider integration

Moniter accepts OAuth 2.0 access tokens using ASP.NET Core JWT bearer authentication and an OpenID Connect discovery authority. It does not accept an ID token as an API access token, issue tokens, collect passwords, or store provider credentials.

Required configuration:

| Setting | Purpose |
|---|---|
| MONITER_AUTHORITY | HTTPS authority/issuer base published by the identity provider |
| MONITER_AUDIENCE | Exact API audience for Moniter access tokens |
| MONITER_MFA_AMR_VALUES | Comma-separated, exact amr values that the provider uses only after successful MFA |
| MONITER_MFA_ACR_VALUES | Comma-separated, exact acr values that represent the required MFA assurance level |
| MONITER_AUTH_REQUIRE_HTTPS_METADATA | Defaults to true; must never be disabled in production |

Issuer, audience, signed-token, signing-key, and lifetime validation are enabled. Clock skew is limited to one minute. Inbound claim mapping is disabled so provider claims can be validated using their protocol names. Detailed bearer authentication errors and token persistence are disabled.

At least one provider-specific amr or acr value must be configured before /health/ready can report healthy. The configured value must be verified against real tokens and provider documentation; a generic or guessed value is not sufficient production evidence.

## Identity mapping and tenant selection

The API maps a validated token using the exact issuer and subject pair to an existing, active UserIdentity record. Email and display name are not account keys. Moniter will not auto-create identities or connect accounts based on email.

The caller may omit X-Organization-Id only if exactly one active membership exists in an active organization. If several memberships exist, the caller must provide one organization ID. The header is a selector, not a credential: the server checks persisted membership and organization status on every API request.

Only after membership resolution does middleware add a server-owned organization, role, identity, and context-resolved marker. Inbound claims using Moniter's reserved context claim types are removed. The tenant context refuses to expose an organization ID without the server-resolved marker.

Failure behavior:
- Missing or invalid bearer tokens are challenged with HTTP 401.
- Invalid organization selectors and ambiguous multi-organization requests receive HTTP 400.
- Missing identity mappings, inactive identities, missing/suspended memberships, inactive organizations, and unauthorized organization selection receive HTTP 403.
- Privileged roles that lack configured, validated MFA evidence receive HTTP 403.
- No token, raw claim set, provider secret, or connection detail is returned in error responses.

## Server-side role matrix

| Role | Member endpoints | Manager policy | Administrator policy | Membership listing |
|---|---:|---:|---:|---:|
| Owner | Yes | Yes | Yes | Yes, with MFA |
| Administrator | Yes | Yes | Yes | Yes, with MFA |
| Manager | Yes | Yes | No | No |
| Analyst | Yes | No | No | No |
| Auditor | Yes | No | No | No |
| Member | Yes | No | No | No |

Owner, Administrator, and Manager roles require an accepted MFA assurance claim for all /api requests. The membership listing endpoint independently requires Owner or Administrator plus the Moniter server-created MFA marker. Upstream role, organization, and Moniter MFA-marker claims do not grant permissions.

All endpoints require authentication by fallback policy unless explicitly marked anonymous. New business endpoints must attach the narrowest suitable authorization policy; frontend visibility is never an authorization control.

## Audit and operational controls

The application emits structured JSON security events for organization-context resolution/denial, privileged MFA denial, authorization challenges/denials, and organization membership listing. Events include an operation/event name, relevant internal IDs where needed, and correlation ID. They exclude access tokens, request bodies, query strings, email addresses, and raw provider claims.

The deployment must separately configure log access, retention, monitoring, backup, and alert delivery. This phase does not create a durable database audit ledger.

## Provisioning and release limitations

Membership creation, role grants, deactivation, invitation, and account linking remain out of scope. Provisioning rows must be created through a separately authenticated and audited administrative process; direct database edits are not a substitute for a future audited administration workflow.

Before production enablement:
1. Configure the real authority, audience, and provider-specific MFA values in the secret/configuration manager.
2. Provision a test identity and membership through an approved process.
3. Run end-to-end tests with actual signed access tokens, invalid/expired tokens, alternate audiences, issuer mismatch, and provider-confirmed MFA/non-MFA sessions.
4. Verify /health/ready against reachable PostgreSQL and valid authentication configuration.
5. Review authorization policy coverage and security audit log retention/access.
6. Require green CI and an independent review of the exact PR head.

No external identity provider was selected or provisioned in this implementation. The integration is provider-configurable but externally unverified until those checks occur.
