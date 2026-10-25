# Phase 0.7 — Test Strategy and CI Baseline

## Remote baseline evidence (2026-10-25)

- GitHub repository: exists, public, default branch metadata is `main`.
- Contents endpoint: 404, “This repository is empty.”
- Commits endpoint: 409, “Git Repository is empty.”
- Branches endpoint: empty array.
- Actions runs endpoint: `total_count = 0`.
- No project files, package manifests, workflows, tests or migration files are available; lint/typecheck/test/build/migration commands cannot be executed against the empty remote repository.
- No successful CI result is claimed. Required checks and branch protection have not been verified from repository settings.

## Planned test layers

1. **Unit:** event validation, timestamps/clock drift, usage duration, redaction, policy evaluation, freshness thresholds, role policy.
2. **Integration:** database constraints, atomic enrollment, credentials/revocation, event ingestion/deduplication, retention deletions, exports/audit.
3. **Security:** tenant boundary matrix for every route/job/export/stream; RBAC; replay; rate limits; sensitive-domain exclusion; secret/dep scanning.
4. **Agent:** supported Windows versions, visible installation/uninstallation, secure credential storage, offline queue limits, policy changes, signed update verification.
5. **End-to-end:** authorized enrollment through actual endpoint event to correct authorized dashboard, revoke and ensure event access stops, verify reconnection/backfill and stale indicators.
6. **Reliability:** API/database/realtime outages, queue overflow, duplicate/out-of-order events, clock skew, restart/recovery.
7. **Performance:** measure ingestion, dashboard freshness and history queries under an agreed number of active devices and events. Set explicit SLOs before production.

## Proposed baseline commands

Once the stack is selected, document exact commands in repository scripts and CI. Common gate categories: formatting/lint, type or compiler checks, unit tests, integration tests against a disposable test database, migration validation, production build/package, dependency audit, secret scan, and agent release signature verification. Commands here are categories only, not claims that commands have run.

## CI policy

- Run required checks on pull requests and protected release branches.
- Require green status for all applicable checks before merge/release.
- Never skip tests, weaken assertions, suppress errors, or report a passing status without verified result.
- Distinguish code-caused failures from unavailable services/credentials/infrastructure and attach reproducible evidence.
- Test migration from a fresh database and supported previous schema.
- Preserve logs/artifacts without secrets or sensitive endpoint activity.

## Completion state

Baseline is **blocked**, not green, because there is no initial commit or workflow. Once code is committed, establish a real baseline and iteratively fix implementation-related failures.
