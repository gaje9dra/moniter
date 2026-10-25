# Phase 1 — CI Workflow

Workflow: `.github/workflows/ci.yml`.

## Jobs
- **Dashboard:** Node 24, globally install pinned pnpm 10.20.0, resolve lockfile from exact package pins, frozen install, ESLint, typecheck, Vitest, Next build.
- **.NET cross-platform:** .NET 10 restore, format verification, Release build, xUnit, vulnerable dependency listing.
- **Windows agent:** restore/build/test on `windows-latest`.
- **Secret scan:** Gitleaks scans checked-out files using a pinned version, without needing a shallow Git-history merge base.

The workflow uses read-only repository permissions and does not require production credentials. Any failing command must fail the job.

## Branch protection
GitHub reported `main` unprotected during this implementation. Configure required checks and review in repository settings before merge. Do not assume branch protection is active.
