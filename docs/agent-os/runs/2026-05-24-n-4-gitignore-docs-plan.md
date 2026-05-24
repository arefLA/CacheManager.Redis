# Implementation Plan: N-4 — Stop ignoring `docs/` and `CLAUDE.md`

Date: 2026-05-24
Investigation Note: `docs/agent-os/runs/2026-05-24-n-4-gitignore-docs-investigation.md`
Approved by: pending

## Problem

`.gitignore` currently excludes `/docs/` and `CLAUDE.md` — both are intended to be tracked per `AGENT-OS.md`. As a result, the entire Agent OS spec, workflows, templates, backlog, run artifacts, the existing SEO article, and the project's CLAUDE.md bootstrap file all sit on disk but have never been committed. A fresh clone loses everything.

## Solution

Remove two lines from `.gitignore`. Keep `/artifacts/` (correct — it's Release-mode pack output). No other change to `.gitignore`. No source/test/sample/README/CHANGELOG changes.

The actual `git add` and `git commit` of the 31 currently-untracked-but-now-trackable files plus `CLAUDE.md` is **out of scope** for this plan — it's a separate human-confirmed staging operation handled in a follow-up task (recommended as `N-4-recovery` in BACKLOG).

## Files to Change

| File | Change |
|------|--------|
| `.gitignore` | Remove the lines `/docs/` and `CLAUDE.md`. The file is currently 11 lines (10 entries); after the change it should be 9 lines (8 entries). |

### Exact replacement

The full target content of `.gitignore` after this plan is applied:

```
/Sample/obj/
/Sample/bin/
/.idea/
/CacheManager.Redis.sln.DotSettings.user
/src/CacheManager.Redis/bin/
/src/CacheManager.Redis/obj/
/test/CacheManager.Redis.Tests/bin/
/test/CacheManager.Redis.Tests/obj/
/artifacts/
```

(No trailing newline change — match the existing convention of the file.)

## New Files

None.

## Public API Changes

None. This is a `.gitignore` change. No code, no interfaces, no public surface, no tests.

Breaking: No.

## Test Plan

- [ ] After the edit, run `git status` — all 31 files under `docs/` and `CLAUDE.md` should now appear as untracked (`??`).
- [ ] Confirm `/artifacts/` is still ignored: `git check-ignore -v artifacts/anything.nupkg` should still show the rule matching.
- [ ] Confirm build/test side: `dotnet build` and `dotnet test` are unaffected (no code change), but run them once to be safe.
- [ ] No new tests are added or modified — this is a repo-config change.

## Out of Scope

- **Not** running `git add docs/` or `git add CLAUDE.md`. That belongs to a separate `N-4-recovery` task with explicit human confirmation per the Hard Rules. The scope of *what* to recover is straightforward (everything currently on disk under `docs/` plus `CLAUDE.md`) but the staging operation is intentionally a human decision point.
- **Not** committing this `.gitignore` change itself — same reason. Build mode applies the edit; user reviews and commits.
- **Not** removing `/artifacts/` from `.gitignore` — that rule is correct.
- **Not** changing any other gitignore rule (`bin/`, `obj/`, `.idea/`, `.DotSettings.user`). All other rules are correct.
- **Not** touching any other file. No `AGENT-OS.md` updates, no README changes, no CHANGELOG edit, no source-code touch.
- **Not** rebasing or reordering the bundled commit `dbf1a8c` that introduced the bad lines. History stays as-is; this fix is a forward correction.
- **Not** deciding whether to commit this on `hygine-repo-health` or after switching to `dev`. That's a branch-strategy decision for the human at apply time.
