# Phase 3 — Authentication and Authorization

**Implementation status:** Merged into `main`.
**Release status:** **BLOCKED — real identity-provider verification, independent security approval, and enforceable branch governance remain open.**
**Current verification baseline:** `72a515e5939a5ecfbb24fc6e19fda1bd3e17f37b` (2026-10-25).

## Evidence checklist

| Gate | Status | Evidence / closure condition |
|---|---|---|
| Phase 3 implementation merged | PASS | PR #4 merged at `4ff27603c76f0fbc3a6442ae8857900fde8661f8`: https://github.com/gaje9dra/moniter/pull/4 |
| Documentation and release-gate update merged | PASS | PR #5 merged at `72a515e5939a5ecfbb24fc6e19fda1bd3e17f37b`: https://github.com/gaje9dra/moniter/pull/5 |
| Full repository CI on current main | PASS | Run [40175666949](https://github.com/gaje9dra/moniter/actions/runs/40175666949), event `push`, branch `main`, exact head SHA `72a515e5939a5ecfbb24fc6e19fda1bd3e17f37b`, conclusion `success`. Jobs verified successful: dashboard lint/typecheck/tests/build; .NET restore/format/build/tests/package advisories; Windows agent restore/build/tests; secret scan. |
| PostgreSQL migrations and tenant-isolation CI on current main | PASS | Run [40175666947](https://github.com/gaje9dra/moniter/actions/runs/40175666947), event `push`, branch `main`, exact head SHA `72a515e5939a5ecfbb24fc6e19fda1bd3e17f37b`, conclusion `success`. PostgreSQL service initialized; restore, formatting, build, PostgreSQL-enabled tests and package-advisory check succeeded. |
| Branch protection / required-check enforcement | BLOCKED | Branch API reports `protected: false`, `protection.enabled: false`, enforcement `off`, no required contexts; rulesets endpoint returned `[]`. Classic protection detail endpoint returns 403 to the current integration, so configuration cannot be changed or authoritatively inspected through that endpoint. Administrator must enable and verify rules. |
| Identity provider selected/provisioned | BLOCKED | No real provider is selected/provisioned in the implementation evidence. Supply the provider authority/issuer, expected API audience, approved test tenant/app and authorized test accounts through deployment configuration. |
| Real signed-token interoperability | BLOCKED | No real provider-issued test tokens or sanitized end-to-end results have been supplied. Unit tests do not substitute for real issuer/discovery/signing-key/audience/lifetime verification. |
| Provider-specific MFA semantics | BLOCKED | No provider documentation/version or observed MFA/non-MFA token pair has been verified. Do not guess `amr` or `acr` values. |
| Independent security review | BLOCKED | GitHub review endpoint returned no submitted reviews for PR #4 or PR #5. An independent reviewer must assess the actual merged authentication and authorization code and record findings. |
| Phase 4 entry gate | BLOCKED | Phase 4 is not authorized until every mandatory gate above is resolved and evidenced. No Phase 4 implementation has been started as part of this verification. |

## Repository-side controls and test coverage inspected

Current merged code configures JWT bearer validation with issuer, audience, signature, signing key, lifetime, required expiry and signed tokens, a one-minute clock skew, disabled inbound claim mapping, disabled detailed auth errors and token saving. The token-validated event requires exactly one nonempty subject and a token issuer, then removes reserved server-managed claims and adds a server-managed trusted-issuer claim.

The organization middleware validates the organization selector's shape, resolves issuer + subject against a pre-provisioned active identity, checks active organization membership and organization status, replaces inbound tenant/role/MFA marker claims, and denies privileged roles absent configured MFA evidence. Health readiness requires authority, audience, HTTPS-compliant configuration, and at least one configured MFA assurance value.

Current automated coverage includes:
- `MfaAssuranceEvaluatorTests`: configured example `amr`/`acr` values, missing/unconfigured values, and rejection of internal Moniter MFA marker by itself.
- `OrganizationContextMiddlewareTests`: replacement of forged tenant/role/MFA claims and rejection of malformed organization selectors.
- `OrganizationContextResolverTests` (PostgreSQL): issuer + subject provisioned identity mapping, multiple-organization selection, and rejection of unauthorized organization selection/unknown subjects.
- `TenantIsolationTests` (PostgreSQL): tenant-scoped reads, cross-tenant writes/foreign-key restrictions, and fail-closed tenant context.
- `HealthTests`: liveness and safe correlation ID behavior.

These tests are evidence of repository-side implementation behavior, not a substitute for signed-token verification against the selected provider. Before production, add/execute integration cases using actual provider-issued tokens for valid tokens, invalid signatures, expired tokens, mismatched issuer/audience, missing or unknown subject mappings, inactive identities/memberships/organizations, forged tenant/role/MFA claims, and provider-confirmed MFA vs non-MFA sessions. Record sanitized outcomes only; never save tokens or secrets in repository or logs.

## Exact external actions required

1. **Identity provider owner:** choose and provision the OIDC/OAuth provider; supply `MONITER_AUTHORITY`, `MONITER_AUDIENCE`, and documented provider-specific `MONITER_MFA_AMR_VALUES` and/or `MONITER_MFA_ACR_VALUES` via the approved secret/configuration store. Keep HTTPS metadata validation enabled in production.
2. **Authorized test setup:** create least-privilege test application/accounts and seed approved test `UserIdentity` plus active/inactive `OrganizationMembership` and organization cases through the approved provisioning path.
3. **Security tester:** execute the token and MFA test matrix against the live configured provider. Attach workflow/test evidence and sanitized claims/assertions without tokens, secrets, personal details or credential material.
4. **Independent reviewer:** review JWT trust configuration, issuer/subject mapping, organization selection, tenant boundaries, role policy matrix, MFA assurance semantics, audit events, failure/readiness paths and deployment settings. Record review findings on an approved GitHub review or audit record. The implementation author must not approve their own changes.
5. **Repository administrator:** in repository Settings → Branches or Rulesets, add an active rule targeting `main`; require PRs, at least one approval from an independent reviewer, approval of the latest push as appropriate, and the exact successful required status checks from the full CI and PostgreSQL workflows. Prevent bypass where supported and verify the rule is active after saving. If the status-check context names differ from the workflow/job labels, use the exact check names reported by GitHub for runs above.
6. **Release owner:** verify those settings through repository UI or an authorized admin API and update this checklist with proof. Keep the phase blocked until all checks pass.

## Final decision

**Phase 3 implementation is merged and post-merge CI plus PostgreSQL verification PASS on `72a515e5939a5ecfbb24fc6e19fda1bd3e17f37b`. Production/release approval remains BLOCKED. Phase 4 is NOT AUTHORIZED.**

No production security approval, actual-provider MFA verification, or branch protection is claimed.
