# Phase 0.7 — Test Strategy and CI Baseline

## CI workflow

Workflow: [CI](../../.github/workflows/ci.yml). The workflow is expected to run on pull requests and pushes to `main` or `feat/**`.

Jobs:
- **Dashboard:** Node.js 24, pinned pnpm 10.20.0, frozen dependency installation, ESLint, TypeScript typecheck, Vitest, and Next.js production build.
- **.NET API and agent:** .NET 10 restore, formatting verification, Release build, xUnit tests, and vulnerability listing for the API, agent and test projects.
- **Windows agent:** Windows runner restores, builds and tests the agent.
- **Secret scan:** Gitleaks scans checked-out files.

## Observed results and required correction

Run [40159780444](https://github.com/gaje9dra/moniter/actions/runs/40159780444) completed successfully on 2026-10-25. The dashboard job succeeded only after running `pnpm install --no-frozen-lockfile`, validating a second frozen install and uploading the generated lockfile as an artifact. That run does **not** prove that the committed lockfile alone is reproducible.

Run [40160244523](https://github.com/gaje9dra/moniter/actions/runs/40160244523) completed with failure on 2026-10-25. The dashboard failed at `pnpm install --frozen-lockfile`; the checked-in lockfile was incomplete. The .NET API/agent job, Windows-agent job, and secret scan succeeded. The failed dashboard install prevented lint, typecheck, frontend tests and build from running in that attempt.

The generated full lockfile was downloaded from run 40159780444 for recovery. A fresh workflow is required after that complete lockfile is committed and the CI step no longer mutates/regenerates it.

## Acceptance requirements

- CI must use `pnpm install --frozen-lockfile` as the only dashboard install path.
- CI must fail on any installation, lint, typecheck, test, build, .NET, Windows-agent, vulnerability-audit or secret-scan failure.
- No credentials or real endpoint devices are required for default checks.
- Windows-specific build and test results must originate from a Windows runner.
- Report actual test counts from job logs where accessible; do not infer counts from a green job summary.
- Do not call the baseline complete while a required check is pending, failed, skipped due to prerequisite failure, or not available for verification.

## Governance note

Branch-protection inspection returned HTTP 403 ("Resource not accessible by integration"). Required checks/reviews settings are unverified through the current connection; repository administrators must inspect GitHub Settings → Rules/Branches directly.
