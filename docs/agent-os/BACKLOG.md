# Task Backlog

All pending work for CacheManager.Redis is tracked here. Orchestrate mode reads this file to select and sequence tasks.

**Task types:** `bug` | `feature` | `investigate` | `refactor` | `test` | `docs` | `sample` | `content` | `release`  
**Priority:** `high` | `medium` | `low`  
**Status:** `pending` | `in-progress` | `complete` | `blocked`

**ID convention:** IDs cross-reference findings in the audit notes under `docs/agent-os/runs/`. Prefix meanings: `BUG` = confirmed bug · `MIS` = doc/package misalignment · `WEAK` = API weakness · `F` = feature · `S` = SEO content · `N` = finding new in 2026-05-24 refresh.

Source audits:
- `docs/agent-os/runs/2026-04-12-full-repo-audit-investigation.md` (BUG/MIS/WEAK/F/S items)
- Inline refresh on 2026-05-24 (N items)

---

## Pending

### Wave 1 — Truth fixes (doc accuracy, low risk)

| ID | Type | Priority | Title | Notes |
|---|---|---|---|---|
| ~~BUG-3~~ | ~~docs~~ | ~~high~~ | ~~Audit and correct CHANGELOG — `[Cacheable]` / `FromModel` / `FromRouteOrQuery` entries~~ | **Complete** — see entry in Complete table below. |
| BUG-4 | docs | high | Remove dead link to `docs/performance-and-observability.md` (or create the file) | Linked from both READMEs at line 198. |
| N-1 | docs | high | Remove or create `Sample/Sample.http` reference | Referenced in both READMEs at line 166; file does not exist. |
| MIS-1 | docs | high | Reconcile `net6.0` claim with `.csproj` `net8.0`-only | Either restore multi-targeting or update both READMEs (line 30). See also N-8 (framework currency). |
| MIS-2 | docs | medium | Fix `GetAsync` XML remarks ("return false" → "returns null") | `src/.../Interfaces/IRedisCacheManager.cs:28`. |
| MIS-3 | docs | high | Fix decorator example — current `LoggingCacheManager<T>` registration creates circular DI | Both READMEs line 128. Either rewrite with Scrutor `Decorate()` pattern or remove. |

### Wave 2 — Correctness

| ID | Type | Priority | Title | Notes |
|---|---|---|---|---|
| BUG-1 | bug | high | Fix `RemoveAsync_ShouldThrowArgumentException_WhenKeyIsEmpty` calling wrong method | Test at `test/.../RedisCacheManagerTests.cs:538` calls `RefreshAsync` instead of `RemoveAsync`. |
| BUG-2 | bug | high | `Set(key, entity, options)` should throw `ArgumentNullException` for null key | `src/.../Services/RedisCacheManager.cs:74` throws `ArgumentException` for both null and whitespace; contract requires `ArgumentNullException` for null. Replace manual guard with `Guard.Against.NullOrWhiteSpace`. |
| N-2 | bug | medium | Fix CS8618 warning in `Sample/Book.cs:6` | `string Name` lacks initializer; add `= string.Empty;` or mark `required` / nullable. |
| N-3 | bug | medium | Fix CS8604 in `Sample/Program.cs:9` (null connection string) | `GetConnectionString("Redis")` is `string?` passed to `AddRedisCacheManager(string)`. Either guard in the sample or make the public API accept `string?` and throw with a clear message. Affects copy-pasted user code. |
| BUG-1-test | test | medium | Add missing `RemoveAsync("")` test | Pair with BUG-1 fix; verify the guard is exercised. |
| N-7 | test | low | Add `GetAsync_ShouldReturnNull_WhenPayloadCannotBeDeserialized` | Async parallel of existing `TryGet` corrupt-payload test. |
| N-6 | test | low | Rename two tests that say "Refresh" but test Remove | `Remove_ShouldCallDistributedCacheRefresh_WhenCalled` (line 420) and `RemoveAsync_ShouldCallDistributedCacheRefresh_WhenCalled` (line 498). |

### Wave 3 — Hygiene & repo health

| ID | Type | Priority | Title | Notes |
|---|---|---|---|---|
| N-4 | refactor | high | Fix `.gitignore` — `/docs/` and `CLAUDE.md` are tracked but ignored | Blocks `git add .` for new files in `docs/agent-os/runs/`, new SEO articles in `docs/`, and updates to `CLAUDE.md`. Will silently break W-09 and W-10. Remove those lines or scope them more narrowly. |
| N-5 | refactor | medium | Gate `<GeneratePackageOnBuild>` to Release configuration | `dotnet build` (Debug) currently produces a `.nupkg` in `bin/`. Add `Condition="'$(Configuration)' == 'Release'"`. |
| MIS-4 | docs | medium | Differentiate root README vs NuGet README | Files are byte-identical today. NuGet README should drop repo-only links (test/, internal docs). |
| MIS-5 | docs | medium | Update `.csproj` `<Description>` to a proper NuGet-listing sentence | Current value is lowercase and generic. |
| MIS-6 | docs | medium | Expand `<PackageTags>` | Add `aspnetcore`, `distributed-cache`, `IDistributedCache`, `stackexchange-redis`, `caching`, `json`, `dotnet`. |

### Wave 4 — Small features (target v2.2.0, non-breaking)

| ID | Type | Priority | Title | Notes |
|---|---|---|---|---|
| F-01 | feature | medium | Add `GetOrSetAsync(key, factory, options?, ct)` | Highest-impact addition: collapses the most common 4-line pattern to one call. |
| F-02 | feature | medium | Add `TryRemoveAsync` and `TryRefreshAsync` returning `Task<bool>` | Completes Try-API symmetry across sync and async. |
| F-07 | feature | low | Add `RedisCacheManagerOptions` alias with `[Obsolete]` on misspelled name | Removes WEAK-3 friction without breaking callers. |
| WEAK-5 | feature | low | Surface deserialization failures (configurable handler or log hook) | Currently `JsonException` is swallowed silently in `Deserialize`. |

### Wave 5 — Larger / breaking (target v3.0.0)

| ID | Type | Priority | Title | Notes |
|---|---|---|---|---|
| F-03 | feature | medium | Restore multi-targeting (`net6.0;net8.0`) | Couples with MIS-1; closes the README mismatch. |
| N-8 | feature | medium | Investigate adding `net9.0` / `net10.0` to multi-target set | Expands install base for newer projects. |
| F-04 | feature | low | Add batch operations: `GetManyAsync`, `RemoveManyAsync` | Common when invalidating entity sets. |
| F-05 | feature | low | OpenTelemetry / DiagnosticSource integration | Emit cache hit/miss spans without requiring a decorator. |
| F-08 | feature | low | Cache-stampede protection (semaphore on first miss) | Prevents thundering herd on cold-start. |
| WEAK-4 | feature | low | Async `TryGetAsync` returning `(bool, TEntity?)` or equivalent | Removes sync/async asymmetry in the get path. |

### Wave 6 — Content (rolling, SEO opportunities)

| ID | Type | Priority | Title | Notes |
|---|---|---|---|---|
| S-01 | content | low | "Strongly-typed Redis caching in .NET 8 with System.Text.Json" | Keyword: strongly typed redis cache dotnet. |
| S-02 | content | low | "Testing Redis caching in .NET without a real Redis server" | Keyword: unit test redis cache dotnet. |
| S-03 | content | low | "IRedisCacheManager vs IDistributedCache" | Honest comparison; both have valid uses. |
| S-04 | content | low | "Multi-tenant Redis key isolation in .NET using InstanceName" | Keyword: redis key prefix dotnet multi-tenant. |
| S-05 | content | low | "Cache-aside pattern in ASP.NET Core step by step" | Full walkthrough with CacheManager.Redis. |
| S-06 | content | low | "Adding logging and metrics to Redis cache in ASP.NET Core" | Decorator pattern (requires MIS-3 fix first). |
| S-07 | content | low | "Sliding vs absolute expiration in Redis: when to use each in .NET" | `DistributedCacheEntryOptions` deep-dive. |

---

## In Progress

| ID | Type | Title | Started | Artifact |
|---|---|---|---|---|

## Blocked

| ID | Type | Title | Reason |
|---|---|---|---|

## Complete

| ID | Type | Title | Version | Artifact |
|---|---|---|---|---|
| BUG-3 | docs | Correct CHANGELOG v1.3.0 and v2.1.0 entries (remove fictional `[Cacheable]` claims) | unreleased (on `dev` as of 2026-05-24, commit `dbf1a8c`) | `runs/2026-05-24-bug-3-changelog-cacheable-investigation.md` · `runs/2026-05-24-bug-3-changelog-cacheable-plan.md` · `runs/2026-05-24-bug-3-changelog-cacheable-verification.md` |
