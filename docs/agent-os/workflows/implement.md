# Workflow: Implementation (W-06)

Mode: **Build**
Requires: Approved Implementation Plan
Produces: Working code + Verification Note

---

## Entry condition

Do not enter Build mode without an approved Implementation Plan. If one does not exist, switch to Investigate and create one.

## Steps

1. Re-read the Implementation Plan before writing any code.

2. Implement in the order specified in the plan. If no order is specified, work from the interface inward:
   - Interface (if changed)
   - Service implementation
   - Extension/registration (if changed)
   - Tests

3. After each logical unit, run:
   ```bash
   dotnet build src/CacheManager.Redis/CacheManager.Redis.csproj
   ```

4. After all changes, run:
   ```bash
   dotnet test
   ```
   All tests must pass.

5. Write Verification Note.

## Scope discipline

- Do not add features beyond the plan.
- Do not rename variables, fix formatting, or clean up code outside the plan's scope.
- Do not bump the version number — that belongs to Release mode.
- Do not update `CHANGELOG.md` — that belongs to Release mode.
- Update `README.md`, `src/CacheManager.Redis/README.md`, and `docs/` only when required to keep public documentation accurate with the implemented behavior. Unrelated doc rewrites or restructuring are out of scope.
- If you encounter something that should be fixed but is out of scope, note it in the Verification Note under Follow-Up Items.

## When implementation diverges from the plan

If you discover during Build that the plan is wrong or needs adjustment:
1. Stop.
2. Describe the discrepancy.
3. Get approval before proceeding.

Do not silently deviate.
