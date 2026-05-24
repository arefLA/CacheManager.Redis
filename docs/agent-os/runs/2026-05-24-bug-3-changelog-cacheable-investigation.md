# Investigation Note: BUG-3 — CHANGELOG describes `[Cacheable]` feature that never shipped

Date: 2026-05-24
Task: BUG-3 (from `docs/agent-os/BACKLOG.md`) — Audit and correct CHANGELOG entries for `[Cacheable]` / `FromModel` / `FromRouteOrQuery`
Mode at time of writing: Investigate

## What Was Investigated

- `CHANGELOG.md` — current v1.3.0 and v2.1.0 entries
- `git log --all -S "Cacheable" / "FromModel" / "FromRouteOrQuery"` — locate every commit referencing the type names
- `git branch -a` — enumerate all local and remote branches
- `git ls-tree -r v1.3.0`, `v2.1.0`, `v2.1.1` — verify file presence at each release tag
- `git diff v1.2.0..v1.3.0 --name-status` — confirm what files actually shipped in v1.3.0
- `git diff v2.0.0..v2.1.0 --stat` — confirm what files actually shipped in v2.1.0
- `git diff v1.2.0..v1.3.0 -- src/.../IRedisCacheManager.cs` — list real API additions in v1.3.0
- `git diff v1.2.0..v1.3.0 -- test/.../RedisCacheManagerTests.cs` — list real test additions in v1.3.0
- `git diff main..12-cache-manager-attribute --name-only` — list files unique to the unreleased feature branch

## Findings

### Root cause

The `[Cacheable]` action filter, `FromModel`, and `FromRouteOrQuery` features exist only on the branch `12-cache-manager-attribute` (present locally and at `origin/12-cache-manager-attribute`). The branch was never merged into `main` or `dev`. The CHANGELOG entries for v1.3.0 and v2.1.0 describe this branch's work as if it had shipped — it has not.

### Evidence

**1. All four `Cacheable`-related commits live on a single branch that's unmerged:**

```
d666c55  Cacheable Action Filter Implemented with static key
bbac702  Cacheable key type implemented now the key can be provided from query, route or a binded model
a5d77f9  cleaning
b2400aa  Prefix option added for attribute, system provides default prefix for FromModel and FromRouteOrQuery
```

`git branch --contains 12-cache-manager-attribute` reports only `12-cache-manager-attribute` itself. Last commit on the branch: `a5d77f9` (2024-11-20). The branch has been dormant for ~18 months.

**2. The branch contains real source files that do not exist anywhere on `main`:**

```
src/CacheManager.Redis/attributes/CacheableAttribute.cs
src/CacheManager.Redis/Enums/CacheableKeyType.cs
```

(Output of `git diff main..12-cache-manager-attribute --name-only` shows additional files unique to that branch — see investigation log.)

**3. No release tag contains any Cacheable-related source:**

`git ls-tree -r v1.3.0 | grep -iE "cacheable|attribute"` returns no results. Same for v2.1.0 and v2.1.1. The tagged file set across all three releases has no Cacheable-related code.

**4. The actual content of v1.3.0 (`git diff v1.2.0..v1.3.0`) is entirely different from what the CHANGELOG claims:**

Files changed:
```
A   .gitignore
M   src/CacheManager.Redis/CacheManager.Redis.csproj
A   src/CacheManager.Redis/Extensions/Helpers.cs
M   src/CacheManager.Redis/Interfaces/IRedisCacheManager.cs
M   src/CacheManager.Redis/Services/RedisCacheManager.cs
M   test/CacheManager.Redis.Tests/Fakers/FakeCustomCacheManager.cs
M   test/CacheManager.Redis.Tests/Services/RedisCacheManagerTests.cs
```

API additions in v1.3.0 (from the interface diff):
```
bool TrySet(string key, TEntity entity);
bool TrySet(string key, TEntity entity, DistributedCacheEntryOptions options);
bool TryRefresh(string key);
bool TryRemove(string key);
```

Test additions in v1.3.0 (selected, ~20+ new tests):
- `TrySet_*`, `TryRefresh_*`, `TryRemove_*` happy and null-key paths
- `Set_/SetAsync_/Refresh_/RefreshAsync_/Remove_/RemoveAsync_` consistent `ArgumentNullException` / `ArgumentException` coverage via Ardalis guards

So **v1.3.0 actually shipped Try-API completion and guard-clause adoption**, not anything attribute-related.

**5. The actual content of v2.1.0 (`git diff v2.0.0..v2.1.0`) is also unrelated to `[Cacheable]`:**

Major changes:
- `README.md` — 299-line rewrite
- `Sample/Controllers/MainController.cs` — controller updates
- `Sample/Sample.http` — removed
- `src/CacheManager.Redis/Extensions/Setup.cs` — +46 lines (DI registration + scope fix)
- `src/CacheManager.Redis/Extensions/TypeExtensions.cs` — +5 lines (BaseType null safety)
- `src/CacheManager.Redis/Services/RedisCacheManager.cs` — +138 lines (sealed class, primary constructor, binary serialization refactor)
- New `test/.../Extensions/HelpersTests.cs`
- Expanded `SetupTests`

No Cacheable-related additions whatsoever. The "prefix support" claim is fictional with respect to what shipped.

### What is wrong, line by line

**CHANGELOG.md v1.3.0 — `Added` section:**
```
- `[Cacheable]` action filter with static key support.
- Cache key derivation from request route and query parameters via `FromRouteOrQuery`.
- Cache key derivation from a bound model via `FromModel`.
```
None of these shipped in v1.3.0. They describe `12-cache-manager-attribute` branch work.

**CHANGELOG.md v2.1.0 — `Added` section:**
```
- Cache key prefix support on `[Cacheable]` attribute. The system provides default prefixes for `FromModel` and `FromRouteOrQuery`.
```
The `[Cacheable]` attribute does not exist in the v2.1.0 release artifact, so prefix support on it cannot exist either.

## Reproduction Steps

1. Install `CacheManager.Redis` 2.1.1 from NuGet (or check out tag `v2.1.1` locally).
2. Search the installed assembly / package contents for any type named `CacheableAttribute`, `Cacheable`, `FromModel`, or `FromRouteOrQuery`.
3. **Expected** (per CHANGELOG v1.3.0 and v2.1.0): types exist; `[Cacheable]` attribute is usable in controllers.
4. **Actual**: types do not exist; `using CacheManager.Redis.attributes;` fails to resolve; user reading the CHANGELOG concludes a feature is missing or broken.

## Risk Surface

This is a documentation/trust issue. No source files need to change.

- **Public API surface:** unaffected (the feature does not exist; correcting the CHANGELOG cannot change what already shipped).
- **Serialization behavior:** unaffected.
- **Registration / DI lifetime:** unaffected.
- **Existing tests:** unaffected.
- **External consumers:** anyone who chose this package on NuGet because of the CHANGELOG mentions of `[Cacheable]` was misled. Correcting the entries makes future evaluators see an honest picture. There is no risk of breaking a real caller because the type they think they're using does not exist.
- **Branch hygiene:** `12-cache-manager-attribute` remains as a candidate for future implementation. Out of scope for this fix, but worth tracking — recommend a follow-up backlog item to either resurrect, archive, or delete that branch.

## Recommendation

**Fix.** Replace both incorrect CHANGELOG entries with accurate descriptions of what actually shipped in those versions, derived from the `git diff` evidence above. No source-code changes needed. An Implementation Plan follows: `docs/agent-os/runs/2026-05-24-bug-3-changelog-cacheable-plan.md`.

Also worth a separate backlog entry (not in this fix): decide what to do with the dormant `12-cache-manager-attribute` branch — either ship it in a future minor version, archive it as a documented design exploration, or delete it.
