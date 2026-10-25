# Phase 0.6 — Privacy, Data Governance and Retention

**Status:** Policy design baseline. Actual retention durations and legal obligations must be approved for each deployment jurisdiction and customer.

## Permitted purpose and notice

Monitoring is for disclosed enterprise purposes on authorized company-managed endpoints under written organizational policy. Provide notice describing collection categories, purposes, access roles, retention, exclusions, and a contact/escalation channel before enrollment or monitoring activation. Obtain qualified legal/privacy review before deployment.

## Data categories

| Category | Purpose | Access | Retention rule | Deletion / special handling |
|---|---|---|---|---|
| Device identity and health | Inventory, security and endpoint support | IT administrators; limited auditors | Defined operational period | Remove/revoke device; retain only required audit record |
| Application transition metadata | Disclosed application-use reporting | Authorized IT/manager/analyst according to role | Configurable category-specific duration | Scheduled deletion/aggregation with auditable outcome |
| Browser title/URL metadata | Only if approved and enabled by policy | Narrowly scoped authorized roles | Shortest justified approved period | Pre-persistence exclusion/redaction; scheduled deletion |
| Organization/user membership | Administration and access control | Organization admins and the user as appropriate | Account/contract lifecycle and required records | Disable access and delete/anonymize as policy/law allows |
| Monitoring policy and approvals | Prove authorized scope and configuration | Policy administrators/auditors | Policy and audit schedule | Retain necessary change history |
| Audit logs | Accountability and incident investigation | Restricted auditor/security role | Separately approved schedule | Controlled expiry; avoid logging sensitive content |
| Exports | Approved reporting | Creator and authorized roles only | Short expiry; secure storage | Expiring download access and scheduled object deletion |
| Optional screen viewing | Not in baseline; separate authorization required | Explicitly approved operators | Disabled by default; shortest approved duration | No recording by default; if later approved, strict expiry and audited deletion |

Specific durations must be configured and documented before live collection. This design does not silently set arbitrary retention periods.

## Access, export and employee process

- Define role access for IT, HR, managers and auditors by purpose and team scope.
- Log sensitive record access, export, policy edits and revocations.
- Provide an employee notice and documented route for questions, access/correction requests where applicable, and escalation.
- Require exports to be justified, minimized, access-controlled, time-limited and audited.
- Use redaction/exclusion for sensitive sites before durable storage where feasible.

## Screen-view safeguards

Screen viewing is excluded from the initial implementation baseline. If separately approved, require explicit role/purpose authorization, clear user/administrator visibility, short-lived sessions, visible audit events, no covert capture, no recording by default, and a reliable stop/revocation mechanism.

## Legal and governance gate

Obtain counsel/privacy review for each applicable jurisdiction and employment context. Validate lawful basis, notice/consultation obligations, cross-border transfers, data subject rights, works council/employee representative requirements and incident response duties as applicable. This document is not legal advice.
