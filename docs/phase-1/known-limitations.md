# Phase 1 — Known Limitations and Blockers

1. Employee activity, browsing history, screen viewing, enrollment, authentication, tenant business models and reports are not implemented.
2. PostgreSQL/EF Core business schema, migrations and database connection are deferred; API readiness currently checks the host health pipeline, not a database.
3. Authentication, RBAC and tenant isolation are not implemented; do not host sensitive business data on this foundation.
4. SignalR is planned but not connected.
5. The agent is a non-collecting scaffold and must not be deployed to employee endpoints.
6. A 90-day ordinary activity-event retention period is only a provisional development/testing design target. No event data is stored or deleted by this foundation. Production retention requires legal and organizational approval.
7. Exact supported Windows 10/11 editions/builds require current lifecycle and customer fleet confirmation before distribution.
8. The latest GitHub Actions run (40159780444) succeeded across all four jobs. However, the committed `apps/dashboard/pnpm-lock.yaml` is still incomplete: CI currently generates the complete resolution in the ephemeral runner and uploads it as an artifact. This must be committed and verified using frozen install only before sign-off.
9. The generated lockfile artifact was downloaded for inspection; it is 4,347 lines / 140,815 bytes, SHA-256 `64a4c74ced662cdef6a412c9c381f71bdd63f6445453a675c7ed7bf9aafdc5d6`. It has not yet been safely applied to the repo branch.
10. `main` was verified unprotected; required checks and PR review must be configured in GitHub settings.
11. No production readiness, legal approval, device credential lifecycle, signed release, or complete security-audit claim is made.
