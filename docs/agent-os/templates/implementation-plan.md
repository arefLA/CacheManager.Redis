# Implementation Plan: <title>

Date: YYYY-MM-DD
Investigation Note: `docs/agent-os/runs/<file>.md`
Approved by: pending | <name>

## Problem

One sentence. What is broken or missing.

## Solution

Describe the approach concretely. Not "improve the caching layer" — describe what method changes, what interface additions, what behavior shifts.

## Files to Change

| File | Change |
|------|--------|
| `src/CacheManager.Redis/...` | Describe the change |
| `test/CacheManager.Redis.Tests/...` | Describe the new tests |

## New Files

None.

(Or list them with their purpose.)

## Public API Changes

None. (This plan does not change any public interface.)

(Or describe the exact signature change:)
```csharp
// Before
void Set(string key, TEntity entity);

// After
void Set(string key, TEntity entity, CancellationToken cancellationToken = default);
```

Breaking: Yes | No

## Test Plan

- [ ] Test scenario: describe what it verifies
- [ ] Test scenario: describe what it verifies
- [ ] Existing tests still pass with no changes

## Out of Scope

Explicit list of things this plan does NOT include. This prevents scope creep during Build.

- Not changing X
- Not refactoring Y
- Not updating README (unless explicitly listed in Files to Change)
