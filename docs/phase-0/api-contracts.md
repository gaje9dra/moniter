# Phase 0.4 — Proposed API Contracts

**Status:** Contract baseline for review; illustrative routes are not implemented. All routes require TLS and server-side authorization unless explicitly enrollment/public. Use a consistent envelope and documented versioning strategy.

## General contract rules

- Validate request schema, size, identifiers and enums; reject unknown sensitive fields.
- Use structured errors such as `400 VALIDATION_ERROR`, `401 UNAUTHENTICATED`, `403 FORBIDDEN`, `404 NOT_FOUND`, `409 CONFLICT`, `410 EXPIRED_OR_REVOKED`, `413 PAYLOAD_TOO_LARGE`, `422 POLICY_REJECTED`, `429 RATE_LIMITED`, and `5xx` transient failures.
- List endpoints use cursor pagination, stable sort order and bounded page sizes. Filters are validated and tenant scope cannot be overridden by query parameters.
- Mutating requests accept an idempotency key where retries could cause duplicate effects.
- Activity ingestion returns accepted/duplicate/rejected event outcomes; success only after durable acceptance.
- Do not return secret tokens after initial issuance; redact them from telemetry and audit logs.

## Proposed route matrix

| Method / route | Purpose and input/output | Authorization and validation |
|---|---|---|
| `GET /api/v1/me` | Trusted identity and memberships | Authenticated user; only own data |
| `POST /api/v1/organizations` | Create organization | Trusted provisioner or owner; audit action |
| `GET /api/v1/organizations/{orgId}/members` | Paged members | Active member with member-read scope; enforce tenant |
| `POST /api/v1/devices/enrollments` | Create time-limited enrollment authorization | Org admin; rate limit; only token hash stored |
| `POST /api/v1/agent/enroll` | Redeem one-time token and bind device | Enrollment proof; one-time atomic redemption |
| `POST /api/v1/agent/heartbeat` | Device health and version | Device credential; bounded payload and throttling |
| `GET /api/v1/agent/policy` | Retrieve current versioned policy | Active enrolled device only |
| `POST /api/v1/agent/events` | Batch minimized activity events | Device credential, schema, event limits, policy checks and idempotency |
| `POST /api/v1/agent/browser-events` | Policy-enabled redacted browser metadata | Device credential and explicitly enabled policy |
| `POST /api/v1/devices/{id}/revoke` | Revoke device/credential | Authorized org admin; idempotent; audit |
| `GET /api/v1/devices` | Filtered, paged inventory | Authorized tenant role |
| `GET /api/v1/devices/{id}/activity` | Historical timeline | Device/team permission; date limits and retention filters |
| `GET /api/v1/reports/application-usage` | Aggregated authorized usage | Report scope; bounded range and query cost |
| `POST /api/v1/reports/exports` | Create an audited export job | Export permission, row/time limits and redaction |
| `GET /api/v1/reports/exports/{id}` | Retrieve export status/download | Same tenant and owner/role check; short-lived download permission |
| `GET /api/v1/audit-logs` | Search audit history | Auditor/admin scope, cursor pagination |
| `GET /api/v1/stream` | Authenticated realtime subscription | Revalidate membership/roles and scope every event |
| `GET/PUT /api/v1/policies/{id}` | Read/update monitoring policy | Policy manager; versioning, approver and audit required |

## Event envelope

Required fields: `schemaVersion`, `eventId`, `deviceSequence` (if supported), `deviceOccurredAt`, `policyVersion`, `kind`, and a bounded kind-specific minimized payload. The organization and authoritative device association come from device credentials, not client-supplied tenant fields.

## Acceptance tests

Test unauthenticated access, least-privilege roles, cross-tenant ID substitution, revoked devices, expired/replayed enrollment, duplicate events, oversized batches, invalid timestamps, disabled collection policies, sensitive-site exclusions, cursor pagination integrity, export authorization and audit events, stream reconnection and membership revocation.
