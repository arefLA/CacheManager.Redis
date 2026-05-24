# Workflow: Investigation (W-02)

Mode: **Investigate**
Produces: Investigation Note + (optionally) Implementation Plan

---

## When to use

- Before implementing any non-trivial change
- When a bug report arrives and the root cause is unknown
- When evaluating an enhancement idea
- When a refactor is proposed and the scope is unclear

## Steps

1. Enter Investigate mode. State the question or bug.

2. Read the relevant source files. Do not skim — read the actual implementation.

3. Run targeted tests if needed to confirm behavior:
   ```bash
   dotnet test --filter "FullyQualifiedName~<TestClass>"
   ```

4. Trace the call path: from the interface through the service into the underlying IDistributedCache.

5. Identify the root cause or gap. Write it down clearly before proceeding.

6. Write the Investigation Note. See `docs/agent-os/templates/investigation-note.md`.

7. If the recommendation is Fix or Enhance, write the Implementation Plan. See `docs/agent-os/templates/implementation-plan.md`.

8. Stop. Present both artifacts. Do not start Build until the human approves.

## Rules

- Do not make permanent edits to source files during investigation.
- Do not assume the bug is where the report says it is. Trace the actual code.
- Do not write the Implementation Plan until the Investigation Note is complete.
- Do not underestimate risk surface. Check what tests cover the area.
- Remove all temporary diagnostic edits before ending the session. The only durable writes allowed are Investigation Notes and Implementation Plans in `docs/agent-os/runs/`.

## Common investigation entry points

| Scenario | Start here |
|----------|-----------|
| Bug in get/set behavior | `src/CacheManager.Redis/Services/RedisCacheManager.cs` |
| Serialization issue | `src/CacheManager.Redis/Services/RedisCacheManager.cs` → JsonSerializer usage |
| DI registration issue | `src/CacheManager.Redis/Extensions/Setup.cs` |
| Options not applied | `src/CacheManager.Redis/RedisCacheMangerOptions.cs` |
| Test failing unexpectedly | `test/CacheManager.Redis.Tests/` → relevant test class |
