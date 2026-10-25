# Phase 0.6 — Privacy, Data Governance and Retention

**Status:** Foundation baseline decided; production legal approvals are still release gates.

## Permitted purpose and notice

Monitoring is restricted to disclosed enterprise purposes on authorized company-managed Windows endpoints, subject to approved organizational policy and applicable law. Provide notice describing collection categories, purposes, roles with access, retention, exclusions and escalation/contact channels before any future collection is enabled.

## Supported endpoint target

Initial intended target is authorized company-managed Windows 10 and Windows 11 endpoints. Before agent distribution, verify exact supported editions/builds, the Windows lifecycle status on the intended deployment date, service/session behavior and organization fleet compatibility. Unsupported or out-of-support operating systems must not be silently treated as supported.

## Data minimization and exclusions

This foundation does not collect employee activity. Later activity categories must have a specified purpose and schema before they are implemented. Never collect passwords, authentication tokens, private messages, form contents, or page/screen contents as part of the baseline. Browser metadata, if separately approved later, must use sensitive-site exclusions and redaction before persistence where feasible.

## Provisional retention baseline

Ordinary activity-event data has a provisional 90-day retention target for development and testing design only. Retention must be configurable by data category and enforced by future persistent storage/deletion jobs. No event store or deletion job is implemented in the foundation, so no activity data is currently retained by this application. Production retention duration requires jurisdiction-specific legal and organizational approval before deployment; 90 days is not a blanket legal approval.

## Roles and audit

Future access must be scoped to least-privilege roles for IT, managers, HR/analysts and auditors. Sensitive data access, exports, policy changes and device revocation must be audited. Tenant and role authorization must be enforced server-side across API calls, realtime subscriptions, exports and background jobs.

## Screen viewing

Screen viewing remains optional, disabled by default and outside the initial foundation. It requires a separate design, policy authorization, visible/auditable access, user/administrator notice as appropriate, session limits and an explicit stop/revocation mechanism.

## Production release gates

- Counsel/privacy review for each deployment jurisdiction and employment context.
- Confirm lawful basis, notice/consultation duties, cross-border transfer, employee rights and representative/works council requirements where applicable.
- Approve category-specific production retention durations and deletion evidence.
- Review monitoring purpose, browser exclusions and the exact Windows versions/editions in fleet.
- Validate role-scoped access, audit, incident response and data deletion before enabling real collection.

This document is not legal advice.
