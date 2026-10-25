# Phase 0.2 — Product Requirements and Scope

**Status:** Requirements baseline; pending validation against target jurisdictions and customer policies.

## 1. Product boundary

Build a corporate administration platform for authorized monitoring of company-managed Windows laptops and desktop PCs. Monitoring must be disclosed, policy-governed, proportionate to a legitimate organizational purpose, and limited to enrolled devices. The platform must not include hidden surveillance, keylogging, credential capture, private-message collection, or security-control evasion.

## 2. Roles

- **Organization owner:** manages organization settings, administrators, retention, and tenant-level policies.
- **IT administrator:** enrolls/revokes devices, manages agent health and approved monitoring policies, and views technical activity within granted scope.
- **Manager:** views permitted activity for assigned teams only.
- **HR / analyst:** accesses approved reports only where organizational policy grants it.
- **Auditor:** read-only access to designated audit records.
- **Employee:** receives required monitoring notice and may have access to notice, device assignment, or data-access/correction procedures according to applicable policy and law.
- **Endpoint agent:** device-scoped machine identity; not an interactive human account.

Authorization must be enforced server-side on every request and data subscription.

## 3. Organization and department management

**Purpose:** Maintain isolated customer organizations, departments, teams, employees, managers, and policies.  
**Inputs/outputs:** Validated organization/member/team changes; organization-scoped records and audit events.  
**Dependencies:** Authentication, authorization, tenant-aware database design.  
**Security/privacy:** Every record and query must be scoped to a tenant; deny-by-default; no tenant ID trusted solely from a client.  
**Failure cases:** Missing membership, revoked role, invalid parent organization, attempted cross-tenant access.  
**Acceptance:** Automated tests prove users cannot read or mutate another tenant's records through APIs, exports, background jobs, or live subscriptions.

## 4. Endpoint management

**Purpose:** Enroll and manage company-owned/authorized Windows endpoints.  
**Inputs/outputs:** One-time enrollment material, device identity, version, health, heartbeat, revocation; current device inventory and connection freshness.  
**Dependencies:** Agent identity, API, policy service.  
**Security/privacy:** Enrollment is explicitly authorized, secrets are device-scoped and rotatable, credentials are stored using OS-appropriate secure storage, and revoked identities are rejected.  
**Failure cases:** Expired/replayed enrollment, duplicate identity, revoked device, stale/offline endpoint, unsupported OS or agent version.  
**Acceptance:** Each device has a verifiable lifecycle; revoked devices cannot ingest events or subscribe; UI distinguishes online, offline, stale, and revoked states using documented thresholds.

## 5. Application activity

**Purpose:** Show timestamped foreground application transitions and calculate usage durations.  
**Data:** Application identity/name where permitted, event time, device ID, sequence/event ID, policy version, and ingestion metadata. Avoid collecting window contents or typed data.  
**Failure cases:** Duplicate, out-of-order, missing, malformed or delayed events; device clock drift.  
**Acceptance:** Durations use documented event-time and idle rules; duplicate delivery is idempotent; late events are handled predictably; excluded applications are not collected beyond what is necessary to enforce the exclusion.

## 6. Browser activity

**Purpose:** Provide policy-enabled browser activity metadata for explicitly supported browser integrations.  
**Data:** Browser identity, permitted active-tab title/URL fields, event time, device and policy reference.  
**Security/privacy:** Browser support and coverage limitations are visible. Sensitive-site exclusions and URL/title redaction happen before persistence where feasible. Do not collect credentials, form values, page contents, or private communications.  
**Failure cases:** Extension/connector absent, permissions denied, unsupported browser, disabled policy, redaction failure.  
**Acceptance:** Tests verify disabled policies and excluded domains do not persist prohibited metadata; UI communicates missing coverage rather than implying complete observation.

## 7. Dashboard and realtime

**Purpose:** Display device inventory, most recent observed application/browser activity, last event, heartbeat and freshness status.  
**Dependencies:** Authenticated API, event pipeline, tenant policy, realtime subscription.  
**Failure cases:** Outage, delayed event, dropped subscription, stale cached state.  
**Acceptance:** Stale/disconnected states are explicit; live changes originate from accepted endpoint events; reconnection restores state without cross-tenant leakage or duplicate activity.

## 8. Reporting and administration

Includes timelines, filters, application usage reports, policy-authorized browsing reports, CSV/optional PDF export, alerts, policies and audit logs. All queries and exports must re-check scope and permissions server-side, be rate/size limited, and audit sensitive access/export. Acceptance tests verify filter correctness, pagination, export redaction, permission enforcement and audit completeness.

## 9. Security and privacy baseline

- Disclose monitoring and establish organization authorization before enrollment.
- Collect only fields needed for documented purposes.
- Apply role/tenant checks on every endpoint, report, export, realtime channel, and background job.
- Use TLS, secure credential lifecycle, bounded event payloads, input validation, rate limits and idempotency.
- Provide configurable retention, deletion workflows, sensitive-site controls and auditability.
- Obtain qualified legal/privacy review for every deployment jurisdiction; this document is not legal advice.

## 10. Cross-cutting quality attributes

- Event correctness: unique event identity, bounded payloads, deduplication, explicit timestamp semantics.
- Reliability: retry with backoff, bounded local buffering, explicit health/freshness, replay protection.
- Observability: structured redacted logs, operational metrics, tracing/correlation identifiers.
- Accessibility and usability: distinguish observed, unknown, excluded, stale and disconnected states.
- Security testing: tenant isolation, privilege boundaries, enrollment abuse, event replay, exports and update integrity.

## 11. Not in initial baseline without separate approval

Hidden monitoring, keystroke capture, credential harvesting, collection of private messages/page contents, bypassing operating-system security controls, and covert screen capture are prohibited. Optional screen viewing requires a separately authorized design, explicit policy, visible access/audit records and legal/privacy review before implementation.
