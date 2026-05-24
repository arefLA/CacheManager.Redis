# Workflow: Refactor (W-05)

Modes: **Investigate** → **Build**
Produces: Investigation Note, Implementation Plan, Verification Note

---

## When to use

- Internal implementation is confusing, duplicated, or inconsistent
- A method needs to be decomposed for testability
- Naming is misleading (internal names only — public API renames are breaking changes)
- Dead code needs to be removed

## The refactor constraint

A safe refactor changes internals without changing observable behavior. If the proposed change alters any of the following, it is not a refactor — it is an enhancement or a fix, and must use the appropriate workflow:

- Any public interface signature
- Any serialization behavior
- Any DI registration behavior
- Any exception type thrown on invalid input

## Phase 1: Investigate

1. Identify the specific problem with the current implementation. Write it down. "It's messy" is not a valid reason — name the actual issue: duplication, unclear responsibility, untestable path, etc.

2. In the Investigation Note, document:
   - Which files and methods are in scope
   - What the observable behavior is today (becomes the pass/fail test for the refactor)
   - What tests currently cover the area
   - Whether any tests would need to be updated (test behavior should not change — only test setup if internal structure changes)

3. Explicitly confirm: **no public API changes.** If public API must change, use W-04 (Enhance).

## Phase 2: Implementation Plan

The plan must include:

- **Files touched** — list every file
- **Public API changes** — must say "None"
- **Before/after test count** — refactors should not reduce test coverage
- **Out of scope** — be explicit about what is NOT being changed in this pass

Large refactors should be broken into smaller plans. Each plan should be independently mergeable and independently verifiable.

## Phase 3: Build

1. Make the changes.
2. Run `dotnet test` after every meaningful step, not just at the end.
3. If any test fails, stop immediately. Do not continue until it passes. A refactor that breaks tests is a bug introduction, not a refactor.
4. Write Verification Note.

## The before/after rule

Every refactor Verification Note must include:

```
Before: dotnet test — X passed
After:  dotnet test — X passed (same count, same tests)
```

If the count changes, explain why in the Verification Note.
