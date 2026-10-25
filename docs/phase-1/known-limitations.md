# Phase 1 — Known Limitations and Blockers

1. No monitoring features are implemented: no employee activity, browser activity, screenshot, enrollment or reporting.
2. Database provider, PostgreSQL schema and EF Core migrations are deferred to the multi-tenant backend phase.
3. Authentication, RBAC and tenant isolation are not yet implemented; no sensitive business data should be hosted by this foundation.
4. SignalR is planned but not connected.
5. Agent has no activity collector or enrollment network client and is not intended for deployment to employee endpoints.
6. 90-day retention is a provisional development/testing design setting only. Production period requires legal/organizational approval and the persistence/deletion code does not yet exist.
7. Exact supported Windows editions/builds require fleet/lifecycle verification before distribution.
8. Playwright E2E, real Windows Service install/uninstall tests and signed agent releases are not yet implemented.
9. Main branch protection was observed disabled; required checks/reviews must be set in repository settings.
10. No workflow result is claimed green until GitHub Actions has actually run on the current commit.
