# Phase 1 — Security Baseline

- Foundation collects no employee activity, browsing history, private messages, form contents, credentials or screen content.
- No production service credentials are required for local development or default CI.
- `MONITER_API_BASE_URL` remains server-side; HTTPS is required for production configuration.
- API correlation IDs are length- and character-validated.
- Unexpected client errors contain generic text and correlation ID; logger records exception type only, not its message or stack.
- Agent settings are validated; diagnostic logs contain version/environment only.
- CI has read-only contents permission except external action requirements and scans tracked files for accidental secrets.
- Dependency vulnerability query is a CI gate and may reveal advisories requiring remediation before passing.
- Windows Service package is pinned. Release signing, secure device credential storage, MFA, tenant isolation, database security and SignalR authorization are future security gates because they are not implemented.
- No claim of a full security audit is made from static scanning alone.
