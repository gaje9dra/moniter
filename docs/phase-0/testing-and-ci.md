# Phase 0.7 — Test Strategy and CI Baseline

## Current CI workflow

Workflow: [CI](../../.github/workflows/ci.yml). It runs for pull requests, pushes to `main` or `feat/**`, and manual dispatch.

Required jobs:
- Dashboard frozen install, lint, typecheck, Vitest, and Next.js production build.
- .NET format, build, xUnit tests, and dependency vulnerability checks.
- Windows agent build and tests on a Windows runner.
- Gitleaks secret scanning.

## Lockfile recovery — committed and verified

The committed file `apps/dashboard/pnpm-lock.yaml` was recovered from artifact `12586430553` from workflow run [40159780444](https://github.com/gaje9dra/moniter/actions/runs/40159780444).

Verified details from successful recovery workflow [40162819427](https://github.com/gaje9dra/moniter/actions/runs/40162819427):
- Target branch: `feat/phase-1-foundation`.
- Lockfile commit: `0d988d74ceef679c54e18d30c54462499e83ca76`.
- Byte length: `140815`.
- SHA-256: `64a4c74ced662cdef6a412c9c381f71bdd63f6445453a675c7ed7bf9aafdc5d6`.
- The recovery job verified the downloaded artifact, committed the file, then fetched the remote file and verified its byte length and SHA-256.
- The temporary recovery workflow was removed from the recovery branch after successful verification.

The recovered lockfile is now present at [apps/dashboard/pnpm-lock.yaml](../../apps/dashboard/pnpm-lock.yaml). This confirms artifact transfer and remote-file integrity only; it does not prove that pnpm accepts the lockfile or that frontend checks pass.

## Latest CI status

The first CI workflow associated with lockfile commit `0d988d74ceef679c54e18d30c54462499e83ca76` is [run 40162832514](https://github.com/gaje9dra/moniter/actions/runs/40162832514). GitHub reports its conclusion as `action_required` and exposes no jobs for that run, so it is not evidence of either a green or failed build. A fresh actionable CI run is required.

Earlier CI runs failed before frontend checks because the committed lockfile contained the literal 7-byte placeholder `PENDING`. Those runs are historical evidence and must not be used as the result for the repaired commit.

## Required validation after recovery

Using pnpm `10.20.0`, run from the dashboard project directory or the documented workspace root as appropriate:

```bash
pnpm install --frozen-lockfile
pnpm lint
pnpm typecheck
pnpm test
pnpm build
```

Acceptance requirements:
- Clean frozen installation succeeds without regenerating the lockfile.
- The lockfile hash remains unchanged after installation.
- Lint, typecheck, unit tests, and the production build all execute and pass.
- The complete Linux and Windows CI pipeline passes on a fresh commit.
- Record actual job outcomes, test counts, commit SHA, and workflow URL.

## Remaining governance gates

Branch protection and required-check enforcement have not been verified because the connected integration returned HTTP 403 for the branch-protection endpoint. Manually inspect GitHub Settings → Rules/Branches.

Privacy/legal review, employee notice, supported Windows builds, production retention approval, and security review remain independent production-release gates. Phase 0 must not be signed off solely because the lockfile transfer succeeded.
