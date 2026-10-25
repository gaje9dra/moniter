# Phase 1 — Known Limitations and Blockers

1. Employee activity, browser history, screen viewing, enrollment and reports are not implemented.
2. PostgreSQL/EF Core business schema and migrations are deferred; API readiness is not database readiness.
3. Authentication, RBAC and tenant isolation are not implemented; do not host sensitive business data in this foundation.
4. SignalR is planned but not connected.
5. The Windows agent is a non-collecting scaffold, not suitable for employee endpoint deployment.
6. 90-day activity-event retention is provisional for development/testing design; no event persistence/deletion job exists. Production duration requires organizational and jurisdiction-specific legal approval.
7. Supported Windows editions/builds require customer fleet and lifecycle verification before release.
8. Initial CI failures included missing pnpm setup, .NET formatting issues, a nonexistent test package reference, and Gitleaks failing to resolve a shallow merge diff. Corrections have been committed; the new workflow run must be checked for actual results.
9. Main branch protection was observed disabled; required checks/reviews must be configured in repository settings.
10. No production-readiness, legal approval, device credential security or agent signing claims are made.
