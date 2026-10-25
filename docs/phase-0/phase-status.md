# Phase 0 — Status and Sign-off

**Last updated:** 2026-10-25  
**Overall status:** INCOMPLETE — the dashboard lockfile is currently a `PENDING` placeholder, so frozen installation and full CI are not green.

## Current Phase 0.7 evidence

- Feature branch: `feat/phase-1-foundation`.
- Latest observed head before this status correction: `a9057e532b190a8c31ce0c860e906f2ea2fd00dc`.
- `apps/dashboard/pnpm-lock.yaml` was inspected directly and contains only `PENDING` (7 bytes; blob `ecae18482d4fba83f1708fc73f5b2d36212f9866`). This is invalid for a frozen install.
- The approved dashboard manifest pins `pnpm@10.20.0`, Next.js `15.5.6`, React/React DOM `19.1.1`, TypeScript `5.9.3`, ESLint `9.39.1`, and Vitest `3.2.4`.
- Workspace root configuration files `package.json`, `pnpm-workspace.yaml`, and `.npmrc` were not found in the feature-branch tree; the dashboard manifest is the only detected pnpm project manifest.
- Prior failing CI: [run 40160966800](https://github.com/gaje9dra/moniter/actions/runs/40160966800). The dashboard frozen install fails with `ERR_PNPM_LOCKFILE_MISSING_DEPENDENCY`; .NET, Windows agent, and secret-scan jobs passed.
- Earlier run [40159780444](https://github.com/gaje9dra/moniter/actions/runs/40159780444) passed frontend checks only after regenerating the lockfile inside CI. It is not proof of frozen installation from committed contents.
- Recovered artifact `12586430553` contains a 140,815-byte lockfile; offline YAML parsing found one root importer, 450 package entries, and 450 snapshots, with importer versions matching the dashboard manifest. The lockfile has not been committed successfully.
- This environment cannot resolve `github.com` for `git clone` or `registry.npmjs.org` to install pnpm; `corepack pnpm@10.20.0 --version` failed with DNS `EAI_AGAIN`. A GitHub blob-write attempt also yielded only 380 bytes rather than the full 140,815-byte file, so the truncated object was not committed.
- Branch protection inspection returned HTTP 403 `Resource not accessible by integration`; settings remain unverified.

## Phase gate table

| Subphase | Status | Evidence / blocker |
|---|---|---|
| 0.1 Repository discovery | Partial | Foundation code exists on feature branch; final tree must be revalidated after lockfile repair. |
| 0.2 Product/privacy requirements | Documented; approval pending | Organizational authorization, employee notice, jurisdiction review and production retention remain release gates. |
| 0.3 Technology decisions | Documented | Stack/version compatibility still needs clean CI confirmation. |
| 0.4 API/data contracts | Partial | Health endpoints exist; PostgreSQL/EF Core business model is deferred. |
| 0.5 Threat model | Partial | Foundation controls and secret scan exist; full production security review is pending. |
| 0.6 Privacy/retention | Documented; production approval pending | No activity data collection or storage implemented. 90 days is provisional for development/testing only. |
| 0.7 CI baseline | **BLOCKED** | Complete lockfile must be committed; fresh frozen-only install and all required jobs must pass. |
| 0.8 Roadmap | Documented | Later-phase monitoring features remain out of scope. |
| 0.9 Final validation | **NOT PASSED** | Do not sign off before green CI, review, and governance evidence. |

## Required next actions

1. Commit the full artifact `12586430553` as `apps/dashboard/pnpm-lock.yaml` without truncation, preserving pinned package versions.
2. From a clean checkout, run `pnpm install --frozen-lockfile` using pnpm `10.20.0`; verify the lockfile hash remains unchanged.
3. Run lint, typecheck, Vitest and production build, followed by all Linux and Windows CI jobs.
4. Update `docs/phase-0/testing-and-ci.md` and both PR descriptions/comments with exact run URL, job conclusions, test counts and commit hash.
5. Verify branch protection/required checks manually in GitHub Settings → Rules/Branches because the connected integration cannot read that endpoint.

**Sign-off: Phase 0 incomplete. Phase 1 remains gated. Do not start Phase 2 or claim a green CI result.**
