# Workflow: Documentation Writing (W-09)

Mode: **Content**
Produces: Updated markdown in `docs/`, `README.md`, or `src/CacheManager.Redis/README.md`

---

## Three documentation surfaces

This repo has three distinct doc surfaces. Know which one you are editing before you start.

| Surface | File | Purpose | Audience |
|---|---|---|---|
| NuGet README | `src/CacheManager.Redis/README.md` | Shown on the NuGet package page | Anyone installing the package |
| Repo README | `README.md` (root) | GitHub repo front page | Contributors, evaluators |
| Standalone guides | `docs/*.md` | Deep-dive topics | Developers using the package |

These files are related but not identical. The NuGet README focuses on usage. The repo README can include contributor information, roadmap, and development setup.

## Steps

1. Enter Content mode. Identify which surface you are updating and why.

2. Read the current version of the file before editing. Never write docs from memory.

3. Read the interface files before writing API usage:
   - `src/CacheManager.Redis/Interfaces/IRedisCacheManager.cs`
   - `src/CacheManager.Redis/Interfaces/IRedisDistributedCache.cs`
   - `src/CacheManager.Redis/Extensions/Setup.cs`
   
   Every method name, parameter name, and return type in the docs must match the actual interface.

4. Write or update the content.

5. Check all code samples compile mentally. If uncertain, confirm against the actual source.

6. Present the draft for review. Do not commit docs without review.

## Rules

- Do not describe methods that do not exist. Read the interface first.
- Do not describe options that are not in `RedisCacheMangerOptions`.
- Do not show a method signature in docs that differs from the actual signature.
- If a doc references a version number (e.g., "added in v2.1.0"), verify it in `CHANGELOG.md`.
- Do not add a "Roadmap" section unless the human explicitly provides roadmap content.

## Keeping NuGet and repo README in sync

The NuGet README and the root README will have overlapping content (installation, registration, core API usage). When one is updated, check whether the other needs the same update. They do not need to be identical, but they must not contradict each other.

## When docs must be updated (not optional)

- Any new public method or option → update NuGet README and root README
- Any registration change → update both READMEs and the Sample
- Any breaking change → add a migration note to the changelog and to the README

Flag doc update requirements in the Verification Note at the end of every Build session that changes public API.
