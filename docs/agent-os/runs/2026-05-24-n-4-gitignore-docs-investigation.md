# Investigation Note: N-4 — `.gitignore` silently excludes the entire Agent OS spec and `CLAUDE.md`

Date: 2026-05-24
Task: N-4 (from `docs/agent-os/BACKLOG.md`, Wave 3) — `.gitignore` ignores `/docs/` and `CLAUDE.md` but both directories' content is intended to be tracked
Mode at time of writing: Investigate

## What Was Investigated

- `.gitignore` — current content and full history (`git log --oneline -- .gitignore`)
- `git show 08d23477` — original `.gitignore` introduced 2024-05-11
- `git show dbf1a8c` — most recent `.gitignore` change (the bundled BUG-3 commit, today)
- `git ls-files docs/` — what's actually tracked under `docs/`
- `git ls-files CLAUDE.md` — whether the project bootstrap file is tracked
- `find docs -type f` — what exists on disk under `docs/`
- `docs/agent-os/AGENT-OS.md` § "Suggested Repository Structure" and § "Recommended First Iteration Plan" — declared intent for `docs/agent-os/` tracking

## Findings

### Root cause

The lines `/docs/` and `CLAUDE.md` were added to `.gitignore` in today's bundled commit `dbf1a8c` ("change log added, readme updated."). They were **not** present in the original `.gitignore` introduced by commit `08d23477` (2024-05-11). The original file contained only build-output and IDE-state exclusions:

```
/Sample/obj/        /Sample/bin/
/.idea/             /CacheManager.Redis.sln.DotSettings.user
/src/CacheManager.Redis/bin/    /src/CacheManager.Redis/obj/
/test/CacheManager.Redis.Tests/bin/    /test/CacheManager.Redis.Tests/obj/
```

The bundled commit added three lines:
```
+/artifacts/
+/docs/
+CLAUDE.md
```

`/artifacts/` is correct — that's the Release-mode pack output directory and should be excluded. The other two are wrong.

### Why this is wrong

`AGENT-OS.md` is explicit that `docs/agent-os/` is the audit trail and must be committed:

- § "Recommended First Iteration Plan", item 1: "~~Commit this spec.~~ Done — `docs/agent-os/` is committed." — this is now factually incorrect.
- § "Suggested Repository Structure" lists the whole `docs/agent-os/` tree with the comment "session bootstrap (read automatically by Claude Code)" for CLAUDE.md and `← session artifacts (committed)` for `runs/`.
- The Orchestrate-mode contract says "Orchestrate mode reads `BACKLOG.md`" — `BACKLOG.md` lives in `docs/agent-os/`, so it must be tracked for any cross-session continuity.

`CLAUDE.md` is the standard Claude Code project-bootstrap file — also intended to be committed so every contributor and every session inherits the same mode rules and gates.

### Scope of the breakage on disk

- `git ls-files docs/` returns **zero** results. Nothing under `docs/` is tracked.
- `git ls-files CLAUDE.md` returns **zero** results. The bootstrap file is not tracked.
- `find docs -type f` returns **31** files currently on disk:
  - `docs/agent-os/AGENT-OS.md`, `README.md`, `BACKLOG.md` (3)
  - `docs/agent-os/workflows/*.md` (15 workflow files)
  - `docs/agent-os/templates/*.md` (8 templates)
  - `docs/agent-os/runs/*.md` (4 artifacts: prior audit + the three I produced for BUG-3 today)
  - `docs/redis-cache-dotnet-guide.md` (the existing SEO article)

All 31 + `CLAUDE.md` would be lost on a fresh `git clone`. The Agent OS system effectively does not exist in the repository — it only exists in this working copy.

### Why the immediate fallout is masked

`git status` does not list these files because `.gitignore` matches them, so a casual `git status` looks clean. The breakage is invisible until someone clones fresh, or until someone runs `git ls-files` and notices the absence.

This session's own work was also affected: the `BACKLOG.md` update earlier, the BUG-3 Investigation/Plan/Verification artifacts, and the file you are reading right now all sit on disk but are invisible to `git status` and `git add .`.

### What about `/artifacts/`?

Should stay ignored. `/artifacts/` is the directory Release mode writes `.nupkg` to (`dotnet pack ... -o ./artifacts`, per `AGENT-OS.md` § "Step 6: Pack"). Generated build output, no value in committing.

## Reproduction Steps

1. From a clean clone of the repository (any branch through `dev`-as-of-commit-`dbf1a8c`):
   ```bash
   git clone <repo> && cd CacheManager.Redis
   ls docs/        # → only build output or nonexistent
   ls CLAUDE.md    # → No such file or directory
   ```
2. **Expected** (per `AGENT-OS.md`): `docs/agent-os/` tree present, `CLAUDE.md` present, so future Claude sessions can find the spec and the backlog.
3. **Actual**: neither exists. New contributors and new Claude sessions have no spec, no backlog, no bootstrap — the entire Agent OS scaffolding is invisible outside this one working copy.

## Risk Surface

- **Public API surface:** unaffected.
- **Serialization / DI / tests:** unaffected.
- **Build:** unaffected (`.gitignore` doesn't influence compilation).
- **Loss-of-work risk:** **high**. Anyone running `rm -rf` on the working copy, or cloning fresh and switching machines, loses the entire Agent OS spec + history. No automatic recovery.
- **Session continuity:** every future Investigate / Build / Content session output also lands in `docs/agent-os/runs/` and will be silently ignored until N-4 is fixed.
- **Coupling:** fixing N-4 unlocks committing the in-flight BACKLOG update, the BUG-3 artifacts produced earlier today, and this artifact itself. None of those can be committed until the gitignore is corrected (or every commit uses explicit paths and the gitignore continues to silently shadow them — fragile).
- **No risk of accidentally tracking the wrong thing:** the gitignore still excludes `bin/`, `obj/`, `.idea/`, `.DotSettings.user`, and `/artifacts/`. None of those live under `docs/` or share a name with `CLAUDE.md`.

## Recommendation

**Fix.** Remove the two erroneous lines from `.gitignore`. Keep `/artifacts/`. After the fix, the user runs an explicit `git add` of the recovered files in a single follow-up commit (the actual `git add` is out of scope of the Implementation Plan because it requires user-confirmed staging decisions for ~32 files and is a Hard-Rule-sensitive operation).

An Implementation Plan follows: `docs/agent-os/runs/2026-05-24-n-4-gitignore-docs-plan.md`.

Also worth a separate **Orchestrate-mode** follow-up after N-4 lands: a one-off pass to stage and commit all currently-untracked-but-tracked-intent files (the 31 docs files + `CLAUDE.md`) in a coherent commit. Not part of this BACKLOG entry — should be its own line item (e.g., `N-4-recovery`).
