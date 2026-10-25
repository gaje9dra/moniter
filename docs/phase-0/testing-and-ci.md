# Phase 0.7 — Test Strategy and CI Baseline

## Baseline before foundation implementation

The repository originally had Phase 0/Phase 1 planning documents only: no application code, manifests, or workflow. No tests could be run on that baseline.

## Current foundation workflow

Workflow: `.github/workflows/ci.yml`. It runs:
- Dashboard: Node 24, pinned pnpm 10.20.0 installation, frozen-lockfile install, lint, TypeScript typecheck, Vitest tests, Next.js production build.
- .NET cross-platform: .NET 10 restore, formatting verification, Release build, xUnit tests and vulnerable dependency listing.
- Windows agent: .NET 10 restore, Release build and xUnit test on `windows-latest`.
- Secret scan: Gitleaks action, configured to scan checked-out tracked content.

## Actual execution evidence

Initial CI run: https://github.com/gaje9dra/moniter/actions/runs/40158587979

Observed failures in that run:
- Dashboard: `actions/setup-node` failed because pnpm was not installed before its cache setup, so lint/typecheck/tests/build did not execute.
- .NET: restore succeeded; `dotnet format --verify-no-changes` failed on unformatted source/tests, so build/test/audit steps did not execute.
- Secret scan: the Gitleaks action tried to scan a shallow PR merge range that was absent from the checkout and failed with an unknown revision. No claim of a clean secret scan can be based on that run.

Further iteration:
- CI now installs the exact pnpm version explicitly and no longer uses pnpm cache setup before pnpm exists.
- Added .editorconfig and formatted the C# files identified by the formatter.
- Removed a nonexistent `Microsoft.Extensions.Configuration.Memory` package reference from tests.
- Gitleaks scan configuration is being corrected to scan tracked files without a shallow-merge base dependency.
- Latest fixes must be revalidated by a fresh, completed workflow run before claiming any check passed.

## Acceptance and gates

A green workflow is required. Do not mark cancelled jobs as passing. Retain failures as evidence, correct their root cause and rerun. Branch protection was observed disabled and must be configured manually or through an authorized repository-settings action.
