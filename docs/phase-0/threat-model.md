# Phase 0.5 — Threat Model

**Status:** Design-level initial model. Revalidate against the implemented stack and deployment environment.

| Threat | Impact | Principal mitigations | Required verification |
|---|---|---|---|
| Unauthorized device enrollment | Unauthorized monitoring or data injection | Admin-authorized one-time expiring enrollment, token hash, rate limits, atomic redemption | Expired/replayed/guessed enrollment tests |
| Stolen agent credential | Forged activity and unauthorized ingestion | Device-scoped credentials, rotation/revocation, protected OS storage, TLS | Revoke and rotation tests |
| Device impersonation | Misattributed events | Strong device identity, proof of possession where appropriate, device-bound credentials | Replay and cloned credential simulations |
| Cross-tenant access | Confidentiality breach | Server-derived tenant scope, scoped queries, constraints/RLS defense in depth | Tenant-matrix tests on every API/report/stream/job |
| Privilege escalation | Unauthorized access/export | Deny-by-default RBAC, short sessions, MFA for privileged roles, audit trails | Role transition and direct API tests |
| Unauthorized live viewing | Exposure of sensitive activity | Explicit view permission, tenant-scoped stream, reauthorization/revocation | Subscription isolation tests |
| Unauthorized screen viewing | Excessive collection and privacy harm | Off by default; separate documented authorization, visible session, short duration, explicit audit and stop control | Authorization, timeout and audit tests before any implementation |
| Event injection/replay | False records and analytics | Schema validation, authenticated device identity, event ID uniqueness, rate limiting and policy checks | Forged/duplicate/malformed/replayed payload tests |
| Sensitive website collection | Collection of personal/sensitive behavior | Pre-persistence exclusions/redaction, documented exceptions, data minimization | Excluded-domain non-persistence tests |
| Unauthorized exports | Bulk disclosure | Separate export permission, limits, redaction, expiry, audit | Access control and leakage tests |
| Database compromise | Bulk loss/disclosure | Least-privilege credentials, network isolation, encryption, patching, tested backups | Restore and access-review exercises |
| Compromised admin account | Broad monitoring access | MFA, least privilege, session controls, alerts and audited sensitive actions | Admin-compromise tabletop and privilege tests |
| Malicious agent update | Endpoint compromise | Signed artifacts, protected signing keys, staged release, rollback and version validation | Signature tampering and rollback tests |
| Excessive retention | Unnecessary sensitive data exposure | Per-category retention, deletion jobs, review and evidence | Expiry/deletion integration tests |
| Outage / recovery | Missing or stale data | Bounded queue, retry, backpressure, idempotency, health/freshness indicators | Failure injection and recovery tests |

## Trust boundaries

Human user/browser, agent/device, backend, database, realtime broker, CI/build/release pipeline, and administrative infrastructure are separate trust domains. Compromise of any one must not automatically imply access to all tenant data or release signing authority.

## Non-goals

No stealth installation, hidden surveillance, keystroke capture, password/token collection, private-message ingestion, or evasion of security controls. Any later screen viewing feature requires a separate threat/privacy review.
