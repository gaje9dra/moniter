# Phase 0.8 — Implementation Roadmap

## Approved technology baseline

- Dashboard: Node.js 24 LTS, pnpm 10.20.0, Next.js 15.5.6 App Router, React 19.1.1 and TypeScript 5.9.3.
- API and agent: C# / .NET 10 LTS; ASP.NET Core Web API and Worker Service hosted as a Windows Service on Windows.
- Future database: PostgreSQL with Entity Framework Core and reviewed, versioned migrations.
- Future realtime: SignalR with authentication and server-enforced tenant/role-filtered subscriptions.
- Tests/CI: Vitest for frontend unit tests, xUnit for .NET tests, GitHub Actions on Linux and Windows, dependency auditing and Gitleaks.

## Phase sequence and gates

1. **Foundation and development setup:** runnable dashboard/API/agent foundation, validated configuration, static checks, unit tests and verified CI.
2. **Multi-tenant persistence:** PostgreSQL/EF Core schema, reviewed migrations, repositories and isolation tests.
3. **Authentication/authorization:** identity integration, membership, role matrix and privileged MFA.
4. **Windows agent lifecycle:** managed install, device identity, secure credentials and signed builds.
5. **Device inventory/health:** authorized enrollment/revocation, heartbeat, version and status.
6. **Application activity:** policy-enabled foreground application metadata and duration/idle rules, only after authorization and privacy review.
7. **Browser metadata:** approved supported-browser integration, sensitive-site exclusions and redaction; no passwords, tokens, private messages or form contents.
8. **Live dashboard:** authenticated SignalR updates with freshness and reconnect handling.
9. **Optional screen viewing:** separate approval/privacy/security gate, disabled by default and may remain out of scope.
10. **Productivity analytics:** approved aggregate definitions and interpretability/fairness review.
11. **Alerts and policies:** versioned policies, audit and delivery retry.
12. **Historical search/reporting:** authorized query, filtering and exports.
13. **Security hardening:** threat remediation and abuse testing.
14. **Performance/scalability:** measured workloads, indexes, backpressure and benchmarks.
15. **Enterprise deployment:** rollout, backup/restore, operations and support.
16. **End-to-end release verification:** production-like acceptance, rollback and explicit release sign-off.

Each phase requires its own scope, implementation, tests and verified CI. No later-phase monitoring behavior is part of this foundation.

## Mandatory continuous-implementation and CI rule

DO NOT STOP WORKING BEFORE ALL REQUIRED CI CHECKS ARE GREEN AND ALL IMPLEMENTATION-RELATED ERRORS, FAILURES, AND ISSUES ARE FIXED. Keep investigating, implementing fixes, and rerunning the relevant checks until CI is green. Do not skip tests, weaken tests, suppress errors, bypass required checks, or claim success without actual verification.

If an unrelated pre-existing failure, unavailable external service, missing credential, or infrastructure problem prevents a green result, investigate it fully, fix it when within authorized scope, and provide exact evidence for any remaining external blocker. Never fabricate green CI results or conceal unresolved failures. Do not mark the phase complete while required acceptance criteria remain unmet.

## Release gates

The initial development environment must not require production credentials or real employee devices. Jurisdiction-specific legal review, employee notice, organizational authorization, exact supported Windows editions/builds and production retention-duration approval remain hard release gates.
