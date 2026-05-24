# Investigation Note — Full Repo Audit

**Date:** 2026-04-12  
**Mode:** Investigate  
**Task:** Full repo audit — bugs, weaknesses, doc/package misalignments, feature ideas, SEO content opportunities  
**Files investigated:** All source, all tests, both READMEs, CHANGELOG.md, Sample/, docs/

---

## What Was Investigated

- `src/CacheManager.Redis/` — all 8 source files
- `test/CacheManager.Redis.Tests/` — all test files and both `.csproj` files
- `README.md` (root) and `src/CacheManager.Redis/README.md` (NuGet)
- `CHANGELOG.md`
- `Sample/Program.cs` and `Sample/Controllers/MainController.cs`
- `docs/redis-cache-dotnet-guide.md`
- `docs/agent-os/BACKLOG.md`

---

## Section 1 — Confirmed Bugs

### BUG-1: Test covers wrong method — `RemoveAsync` empty-key path is untested

**File:** `test/CacheManager.Redis.Tests/Services/RedisCacheManagerTests.cs`, line 530  
**Test name:** `RemoveAsync_ShouldThrowArgumentException_WhenKeyIsEmpty`

The test body calls `redisCacheManager.RefreshAsync(string.Empty)` — not `RemoveAsync`. So `RemoveAsync("")` is never exercised, meaning the guard on `RemoveAsync` for empty keys has zero test coverage. The actual `RemoveAsync` implementation is correct (uses `Guard.Against.NullOrWhiteSpace`), but the missing test hides any future regression.

```csharp
// What the test says it tests:
Func<Task> act = async () => await redisCacheManager.RemoveAsync(null);

// What the empty-key test actually calls (line 538):
Func<Task> act = async () => await redisCacheManager.RefreshAsync(string.Empty); // BUG
```

---

### BUG-2: `Set(key, entity, DistributedCacheEntryOptions)` throws wrong exception type for null key

**File:** `src/CacheManager.Redis/Services/RedisCacheManager.cs`, line 73–76

All throwing overloads that receive a null key must throw `ArgumentNullException` per the interface XML docs. The `Set(key, entity, options)` overload uses `!key.HasValue()` + manual `throw new ArgumentException(...)`, which throws `ArgumentException` for *both* null and whitespace — violating the contract.

| Overload | Null key behaviour | Correct? |
|---|---|---|
| `Set(key, entity)` | `Guard.Against.NullOrWhiteSpace` → `ArgumentNullException` | ✅ |
| `Set(key, entity, options)` | `!key.HasValue()` → `ArgumentException` | ❌ |
| `SetAsync(key, entity, ct)` | `Guard.Against.NullOrWhiteSpace` → `ArgumentNullException` | ✅ |
| `SetAsync(key, entity, opts, ct)` | `Guard.Against.NullOrWhiteSpace` → `ArgumentNullException` | ✅ |

No test exists for `Set(null, entity, options)`, so this is currently silent. The fix is to replace the manual guard in that overload with `Guard.Against.NullOrWhiteSpace(key)`.

---

### BUG-3: `[Cacheable]` attribute and related types are claimed in CHANGELOG but do not exist in source

**CHANGELOG entries:**
- v1.3.0: "Added `[Cacheable]` action filter with static key support, `FromRouteOrQuery`, `FromModel`"
- v2.1.0: "Added: Cache key prefix support on `[Cacheable]` attribute"

**Reality:** Zero matches for `Cacheable`, `FromModel`, or `FromRouteOrQuery` in `src/` or `test/`. These types were either never merged into the branch being shipped, or were removed without a CHANGELOG `Removed` entry.

**Risk:** Users reading the CHANGELOG expect a `[Cacheable]` attribute to exist. It does not. This is a truthfulness and trust issue for anyone evaluating the package.

---

### BUG-4: Dead link — `docs/performance-and-observability.md` referenced in both READMEs but does not exist

**Files:** `README.md:198`, `src/CacheManager.Redis/README.md:198`

```markdown
See [`docs/performance-and-observability.md`](docs/performance-and-observability.md) for additional guidance.
```

The file `docs/performance-and-observability.md` does not exist in the repository. The link is broken in the root README and in the NuGet package README.

---

## Section 2 — Doc / Package Misalignments

### MIS-1: README claims `net6.0` support; `.csproj` only targets `net8.0`

**Files:** `README.md:30`, `src/CacheManager.Redis/README.md:30`, `CacheManager.Redis.csproj:10`

Both READMEs say: "Target frameworks: `net6.0`, `net8.0`."  
The `.csproj` has `<TargetFramework>net8.0</TargetFramework>` (singular — not multi-target).  
The test project also targets `net8.0` only.  
A `net8.0`-only package cannot be installed into a `net6.0` project via NuGet. Users on net6 will get an error. The CHANGELOG v2.0.0 says "net6.0 support retained" but the project file contradicts this.

Options: either restore multi-targeting (`<TargetFrameworks>net6.0;net8.0</TargetFrameworks>`) or update docs to say "net8.0 only."

---

### MIS-2: `GetAsync` XML doc remarks copied verbatim from `TryGet` — type mismatch

**File:** `src/CacheManager.Redis/Interfaces/IRedisCacheManager.cs`, line 28–30

```xml
/// <remarks>
/// return false if the key is null or whitespace
/// </remarks>
```

`GetAsync` returns `Task<TEntity?>`, not `bool`. "return false" is wrong; should say "returns null."

---

### MIS-3: Decorator example in both READMEs causes circular DI dependency

**Files:** `README.md:128–152`, `src/CacheManager.Redis/README.md:128–152`

The example shows `LoggingCacheManager<T>` taking `IRedisCacheManager<T>` as a constructor dependency, then being registered via `options.CustomImplementation = typeof(LoggingCacheManager<>)`. This replaces the `IRedisCacheManager<>` registration with `LoggingCacheManager<>`. When the container tries to resolve `LoggingCacheManager<T>`, it needs `IRedisCacheManager<T>`, which resolves to `LoggingCacheManager<T>` again — circular dependency exception at runtime.

The correct decorator pattern requires registering the inner implementation separately (e.g. using Scrutor's `services.Decorate()` or manually registering `RedisCacheManager<>` under a different key). The current example compiles but fails at runtime. This needs either a corrected code example or removal.

---

### MIS-4: Root README and NuGet README are 100% identical

**Files:** `README.md`, `src/CacheManager.Redis/README.md`

The AGENT-OS spec treats them as distinct. Both files are byte-for-byte identical today. This is a maintenance liability — any update requires touching two files. More importantly, the NuGet README should be optimised for the NuGet discovery context (less repo-context content, no links to `test/` folder).

---

### MIS-5: `.csproj` `<Description>` is lowercase and generic

**File:** `src/CacheManager.Redis/CacheManager.Redis.csproj:8`

```xml
<Description>a generic redis cache manager for .net applications</Description>
```

All-lowercase, starts with lowercase "a", uses ".net" instead of ".NET". On NuGet this appears verbatim in the package listing and search results. A more useful description that leads with the value proposition would help discoverability.

---

### MIS-6: `<PackageTags>` are too sparse

**File:** `src/CacheManager.Redis/CacheManager.Redis.csproj:16`

```xml
<PackageTags>redis, cache manager, generic</PackageTags>
```

Missing high-value NuGet search terms: `aspnetcore`, `caching`, `distributed-cache`, `stackexchange-redis`, `json`, `dotnet`, `IDistributedCache`. NuGet search uses tags; sparse tags reduce discoverability.

---

## Section 3 — API Weaknesses

### WEAK-1: No async Try variants for Remove and Refresh

`TryRemove` and `TryRefresh` exist in the sync API (return `false` for invalid keys, never throw). The async API has only `RemoveAsync` and `RefreshAsync` — both throw on invalid keys. There is no async non-throwing path. Callers who want "remove if key is valid, else do nothing" in async code must catch exceptions or validate the key themselves.

Missing: `TryRemoveAsync` and `TryRefreshAsync` returning `Task<bool>`.

---

### WEAK-2: No `GetOrSetAsync` helper

The most common use of a cache is: get if present, else compute and store. Today that requires 4 lines:

```csharp
var cached = await _cache.GetAsync(key, ct);
if (cached is not null) return cached;
var value = await ComputeAsync();
await _cache.SetAsync(key, value, ct);
```

A `GetOrSetAsync(string key, Func<CancellationToken, Task<TEntity>> factory, DistributedCacheEntryOptions? options, CancellationToken ct)` method would reduce this to one line and is the pattern every caller uses. This is the single highest-impact API addition.

---

### WEAK-3: Typo in options class name is a permanent friction source

`RedisCacheMangerOptions` (missing 'a') — acknowledged as "historical spelling" in docs. Every developer who types this misspells it. Without a sealed constructor or alias, there's no IDE correction. A properly-named alias (`RedisCacheManagerOptions`) with an `[Obsolete]` redirect on the old name would let users use the correct spelling while maintaining backwards compatibility until a major version.

---

### WEAK-4: No `CancellationToken` in sync `TryGet`

`TryGet` is sync-only. In an async controller, calling it blocks a thread while Redis responds. There's no async path that returns `(bool, TEntity?)` — callers must either use `TryGet` (sync) or `GetAsync` (async, returns nullable without the bool). The API asymmetry encourages mixing sync and async patterns incorrectly.

---

### WEAK-5: Serialization errors are silently swallowed in `TryGet`

**File:** `src/CacheManager.Redis/Services/RedisCacheManager.cs`, lines 157–165

`Deserialize()` catches `JsonException` and returns `null`. This means a corrupted cache entry silently returns `false`/`null` from `TryGet` — indistinguishable from a cache miss. There's no way for a caller to know whether the key was absent or whether the stored bytes couldn't be parsed. A log event or configurable error handler would give operators visibility.

---

## Section 4 — Feature Ideas for Future Versions

Listed in rough priority order (highest first):

| ID | Feature | Why |
|---|---|---|
| F-01 | `GetOrSetAsync(key, factory, options, ct)` | Eliminates the most common boilerplate; 1-liner cache-aside |
| F-02 | `TryRemoveAsync` / `TryRefreshAsync` | Completes the Try-API symmetry across sync and async |
| F-03 | Restored `net6.0` multi-targeting | Matches current README claim; expands install base |
| F-04 | Batch operations: `GetManyAsync`, `RemoveManyAsync` | Common when invalidating entity sets |
| F-05 | OpenTelemetry / DiagnosticSource integration | Emit cache hit/miss spans without requiring a decorator |
| F-06 | Configurable error handler on deserialisation failure | Currently swallowed silently; hook for logging/metrics |
| F-07 | `RedisCacheManagerOptions` alias (correct spelling) with `[Obsolete]` on old name | Remove friction without breaking callers |
| F-08 | Cache stampede protection (e.g. `SemaphoreSlim` on first miss) | Prevents thundering herd on cold-start |

---

## Section 5 — SEO Content Opportunities (Current Features Only)

All ideas below are grounded in features that exist today. No invented functionality.

| ID | Article / Guide Idea | Primary Keyword | Angle |
|---|---|---|---|
| S-01 | "Strongly-typed Redis caching in .NET 8 with System.Text.Json" | strongly typed redis cache dotnet | Replaces byte[] with IRedisCacheManager<T>; shows serializer config |
| S-02 | "Testing Redis caching in .NET without a real Redis server" | unit test redis cache dotnet | FakeCustomCacheManager / CustomImplementation swap pattern |
| S-03 | "IRedisCacheManager vs IDistributedCache — which to use in ASP.NET Core" | IDistributedCache vs redis dotnet | Honest trade-off comparison; both have valid uses |
| S-04 | "Multi-tenant Redis key isolation in .NET using InstanceName" | redis key prefix dotnet multi-tenant | InstanceName option, collision prevention, environment separation |
| S-05 | "Cache-aside pattern in ASP.NET Core step by step" | cache-aside pattern aspnetcore | Full walkthrough with CacheManager.Redis; sliding + absolute TTL |
| S-06 | "Adding logging and metrics to Redis cache in ASP.NET Core" | redis cache logging metrics aspnetcore | Decorator pattern (with corrected DI example) |
| S-07 | "Sliding vs absolute expiration in Redis: when to use each in .NET" | redis cache expiration dotnet | DistributedCacheEntryOptions deep-dive, practical examples |

The existing `docs/redis-cache-dotnet-guide.md` covers broad Redis/.NET ground. The S-xx articles above each cover a narrower, higher-intent keyword with a concrete how-to that links back to the package.

---

## Risk Surface

- **BUG-3** (`[Cacheable]` in CHANGELOG but absent in code) is the highest trust risk. Users evaluating the package via CHANGELOG believe they're getting an attribute-based caching feature.
- **BUG-1** (wrong method in test) is a regression risk for `RemoveAsync` empty-key guard.
- **BUG-2** (wrong exception type) is a silent contract violation; discoverable by consumers who `catch (ArgumentNullException)`.
- **MIS-1** (net6 claim) will cause install failures for net6 users and support questions.
- **MIS-3** (broken decorator example) will cause runtime failures for users who follow the README.

---

## Summary: Tickets to Create

| Priority | Category | Short title |
|---|---|---|
| High | bug | Fix wrong method call in `RemoveAsync_ShouldThrowArgumentException_WhenKeyIsEmpty` test |
| High | bug | Fix `Set(key, entity, options)` to throw `ArgumentNullException` for null key |
| High | bug | Audit and correct CHANGELOG — `[Cacheable]` / `FromModel` / `FromRouteOrQuery` entries |
| High | docs | Remove dead link to `docs/performance-and-observability.md` (or create the file) |
| High | docs | Fix decorator example in both READMEs — circular DI or replace with correct pattern |
| High | docs | Correct net6.0 claim — either restore multi-targeting or update docs |
| Medium | docs | Fix `GetAsync` XML remarks (says "return false", should say "returns null") |
| Medium | docs | Update `.csproj` Description and PackageTags |
| Medium | test | Add missing `RemoveAsync("")` test |
| Medium | feature | Add `GetOrSetAsync` to interface and implementation |
| Medium | feature | Add `TryRemoveAsync` / `TryRefreshAsync` |
| Low | feature | Add `RedisCacheManagerOptions` alias with `[Obsolete]` on misspelled name |
| Low | content | Write S-01: "Strongly-typed Redis caching in .NET 8 with System.Text.Json" |
| Low | content | Write S-02: "Testing Redis caching in .NET without a real Redis server" |
| Low | content | Write S-03: IRedisCacheManager vs IDistributedCache comparison |
