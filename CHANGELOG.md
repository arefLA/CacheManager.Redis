# Changelog

All notable changes to CacheManager.Redis are documented here.
Versions follow [Semantic Versioning](https://semver.org/).

---

## [2.1.2] — 2026-05-24

### Fixed
- README: dropped incorrect `net6.0` target claim; package targets `net8.0` only as before (MIS-1).
- README: removed dead link to `docs/performance-and-observability.md` (BUG-4).
- README: removed dead link to `Sample/Sample.http` (file was deleted earlier in v2.1.0 cleanup) (N-1).
- README: corrected the `LoggingCacheManager<T>` decorator example, which caused a circular DI exception at runtime. Rewritten to use [Scrutor](https://github.com/khellang/Scrutor)'s `Decorate` extension (MIS-3).
- `IRedisCacheManager<T>.GetAsync` XML documentation: corrected the "return false" remark (copy-pasted from `TryGet`) to "Returns null" (MIS-2).
- CHANGELOG: corrected v1.3.0 and v2.1.0 entries that referenced a `[Cacheable]` action filter and `FromModel` / `FromRouteOrQuery` key sources that were never released. The work exists on an unmerged feature branch and is not part of any shipped version (BUG-3).

### Internal
- `.gitignore`: ignore `.DS_Store` globally to keep `git status` clean on macOS.

---

## [2.1.1] — 2026-02-13

### Fixed
- Typo in README for `TryGetAsync` operation description.

---

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

---

## [2.0.0] — 2024-11-22

### Changed
- **Breaking:** Upgraded target framework to `net8.0`. `net6.0` support retained.
- Updated `Ardalis.GuardClauses` dependency to v5.
- Internal refactor and optimization pass on `RedisCacheManager` and `RedisDistributedCache`.

### Internal
- Tests updated to reflect .NET 8 and updated guard clause API.

---

## [1.4.0] — 2024-11-20

### Fixed
- Fixed session cache scope registration — `IRedisCacheManager<T>` was incorrectly registered as singleton in some configurations.
- Fixed `MethodNotFoundException` caused by Ardalis.GuardClauses API change in v5.

### Changed
- Updated to .NET 8 and `Ardalis.GuardClauses` v5.

---

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

---

## [1.2.0] — 2024-05-07

### Internal
- README improvements and clarifications.

---

## [1.1.0] — 2024-05-05

### Added
- `HasValue` extension method on cache results.
- `TryGet` returns `false` (instead of throwing) when the key is null or whitespace.
- `TryGetAsync` returns `null` (instead of throwing) when the key is null or whitespace.

### Fixed
- Fixed custom `IRedisCacheManager<T>` implementation initialization bug.

### Internal
- Added unit tests for null-key and edge-case behavior.
- Added `Ardalis.GuardClauses` for null argument handling.

---

## [1.0.0] — initial release

### Added
- `IRedisCacheManager<T>` — strongly-typed get/set/remove interface over `IDistributedCache`.
- `IRedisDistributedCache` — wrapper carrying serializer and options metadata.
- `AddRedisCacheManager` extension for DI registration in ASP.NET Core.
- System.Text.Json serialization with configurable `JsonSerializerOptions`.
- Configurable default `DistributedCacheEntryOptions`.
- Custom implementation support via `options.CustomImplementation`.
- Sample ASP.NET Core project.
