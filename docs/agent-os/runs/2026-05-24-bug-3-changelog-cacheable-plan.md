# Implementation Plan: BUG-3 — Correct CHANGELOG entries for v1.3.0 and v2.1.0

Date: 2026-05-24
Investigation Note: `docs/agent-os/runs/2026-05-24-bug-3-changelog-cacheable-investigation.md`
Approved by: pending

## Problem

`CHANGELOG.md` v1.3.0 and v2.1.0 describe a `[Cacheable]` action filter and related `FromModel` / `FromRouteOrQuery` features that have never shipped in any released version. The feature lives only on an unmerged branch (`12-cache-manager-attribute`). Anyone reading the CHANGELOG on the repo or NuGet expects functionality that does not exist.

## Solution

Replace the inaccurate entries with descriptions of what each release actually contained, derived from `git diff` evidence captured in the Investigation Note. This is a documentation-only fix. No source files change.

The replacement text is fully specified below. Build mode should copy-paste it verbatim — do not paraphrase, do not add additional improvements, do not reorder unrelated entries.

## Files to Change

| File | Change |
|------|--------|
| `CHANGELOG.md` | Replace the v1.3.0 entry (lines 52–63 in current file) with the v1.3.0 replacement block below. Replace the v2.1.0 entry (lines 15–27) with the v2.1.0 replacement block below. No other entries change. No file metadata, headings, or version ordering changes. |

### v1.3.0 — replacement block

Replace the current v1.3.0 entry with:

```markdown
## [1.3.0] — 2024-05-11

### Added
- `TrySet(key, entity)` and `TrySet(key, entity, options)` — non-throwing write variants returning `bool`.
- `TryRefresh(key)` — non-throwing refresh variant returning `bool`.
- `TryRemove(key)` — non-throwing remove variant returning `bool`.
- `Helpers.HasValue` string extension method.

### Changed
- `Set` / `SetAsync` / `Refresh` / `RefreshAsync` / `Remove` / `RemoveAsync` now consistently throw `ArgumentNullException` for null keys and `ArgumentException` for empty/whitespace keys, via `Ardalis.GuardClauses`.

### Internal
- Expanded null-key and guard-clause test coverage across the API surface.
- Added repository `.gitignore`.
```

### v2.1.0 — replacement block

Replace the current v2.1.0 entry with:

```markdown
## [2.1.0] — 2026-02-13

### Changed
- Refactored `RedisCacheManager<T>` to a `sealed` class using a primary constructor.
- Switched serialization to `JsonSerializer.SerializeToUtf8Bytes` / `JsonSerializer.Deserialize<T>(byte[])`, avoiding intermediate string allocations.
- Hardened `TypeExtensions.IsAssignableToGenericType` against null `BaseType` traversal.
- Adjusted `AddRedisCacheManager` registration so `IRedisCacheManager<T>` is registered as scoped and the underlying `IRedisDistributedCache` as singleton (carrying serializer/options metadata).

### Internal
- Major README rewrite covering installation, registration, API usage, decorators, sample, and testing.
- Expanded unit-test coverage across `RedisCacheManager`, `Setup`, and `Helpers` (new `HelpersTests.cs`).
- Sample project cleanup: controller updates, `Sample.http` removed, `appsettings.Development.json` simplified.
```

Note for Build mode: there is intentionally no `### Added` section for v2.1.0. Inspection of `git diff v2.0.0..v2.1.0` shows no new public API surface was added in that release — every visible change is refactoring or internal/test work. Do not invent an Added section.

## New Files

None.

## Public API Changes

None. This is a documentation-only change to `CHANGELOG.md`. No interfaces, classes, methods, or DI registrations are touched.

Breaking: No.

## Test Plan

- [ ] Run `dotnet test` after the edit to confirm the build is unaffected. (Tests should all pass exactly as before — there is no code under change.)
- [ ] Manual diff review: confirm the only file modified is `CHANGELOG.md`, and within it only the v1.3.0 and v2.1.0 entry blocks change.

## Out of Scope

These items belong to other backlog entries and **must not be touched during this Build session**:

- Not deleting, archiving, or merging the `12-cache-manager-attribute` branch. (Recommend a separate backlog entry to decide its fate.)
- Not implementing `[Cacheable]`, `FromModel`, or `FromRouteOrQuery` in source.
- Not adding a `### Removed` section anywhere — these features were never released, so they cannot be "removed" from a CHANGELOG.
- Not touching `README.md` or `src/CacheManager.Redis/README.md` (those have their own dead-link and doc-accuracy backlog items: BUG-4, N-1, MIS-1, MIS-2, MIS-3).
- Not bumping the package version. Per AGENT-OS, CHANGELOG corrections that don't reflect a new release are not themselves a release; the next genuine release (e.g. v2.1.2 or v2.2.0) can incorporate this corrected history under its own version bump in Release mode.
- Not modifying any other CHANGELOG entry (v1.0.0, v1.1.0, v1.2.0, v1.4.0, v2.0.0, v2.1.1) — the investigation only confirmed inaccuracy in v1.3.0 and v2.1.0.
