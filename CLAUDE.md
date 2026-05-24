# CacheManager.Redis — Claude Context

## What this repo is

A .NET Redis caching helper library. Source in `src/`, tests in `test/`, sample ASP.NET Core app in `Sample/`, docs in `docs/`. NuGet package ID: `CacheManager.Redis`. Current version: 2.1.1.

Key files:
- `src/CacheManager.Redis/Interfaces/IRedisCacheManager.cs` — public interface
- `src/CacheManager.Redis/Services/RedisCacheManager.cs` — implementation
- `src/CacheManager.Redis/Extensions/Setup.cs` — DI registration
- `src/CacheManager.Redis/RedisCacheMangerOptions.cs` — configuration options
- `src/CacheManager.Redis/CacheManager.Redis.csproj` — package metadata and version
- `src/CacheManager.Redis/README.md` — NuGet package README (distinct from root README)
- `CHANGELOG.md` — version history
- `docs/agent-os/BACKLOG.md` — task backlog (read by Orchestrate mode)

## Agent OS

This repo uses Agent OS v1. Read the spec before starting any work session:

**`docs/agent-os/AGENT-OS.md`** — complete specification  
**`docs/agent-os/README.md`** — quick-start reference

## How to start a session

State your mode and task at the top of your first message:

```
Mode: [Orchestrate | Explore | Investigate | Build | Release | Content]
Task: <what you want done>
Context: <any relevant background>
```

**Default mode if unspecified: Explore. No file edits in Explore.**

## Mode summary

| Mode | Use for | Can edit files? |
|---|---|---|
| Orchestrate | Task backlog management, workflow coordination | Yes — docs/agent-os/BACKLOG.md only |
| Explore | Understanding, mapping, questions | No |
| Investigate | Bug/behavior analysis, produces Investigation Note + Implementation Plan | Yes — docs/agent-os/runs/ only |
| Build | Implementing approved changes | Yes — src/, test/, Sample/ only |
| Release | Version bump, pack, publish | Yes — .csproj, CHANGELOG.md, CLAUDE.md |
| Content | Docs, samples, SEO articles | Yes — docs/, README.md, Sample/ |

## Hard rules (always active, regardless of mode)

- No git push, gh release, or dotnet nuget push without explicit per-action confirmation.
- Show the exact command before running any state-changing operation (git add, git commit, git tag, git push, dotnet pack, dotnet nuget push, gh release create).
- Build mode requires an approved Implementation Plan before any code changes.
- Release mode requires a completed Release Checklist before proceeding.
- NuGet publish requires the GitHub release to be live first.
- SEO and documentation content must only describe features that exist in the released package.
- If a gate condition fails, stop and state which condition is unmet. Do not suggest bypassing the gate.

## Branch model

All work starts on `dev`. PRs target `main`. Release mode runs only after the PR is merged and `main` HEAD is the merged feature commit, not stale. Do not run Release mode from `dev`.

## Artifacts

Session artifacts (investigation notes, plans, verification notes) are saved in `docs/agent-os/runs/` using the naming convention:

```
YYYY-MM-DD-<slug>-<type>.md
```

Examples: `2026-04-12-null-key-bug-investigation.md`, `2026-04-12-async-overload-plan.md`
