# Phase 0.2 — Product Requirements and Scope

**Status:** Approved foundation requirements; production requirements remain subject to organization and jurisdiction review.

## Product boundary

Moniter is planned as a corporate employee-monitoring and endpoint-management platform for authorized company-managed Windows 10/11 endpoints. The initial foundation includes a dashboard shell, API health endpoints and a non-collecting Windows agent scaffold only. It does not implement employee monitoring or business features.

## Roles and organization

Future roles: organization owner, IT administrator, manager, HR/analyst, auditor and device-scoped endpoint agent. Server-side authorization and strict tenant isolation must protect all organization, department, employee, device, event, report, export and realtime data. No authentication or tenant business model is implemented in Phase 1.

## Endpoint support

The initial intended OS family is Windows 10/11. Before release, verify exact editions/builds and current lifecycle requirements against the customer fleet. Device enrollment, assignment, revocation and health management are future phases.

## Activity and browser data

Future application activity is restricted to approved metadata necessary to calculate application transitions/durations under documented policies. Browser integration, when separately approved, may collect only explicitly permitted browser/tab metadata with clear coverage limitations, sensitive-site exclusions and redaction. Passwords, authentication tokens, private messages, form contents, and page/screen contents are excluded. No such collection is implemented in the foundation.

## Foundation dashboard and API

- Dashboard displays the actual API liveness response and explicit unavailable/error states.
- API exposes service metadata, liveness, readiness and Development OpenAPI documentation.
- API returns correlation identifiers and normalized unexpected-error responses.
- Windows agent validates configuration and reports version/diagnostics while supporting cancellation-based shutdown.
- No fake activity, demo employees or simulated monitoring events are presented as real.

## Privacy, security and retention

Organizational authorization and appropriate employee notice are required before future monitoring. Role-based access and auditability are mandatory for sensitive data. Ordinary activity events use a provisional 90-day retention configuration in development/testing design only; production duration requires jurisdiction-specific legal and organizational approval. Screen viewing is optional, disabled by default and outside this foundation.

## Failure scenarios and acceptance criteria

- API offline: dashboard clearly presents unavailable state within the bounded request timeout.
- Invalid API origin: configuration validation returns an actionable error without exposing secrets.
- Invalid correlation ID: server replaces it and does not reflect unsanitized input.
- Unexpected API exception: client receives generic problem JSON with correlation ID; internal logs exclude exception content/stack.
- Invalid agent settings: startup fails with a clear configuration message, without secrets.
- Agent stop signal: worker logs a shutdown event and honors cancellation.
- Foundation starts/builds/tests on appropriate hosted Linux and Windows runners with real CI evidence before phase sign-off.
