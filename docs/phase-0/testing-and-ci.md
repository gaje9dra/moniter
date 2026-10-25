# Phase 0.7 — Test Strategy and CI Baseline

## Current CI workflow
Workflow: [CI](../../.github/workflows/ci.yml). The intended workflow runs on pull requests, pushes to `main` or `feat/**`, and manual dispatch.

Jobs include dashboard frozen install/lint/typecheck/Vitest/Next build; .NET format/build/xUnit/vulnerability checks; Windows agent build/tests on a Windows runner; and Gitleaks secret scanning.

## Current blocker

The actual feature branch contains `apps/dashboard/pnpm-lock.yaml` with the literal 7-byte content `PENDING`. A frozen install cannot succeed with this file. CI run [40160966800](https://github.com/gaje9dra/moniter/actions/runs/40160966800) failed the dashboard dependency install with `ERR_PNPM_LOCKFILE_MISSING_DEPENDENCY`; .NET API/agent, Windows agent, and secret-scan jobs passed.

The dashboard manifest declares `packageManager: pnpm@10.20.0`, Next.js `15.5.6`, React and React DOM `19.1.1`, TypeScript `5.9.3`, ESLint `9.39.1`, and Vitest `3.2.4`. A recovered artifact from prior run `40159780444` (artifact ID `12586430553`) contains a 140,815-byte lockfile. Offline YAML inspection reports one root importer, 450 package entries and 450 snapshots. Its direct importer declarations match the manifest, but it has not yet been committed correctly.

The prior run `40159780444` passed only after running `pnpm install --no-frozen-lockfile` inside the CI runner and uploading the generated artifact. That is not acceptable evidence of reproducibility from the committed lockfile. CI must retain `pnpm install --frozen-lockfile` as the only dependency installation path.

## Execution and transfer limitations encountered

- `git clone https://github.com/gaje9dra/moniter.git` fails because this environment cannot resolve `github.com`.
- `corepack pnpm@10.20.0 --version` fails because this environment cannot resolve `registry.npmjs.org` (`EAI_AGAIN`).
- The GitHub connector's blob-write attempt returned a 380-byte blob for the intended 140,815-byte lockfile, so the incomplete object was not used. The file has not been committed; do not infer successful transfer from artifact download alone.

## Acceptance checklist

- [ ] Commit the full generated `pnpm-lock.yaml` without truncation or placeholders.
- [ ] From a clean checkout, use pnpm 10.20.0 and run `pnpm install --frozen-lockfile` successfully.
- [ ] Verify the lockfile hash is unchanged after the install; repeat in a clean environment where possible.
- [ ] Run frontend lint, typecheck, Vitest and production build against the frozen install.
- [ ] Verify a fresh run has all dashboard, .NET, Windows and secret-scanning jobs green.
- [ ] Record actual test counts and the exact commit/run URLs.
- [ ] Keep Phase 0.9 incomplete until CI and repository governance evidence are verified.

## Governance

Branch-protection inspection returned HTTP 403 (`Resource not accessible by integration`); rules and required checks remain unverified. Manually inspect GitHub Settings → Rules/Branches. Privacy/legal review, employee notice, supported Windows builds and production retention approval are separate release gates.
