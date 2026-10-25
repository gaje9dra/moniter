# Phase 0.7 — Test Strategy and CI Baseline

## Baseline before foundation implementation

The original repository had planning documents only and no runnable project or CI workflow.

## Current workflow

`.github/workflows/ci.yml` has four jobs:
- Dashboard: Node 24, pinned pnpm 10.20.0, dependency resolution and a frozen-lockfile verification, ESLint, TypeScript typecheck, Vitest, Next.js build.
- .NET Linux: .NET 10 restore, formatting verification, Release build, xUnit tests and vulnerable dependency listing.
- Windows agent: Windows runner restore, build and xUnit tests.
- Secret scan: Gitleaks scans checked-out working-tree files, avoiding the shallow history range issue seen in the first iteration.

## Actual CI evidence

- Initial run: https://github.com/gaje9dra/moniter/actions/runs/40158587979 — dashboard setup failed because pnpm was not installed before setup-node's pnpm cache; .NET formatter failed; Windows restore and secret scan did not complete successfully.
- Run: https://github.com/gaje9dra/moniter/actions/runs/40158715836 — dashboard frozen install caught incomplete placeholder lockfile; .NET format caught whitespace problems in API tests; Windows restore was on a commit before removal of an invalid package; Gitleaks failed on shallow Git history.
- Run: https://github.com/gaje9dra/moniter/actions/runs/40158782374 — secret scan succeeded (“no leaks found”); dashboard frozen install and lint/typecheck/unit tests succeeded after the workflow resolves the lockfile on runner; dashboard build, .NET formatting/build/test, and Windows job were still running at last inspection.

The current lockfile is being resolved during CI because direct execution/package resolution from the repository-writing environment is unavailable. A complete generated lockfile still needs to be committed to the branch to fully satisfy the committed lockfile requirement. Do not call the current lockfile artifact a fully verified committed dependency lockfile until that is done.

## Acceptance gate

A fresh workflow run must finish with all jobs successful. Check cancellations are not passes. Do not mark Phase 0 or Phase 1 complete until the current final commit has green test/build/security results and the generated lockfile has been committed.
