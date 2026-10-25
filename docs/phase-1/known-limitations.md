# Phase 1 — Known Limitations and Blockers

1. Employee activity, browsing history, screen viewing, device enrollment, authentication, tenant business APIs and reporting are not implemented.
2. Phase 1 has no PostgreSQL/EF Core integration; initial tenant-aware persistence is Phase 2 scope.
3. Authentication, RBAC and application-level tenant authorization are not implemented in the Phase 1 foundation. Do not host sensitive business data on the foundation alone.
4. SignalR is planned but not connected.
5. The agent is a non-collecting scaffold and must not be deployed to employee endpoints for monitoring.
6. A 90-day ordinary activity-event retention period is only a provisional development/testing design target. No event data is stored or deleted by the Phase 1 foundation. Production retention requires legal and organizational approval.
7. Exact supported Windows 10/11 editions/builds require current lifecycle and customer fleet confirmation before distribution.
8. The full `apps/dashboard/pnpm-lock.yaml` is committed on `main`; a fresh PR CI run passed frozen install, lint, typecheck, tests and build. See [run 40162888119](https://github.com/gaje9dra/moniter/actions/runs/40162888119).
9. `main` remains unprotected and required checks are not enforced. Configure branch protection before merging further phases.
10. No production readiness, legal approval, device credential lifecycle, signed release, or complete security-audit claim is made.
