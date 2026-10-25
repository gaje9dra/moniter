# Phase 0.7 — Test Strategy and CI Baseline

## Original baseline
Before the foundation work, the repository contained documentation only. There were no application manifests, source code, tests, or GitHub Actions workflows, so a runnable baseline could not be measured.

## Current CI workflow
Workflow: [CI](../../.github/workflows/ci.yml). Latest verified successful run: https://github.com/gaje9dra/moniter/actions/runs/40159780444 on commit `3383108ad72c6917d0ccbc686a9c28aacff426f2`.

The workflow includes four jobs:
- **Dashboard:** Node 24, pnpm 10.20.0, dependency resolution/frozen install, ESLint, TypeScript typecheck, Vitest tests, and Next.js production build.
- **.NET:** .NET 10 restore, formatting verification, Release build, xUnit test command, and vulnerability listing for each of the four projects.
- **Windows agent:** Windows runner restores, builds and tests the agent.
- **Secret scan:** Gitleaks scans checked-out files using v8.24.3.

## Observed results
Run `40159780444` concluded **success**. All four jobs completed successfully:
- Dashboard / lint, typecheck, tests, build — success.
- .NET API + agent / restore, build, tests — success.
- Windows agent / Windows build and tests — success.
- Secret scan — success.

Earlier failed runs are retained in GitHub Actions history and were used to fix pnpm setup, lockfile resolution, C# formatting, xUnit imports, missing OpenAPI dependency, vulnerable OpenAPI dependency, audit asset discovery, and shallow-history secret scanning. Do not treat cancelled historical runs as passing.

## Dependency lockfile caveat
The dashboard CI currently resolves an incomplete checked-in lockfile during the run, validates the generated lockfile with a second frozen install, and uploads it as an artifact. The artifact was recovered, but its full content has **not yet been successfully committed** as the complete `apps/dashboard/pnpm-lock.yaml`. This is a remaining reproducibility blocker: a successful workflow does not prove that the committed lockfile alone is complete, because the first install updates it in the runner. Update CI to use frozen-lockfile only after the generated file is correctly committed.

## Sign-off gate
Although the latest observed CI run is green, Phase 0 and Phase 1 cannot be signed off until the full generated pnpm lockfile is committed, a fresh workflow passes against that committed lockfile without resolving/modifying it, test counts are confirmed, and the final PR/branch configuration is reviewed.
