# Phase 0.4 — Proposed Data Model

**Status:** Logical proposal, not a migration. Match existing schema before implementation; repository currently has no schema to inspect.

## Entities and relationships

- **Organization**: tenant root; organization ID, name, status, created/updated timestamps.
- **Department**: belongs to organization; optional parent department.
- **User**: identity mapped from trusted authentication subject; avoid duplicate credentials in app tables.
- **Membership**: user-to-organization link with role and status; unique organization/user pair.
- **ManagedDevice**: belongs to organization, optionally assigned to employee; device status, OS/agent versions, enrolled/last-seen/revoked timestamps.
- **DeviceEnrollment**: one-time enrollment token hash, intended organization, expiry, redeemed/revoked timestamps; never store bearer token plaintext.
- **MonitoringPolicy**: organization-scoped version, effective period, approved collection flags, exclusions, retention reference and approver.
- **AgentHeartbeat**: device, server-received time, agent version, health/status fields.
- **ActivityEvent**: organization/device, unique event ID, sequence, kind, capture/receive times, policy version, minimized structured payload.
- **ApplicationUsage**: derived application/time interval or aggregate, with provenance to source events and idempotent rebuild strategy.
- **BrowserActivity**: policy-enabled, redacted browser metadata and capture/receive timestamps.
- **Alert**: organization, severity, status, trigger and resolution/audit timestamps.
- **AuditLog**: actor, organization, action, target, timestamp, correlation ID, redacted metadata.
- **RetentionPolicy**: organization and data category, retention duration, effective time and deletion workflow state.

## Constraints and indexes

- Stable primary keys; foreign keys preserve ownership relationships.
- Unique membership constraint on (organization_id, user_id).
- Unique device-scoped event identity on (device_id, event_id).
- Enrollment token hashes unique; enrollment token redemption must be atomic and expire.
- Index devices by (organization_id, status) and (organization_id, last_seen_at).
- Index events by (organization_id, device_id, occurred_at) and (organization_id, received_at); evaluate time partitioning only from measured volumes.
- Index browser records by (organization_id, device_id, occurred_at); apply retention and exclusion controls.
- Index audit logs by (organization_id, occurred_at) and target/action fields as query patterns require.
- Use foreign keys or carefully documented cascades; retention deletions must not erase required audit evidence without an approved retention/legal process.

## Tenant isolation and classification

Tenant-owned tables carry organization ownership either directly or via a constrained parent. Derive ownership in trusted server code; consider PostgreSQL row-level security as defense in depth if PostgreSQL is chosen. Treat browser titles/URLs and employee activity as sensitive monitoring data. Keep logs and analytics payloads minimized and redact sensitive values.

## Retention and migration requirements

Set per-category retention durations with legal/privacy review; define deletion jobs as idempotent, observable and auditable. Make schema changes via reviewed, forward/backward-compatible migrations. Test fresh database setup and upgrade from every supported baseline. No schema migration should be created until the actual implementation stack and database have been selected.
