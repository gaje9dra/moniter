# Phase 0.3 — Technical Architecture

**Status:** Proposed architecture; not an audit of existing code. The repository is empty, so decisions remain subject to implementation constraints and stakeholder approval.

## Logical components

1. **Administration web app:** employee/device management, policies, dashboards, audit, reporting. Authenticated user sessions; no direct database access.
2. **Backend API:** authentication integration, tenant authorization, enrollment lifecycle, policy distribution, event ingestion, historical queries, exports and audit.
3. **Windows endpoint agent:** visible/managed installation, device identity, policy-controlled application observation, optional supported browser metadata integration, heartbeat, bounded encrypted local queue, signed updates.
4. **Relational database:** existing supported database if one is selected later; PostgreSQL is a candidate for transactional tenant-scoped records and indexed historical activity.
5. **Event processing:** validation, authentication, deduplication, policy enforcement, durable persistence and derived usage calculations.
6. **Realtime delivery:** authenticated tenant/role-filtered events; dashboard must reconcile with persisted state after reconnect.
7. **Operations:** CI/CD, secrets management, metrics, redacted logs, backups, restore tests and incident response.

## Architecture principles

- Enforce tenant boundaries server-side and, if PostgreSQL is selected, consider row-level security as defense in depth alongside application authorization.
- Never accept organization ownership solely from an agent or client payload; derive ownership from authenticated device identity.
- Separate high-volume append-oriented activity events from mutable device and policy state.
- Use versioned contracts and migrations; no undocumented coupling between agent and API.
- Realtime is a delivery optimization, not the system of record; persisted accepted events are authoritative.
- Use the repository's technology only once a real codebase exists. Avoid selecting new libraries until support, security, licensing and operational fit are evaluated.

## Event lifecycle

1. Organization administrator authorizes enrollment.
2. Enrollment exchange binds an approved endpoint to an organization and creates a device-scoped identity.
3. Agent requests policy; server authenticates the device, checks status and returns the approved versioned policy.
4. Agent emits minimized events with unique event IDs, sequence/capture timestamps and policy version.
5. API authenticates device, applies rate/size/schema checks and verifies policy/tenant association.
6. Event is deduplicated and persisted transactionally; derived records are updated idempotently.
7. Realtime notification is published only after persistence and scoped to authorized subscribers.
8. Dashboard updates the view and labels freshness based on heartbeat/event timestamps.
9. Historical reports query persisted events with tenant authorization and retention filtering.
10. Revocation disables policy/event access immediately and invalidates device credentials.

## Failure recovery

- Agent offline: bounded encrypted local queue, TTL and maximum size; discard according to policy and report dropped-event counters rather than buffering indefinitely.
- Retry: exponential backoff with jitter, server-directed retry hints and stable event IDs.
- Duplicate delivery: unique database constraint on (device_id, event_id), with safe idempotent acknowledgement.
- Out-of-order delivery: retain capture time and ingestion time separately; use a documented late-arrival policy.
- Clock drift: server receive time is separate; flag anomalous device clock offsets rather than rewriting original timestamps silently.
- Realtime loss: reconnect, re-authorize and refresh from persisted cursor/history.
- Database or queue outage: return retryable response, maintain bounded queue/backpressure, alert operators and avoid acknowledging events before durable acceptance.
- Compromised/revoked device: deny ingestion, invalidate credentials and create security audit record.
- Agent upgrade: signed releases, staged rollout, compatibility window, health reporting and rollback.

## Operations and recovery

Define backup RPO/RTO before production. Encrypt backups, restrict access, test restoration in an isolated environment, monitor ingestion latency/error rate/queue depth/heartbeat freshness, and document emergency revocation and incident procedures.
