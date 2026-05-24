# Workflow: Repo Exploration (W-01)

Mode: **Explore**
Produces: Exploration Summary (optional)

---

## When to use

- Starting a new session with no prior context
- Answering structural questions about the codebase
- Understanding where a feature lives before investigating it
- Giving a new collaborator a map of the repo

## Steps

1. Enter Explore mode. Only read-only and non-state-changing local validation commands are permitted in this mode.

2. Read in this order:
   - Repo root: `ls`, `README.md`, `CacheManager.Redis.sln`
   - Source: `src/CacheManager.Redis/` — interfaces, services, extensions, options
   - Tests: `test/CacheManager.Redis.Tests/` — test classes, fakes
   - Sample: `Sample/` — entry point, controllers
   - Docs: `docs/`

3. Run `dotnet build` if you need to confirm the current build state.

4. Answer the question asked. Flag any bugs, design issues, or improvement opportunities noticed — but stop short of proposing implementation details or producing an Implementation Plan.

5. If the exploration reveals a bug or obvious gap, note it explicitly:
   > "During exploration I noticed X. This may be worth investigating. Do you want me to switch to Investigate mode?"
   Then stop.

## Output

If a written summary is requested, use the template at `docs/agent-os/templates/exploration-summary.md`.

Save to `docs/agent-os/runs/` using the standard naming convention:

```
YYYY-MM-DD-<slug>-explore.md
```

Example: `2026-04-12-initial-codebase-explore.md`
