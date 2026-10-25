# Phase 0.5 — Security Requirements

## Identity and authorization

- All human access must use a vetted authentication mechanism; privileged accounts should require MFA.
- Authorize on the server for each object and action; default deny.
- Enforce tenant membership and team/device scope in APIs, exports, realtime, caches and background jobs.
- Use distinct human and endpoint identities with minimal permissions.

## Enrollment and endpoint credentials

- Enrollment is explicitly authorized, short-lived, one-use and rate-limited.
- Store token hashes only; redeem atomically; never log secrets.
- Device credentials must be scoped, rotatable, revocable and protected using platform-appropriate secure storage.
- Revocation must promptly block policy retrieval and event ingestion.

## Transport, data and application

- TLS for traffic; encryption at rest where supported and appropriate.
- Validate schemas, sizes, event types, timestamps and policy versions.
- Use parameterized database operations and safe output encoding.
- Apply rate limits, bounded batches, backpressure and request timeouts.
- Do not log raw browser URLs, sensitive titles, credentials, full payloads or enrollment tokens.
- Use explicit sensitive-site exclusions and avoid collecting contents, form values, typed input, passwords, authentication tokens or private messages.

## Updates and supply chain

- Sign endpoint releases; protect signing keys separately from ordinary CI credentials.
- Verify signatures and supported versions before installation; staged rollout and rollback are required.
- Pin/review dependencies where practical, scan vulnerabilities and secrets, produce an SBOM where supported, and patch within defined SLAs.

## Audit and incident response

- Log privileged policy changes, enrollment/revocation, sensitive activity access, exports and screen-view actions if separately approved.
- Audit records are append-oriented, access-controlled and redact unnecessary personal data.
- Define revocation, incident triage, key rotation, customer notification and recovery procedures.

## Required release security gates

No production release until security review verifies tenant isolation, RBAC, endpoint credential lifecycle, event integrity, sensitive-site exclusions, export controls, dependency/secrets scanning, signed updates, backup restoration and documented vulnerability response.
