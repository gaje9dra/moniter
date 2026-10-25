# Phase 0.8 — Implementation Roadmap

## Approved technology baseline

- Dashboard: Node.js 24 LTS, pnpm 10.20.0, Next.js App Router, React, TypeScript.
- API and agent: C# / .NET 10 LTS; ASP.NET Core API and a Worker Service hosted as a Windows Service on Windows.
- Future database: PostgreSQL with Entity Framework Core and reviewed versioned migrations.
- Future real-time transport: SignalR with authentication and tenant/role-filtered subscriptions.
- Tests/CI: Vitest for frontend unit tests, xUnit for .NET tests, GitHub Actions on Linux and Windows, dependency audit and secret scan.

## Phase sequence

1. **Foundation and development environment:** minimal dashboard/API/agent foundation, environment validation, static checks, unit tests and CI.
2. **Multi-tenant backend and database:** PostgreSQL/EF Core tenant-aware schema, migrations, repositories and isolation tests.
3. **Authentication and authorization:** identity integration, membership, role matrix and privileged MFA.
4. **Windows endpoint agent lifecycle:** managed install, device identity, secure credentials and signed builds.
5. **Device inventory and health:** authorized enrollment/revocation, heartbeat, version and status.
6. **Application activity:** policy-enabled foreground application metadata and duration/idle rules.
7. **Browser activity:** approved supported-browser integration and sensitive-site redaction/exclusion.
8. **Live dashboard:** authenticated SignalR updates and freshness/reconnect handling.
9. **Optional authorized screen viewing:** separate approval and privacy/security gates; disabled by default and may remain out of scope.
10. **Productivity analytics:** approved aggregate definitions and interpretability/fairness review.
11. **Alerts and policies:** versioned policies, alerts, audit and delivery retry.
12. **Historical search/reporting:** authorized query, filtering and exports.
13. **Security hardening:** threat remediation, abuse testing and secure defaults.
14. **Performance/scalability:** measured workloads, indexes, backpressure and benchmarks.
15. **Enterprise deployment:** rollout, backup/restore, operations and support.
16. **End-to-end release verification:** production-like acceptance, rollback and explicit release sign-off.

Every phase requires its own prompt, implementation, tests and verified CI gates. No later-phase monitoring behavior is part of Phase 1.

## Mandatory CI rule

> DO NOT STOP WORKING BEFORE ALL REQUIRED CI CHECKS ARE GREEN AND ALL IMPLEMENTATION-RELATED ERRORS, FAILURES, AND ISSUES ARE FIXED. Keep investigating, implementing fixes, and rerunning the relevant checks until CI is green. Do not skip tests, weaken tests, suppress errors, bypass required checks, or claim success without actual verification.

If an external service, credential or infrastructure blocker prevents a required check, provide reproducible evidence and the exact action required to unblock it. Never fabricate a green result.
