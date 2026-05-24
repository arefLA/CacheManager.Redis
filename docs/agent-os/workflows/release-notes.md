# Workflow: Release Notes / Changelist (W-11)

Mode: **Release**
Produces: Changelog entry + GitHub release body

---

## Steps

1. Get the commit range:
   ```bash
   git log <previous-tag>..HEAD --oneline
   # If no previous tag:
   git log --oneline
   ```

2. Categorize commits:
   - **Added**: new public API, new behavior, new options
   - **Changed**: modified behavior, updated dependencies
   - **Fixed**: bug fixes
   - **Internal**: refactors, test additions, tooling, CI — not user-facing

3. Write the changelog entry in `CHANGELOG.md`. See template in `docs/agent-os/templates/changelog-entry.md`.

4. Write the GitHub release body. Same content, but add:
   - A one-sentence summary at the top
   - Installation line: `dotnet add package CacheManager.Redis --version X.Y.Z`
   - Link to full changelog

5. Human reviews both before they are used.

## Rules

- Write what changed, not why it was committed.
- Do not include internal implementation details that package consumers don't care about.
- Fixes that correct behavior introduced in the same release cycle can be omitted (they never shipped to users).
- Keep entries short. One line per change is usually sufficient.

## GitHub release body structure

    ## CacheManager.Redis vX.Y.Z

    <one-sentence summary of this release>

    ```bash
    dotnet add package CacheManager.Redis --version X.Y.Z
    ```

    ### What's changed

    **Added**
    - ...

    **Fixed**
    - ...

    **Changed**
    - ...

    ---

    [Full changelog](https://github.com/arefLA/CacheManager.Redis/blob/main/CHANGELOG.md)
