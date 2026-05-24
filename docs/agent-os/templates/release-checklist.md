# Release Checklist: v<version>

Date: YYYY-MM-DD
Releasing: CacheManager.Redis v<version>
Previous version: v<previous>
Release type: Patch | Minor | Major

---

## Pre-Release: Branch State

- [ ] PR from `dev` to `main` is merged
- [ ] Currently on `main`: `git branch --show-current` → `main`
- [ ] `main` HEAD is the merged feature commit, not stale: `git log --oneline -3`
- [ ] Working tree is clean: `git status` — no unexpected uncommitted files
- [ ] `./artifacts/` is empty or absent — no stale `.nupkg` from a previous run

## Pre-Release: Code and Tests

- [ ] `dotnet build src/CacheManager.Redis/CacheManager.Redis.csproj` — no errors, no warnings
- [ ] `dotnet test` — X passed, 0 failed, 0 skipped
- [ ] Verification Note(s) exist for all changes in this release

## Pre-Release: Version and Metadata

- [ ] `<Version>` in `CacheManager.Redis.csproj` updated to `<version>`
- [ ] `<Description>` in `.csproj` is accurate
- [ ] `<PackageTags>` in `.csproj` is accurate
- [ ] `CLAUDE.md` version updated: `Current version: <version>`

## Pre-Release: Documentation

- [ ] Root `README.md` reflects current API (check all code samples)
- [ ] `src/CacheManager.Redis/README.md` (NuGet README) reflects current API
- [ ] `CHANGELOG.md` entry written for `v<version>`
- [ ] Any new public API is documented in the README

## Pre-Release: Sample

- [ ] `dotnet build Sample/Sample.csproj` — no errors

## Package

- [ ] `dotnet pack -c Release` completed without warnings
- [ ] Package Validation Note written and saved to `docs/agent-os/runs/`
- [ ] `.nupkg` filename: `CacheManager.Redis.<version>.nupkg`
- [ ] `.nupkg` contains `src/CacheManager.Redis/README.md` (the NuGet README — not the root README)
- [ ] Target frameworks in `.nupkg`: `net6.0`, `net8.0`
- [ ] Dependencies in `.nupkg`:
  - [ ] `Ardalis.GuardClauses` 5.x
  - [ ] `Microsoft.Extensions.Caching.StackExchangeRedis` 8.x

## Release: GitHub

- [ ] Tag `v<version>` created on `main`
- [ ] Release commit pushed to `origin/main` — **explicit confirmation required**
- [ ] Tag pushed to origin — **explicit confirmation required**
- [ ] GitHub release created with changelog as body — **explicit confirmation required**
- [ ] `.nupkg` attached to GitHub release

## Release: NuGet

- [ ] NuGet publish command ready (API key confirmed available)
- [ ] `dotnet nuget push` run — **explicit confirmation required**
- [ ] Package visible on nuget.org (may take up to 15 min)
- [ ] NuGet README renders correctly
- [ ] Version badge on GitHub README (if present) reflects new version

## Post-Release

- [ ] Post-release note written in `docs/agent-os/runs/`
- [ ] Any follow-up tasks logged
