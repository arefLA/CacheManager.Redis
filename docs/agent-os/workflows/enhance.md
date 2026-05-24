# Workflow: Enhancement Proposal (W-04)

Modes: **Investigate** → **Build** (after approval)
Produces: Investigation Note, Implementation Plan, Verification Note

---

## When to use

- A new method or overload is proposed for `IRedisCacheManager<T>`
- A new option is proposed for `RedisCacheMangerOptions`
- A new registration helper is proposed for `Setup.cs`
- A behavioral improvement is requested that does not fix a specific bug

## Phase 1: Investigate the proposal

Before writing a single line of implementation:

1. Read the relevant interfaces and services in full.
2. Answer these questions in the Investigation Note:
   - **What exists today?** Describe the current behavior exactly.
   - **What is the gap?** What can a user not do today that they should be able to?
   - **Does it fit the library's scope?** CacheManager.Redis reduces boilerplate around typed Redis caching. Enhancements that add Redis data structures, pub/sub, or unrelated distributed systems concerns are out of scope.
   - **Is there a public API change?** New interface methods are breaking for anyone using a custom `IRedisCacheManager<T>` implementation. This must be flagged explicitly.
   - **What is the test surface?** What new test scenarios are needed?

3. Write the Investigation Note.
4. Stop. Present findings. Do not write the Implementation Plan until the direction is approved.

## Phase 2: Implementation Plan

Only after explicit approval of the direction:

1. Write the Implementation Plan with the full scope.
2. Pay particular attention to **Public API Changes**. If a method is added to `IRedisCacheManager<T>`, all of the following must be updated:
   - The interface in `Interfaces/IRedisCacheManager.cs`
   - The implementation in `Services/RedisCacheManager.cs`
   - The fake in `test/CacheManager.Redis.Tests/Fakers/FakeCustomCacheManager.cs`
   - The README (`README.md` and `src/CacheManager.Redis/README.md`) — update in the same Build session
3. List any overloads that should be added alongside the primary change (e.g., sync + async variants).

## Phase 3: Build

Follow W-06. If the enhancement adds a new public method, README updates (`README.md` and `src/CacheManager.Redis/README.md`) must be completed in the same Build session — not deferred to Content mode. Note any README changes in the Verification Note.

## Scope discipline

Enhancement proposals often expand during implementation. If during Build you discover the plan needs to grow:
1. Stop.
2. Describe the expansion.
3. Get approval.

Do not add "while I'm in here" changes.

## Breaking change rule

Any change to `IRedisCacheManager<T>` or `IRedisDistributedCache` signatures is a **breaking change** for consumers who implement the interface themselves — most commonly in test fakes like `FakeCustomCacheManager`. Under strict semver this warrants a major version bump.

This package uses a pragmatic interpretation: because most consumers only *inject* the interface rather than implement it, additive interface changes (new methods) are treated as minor bumps. Removals or signature changes to existing methods remain major bumps.

**The rule:**
- Removing or renaming an existing method on a public interface → **major** version bump
- Adding a new method to a public interface → **minor** version bump, document it clearly
- All interface changes → called out explicitly in the Implementation Plan and documented in the changelog under **Added** (new methods) or **Changed** (modified signatures)

Regardless of version bump level, always update `FakeCustomCacheManager` in the same Build session or the test project will not compile.
