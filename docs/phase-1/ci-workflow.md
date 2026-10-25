# Phase 1 — CI Workflow

Workflow: `.github/workflows/ci.yml`.

## Jobs
- **Dashboard:** Node 24, pnpm frozen-lockfile install, lint, typecheck, unit tests, production build.
- **.NET Linux:** restore solution, formatting verification, Release build, tests, vulnerable package audit.
- **Windows agent:** restore/build/test agent project on `windows-latest`.
- **Secret scan:** Gitleaks action against tracked files.

The workflow uses read-only repository permissions and does not require production secrets. A failed command fails its job. The workflow must be observed through an actual GitHub Actions run before describing any check as passed.

## Branch protection
At the time these documents were created, `main` was unprotected. Required checks/reviews must be configured in GitHub repository settings; if settings access is unavailable, follow the repository owner's manual configuration. Do not merge on the assumption that branch protection exists.
