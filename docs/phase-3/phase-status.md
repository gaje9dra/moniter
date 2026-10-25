# Phase 3 — Authentication and Authorization

**Implementation status:** Merged into `main` on 2026-10-25.
**Release status:** **BLOCKED — not production-verified; do not authorize Phase 4 yet.**

- Phase 3 pull request: https://github.com/gaje9dra/moniter/pull/4
- PR head: `f3ee9375a669615f8890d7b7d189a90206ef3277`
- Merge commit / current main at post-merge verification: `4ff27603c76f0fbc3a6442ae8857900fde8661f8`
- PR state: merged; GitHub reports the merge commit above.
- PR review records: no submitted reviews were returned by GitHub at verification.
- Branch protection: GitHub's `main` branch response reported `protected: false`, protection disabled, required status-check enforcement `off`, and no required contexts. The rulesets endpoint returned an empty list. Classic-protection detail could not be read by the current integration (403), so the branch response and empty ruleset response are the positive evidence available.
- Identity provider: none selected or provisioned in this implementation.
- Real-provider tokens/MFA: not verified.
- Independent security approval: not recorded.
- Post-merge workflow run discovery: the available commit-workflow lookup returned no runs for merge commit `4ff2760...`; do not infer post-merge CI success from that empty result.

## CI evidence on the exact PR head

These are completed, successful **pre-merge PR-head** workflow runs for `f3ee9375a669615f8890d7b7d189a90206ef3277`, not post-merge validation:

- Full repository CI (pull request): https://github.com/gaje9dra/moniter/actions/runs/40174807847 — passed.
- PostgreSQL migrations and tenant-isolation verification: https://github.com/gaje9dra/moniter/actions/runs/40174807842 — passed.
- The PR description also records full repository CI (push): https://github.com/gaje9dra/moniter/actions/runs/40174804847 — passed.
- Workflow evidence above covers the PR head only. Confirm a fresh workflow run for the merged `main` SHA before treating post-merge CI as verified.

## Implemented features

| Area | Implementation present in merged Phase 3 | Verification boundary |
|---|---|---|
| JWT bearer validation | Configured OIDC authority and API audience; signature, issuer, audience, lifetime, signing keys, and subject requirements | Automated pre-merge CI passed; real provider interoperability is unverified |
| Identity mapping | Exact validated issuer + subject matched to an active, pre-provisioned identity; no email-based linking or automatic account creation | Provisioning and real tokens remain external prerequisites |
| Organization/tenant resolution | Active identity, organization and membership resolution before server-owned tenant claims are established | PostgreSQL CI passed pre-merge; independent security review still needed |
| Claim trust boundary | Inbound organization, role, user-identity, tenant-context and Moniter MFA-marker claims are not accepted as authoritative | Regression tests exist; independent review remains open |
| Role authorization | Central member, manager, administrator and MFA-specific policies with authenticated fallback | Automated checks passed on PR head; future endpoints still need explicit narrow policies |
| Privileged MFA | Configured provider `amr`/`acr` assurance values for privileged roles and privileged membership listing | Actual provider semantics and tokens not verified |
| Audit logging | Structured security events designed to exclude tokens, raw provider claims and sensitive request contents | Production retention, access, alerting and export must be configured |
| Readiness | Readiness depends on database reachability and complete identity/MFA configuration | Must be verified against actual deployment configuration and provider |
| Database | No Phase 3 schema migration; existing identity and membership tables reused | PostgreSQL migration/tenant-isolation workflow passed pre-merge |

## Required release blockers

1. Select and provision the actual OIDC identity provider.
2. Configure `MONITER_AUTHORITY`, `MONITER_AUDIENCE`, and provider-specific `MONITER_MFA_AMR_VALUES` and/or `MONITER_MFA_ACR_VALUES` in the deployment secret/configuration manager. Never commit credentials or secrets.
3. Provision test `UserIdentity` and active `OrganizationMembership` records through an approved, authorized process.
4. Execute authorized end-to-end verification with actual provider-issued access tokens: valid token, invalid signature, issuer/audience mismatch, expired token, missing/unknown subject mapping, inactive identity/membership/organization, forged tenant/role/MFA claims, and provider-confirmed MFA vs non-MFA sessions. Record provider version/configuration and sanitized results without exposing tokens.
5. Obtain independent security review of authentication, authorization, tenant isolation, MFA, error paths and audit logging. The PR author reviewing their own code does not satisfy this gate.
6. Configure enforceable `main` branch protection/rules requiring the repository's full CI and PostgreSQL tenant-isolation check, plus appropriate independent review. Verify enforcement after configuring it.
7. Run and verify full CI and PostgreSQL integration on the **actual merged `main` SHA** and record the workflow links/results.

## Phase 4 entry decision

**NOT AUTHORIZED.** Phase 3 is merged, but the external provider, actual-token/MFA verification, independent security review, branch protection, and post-merge CI evidence are unresolved. Do not create a Phase 4 implementation branch or change Windows endpoint-agent code until these gates are completed and explicitly reviewed.

When authorized, Phase 4 must follow `docs/phase-0/implementation-roadmap.md`: Windows endpoint agent lifecycle, managed installation, device identity, secure credentials and signed builds. First inspect existing agent architecture and identify the precise Phase 4 contract; do not invent enrollment, revocation, or backend API behavior belonging to Phase 5 or later.
