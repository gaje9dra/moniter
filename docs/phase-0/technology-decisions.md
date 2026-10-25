# Phase 0.3 — Technology Decisions

**Decision status:** Preliminary recommendations only. The repository contains no source files or manifests, so no existing stack can be preserved or confirmed yet.

| Area | Candidate / principle | Rationale | Gate before adoption |
|---|---|---|---|
| Web dashboard | Existing framework if discovered; otherwise evaluate a mainstream typed web framework | Maintainability, accessibility, ecosystem | Compare team skills, hosting and security support |
| Backend API | Use one well-supported API stack; avoid duplicated business rules | Central authorization and contracts | Confirm concurrency, observability, deployment and ecosystem |
| Windows agent | Native Windows service/agent with visible installation and supported OS APIs | Reliable lifecycle and OS integration | Review least privilege, signing, uninstall, upgrade and privacy behavior |
| Database | PostgreSQL candidate, or existing supported database if later introduced | Transactions, constraints and indexed time-range queries | Capacity, hosting, backup and tenant-isolation review |
| Realtime | Authenticated WebSocket/SSE or managed equivalent | Near-real-time updates | Tenant filtering, reconnect semantics, backpressure and operational cost |
| Event processing | Durable database transaction initially; queue only when justified | Avoid premature infrastructure while preserving durable acceptance | Load tests and explicit delivery guarantees |
| Authentication | Established OIDC-compatible identity provider or existing secure auth | Avoid custom password/security implementation | Tenant membership model, MFA and deployment requirements |
| Secrets | Hosting secret manager and OS credential protection | Limit secret exposure | Rotation, access review, recovery |
| Distribution | Signed, versioned releases and staged rollout | Tamper resistance and rollback | Signing-key protection and update threat model |

Do not introduce technology based solely on this draft. Finalize choices after product deployment constraints, security review, team support capability, cost, licensing and actual repository state are known.
