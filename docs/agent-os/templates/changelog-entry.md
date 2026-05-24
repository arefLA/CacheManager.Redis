# Changelog Entry Template

Use this template for each version entry in `CHANGELOG.md` at the repo root.

---

## [X.Y.Z] — YYYY-MM-DD

### Added

- Brief description of new feature or capability. Reference the relevant interface or method if useful.

### Changed

- Brief description of changed behavior. Call out any migration steps if needed.

### Fixed

- Brief description of bug fixed. One line per fix. Optional: reference issue number (`#42`).

### Internal

- Refactors, test additions, tooling updates — changes that do not affect the public API or package consumers.

---

## Rules for writing changelog entries

- Write in the past tense: "Added", "Fixed", "Changed" — not "Add", "Fix", "Change".
- Entries go at the **top** of `CHANGELOG.md`, above older versions.
- Do not include internal commit hashes or branch names.
- Do not include entries for changes that were reverted before release.
- If a section has no entries, omit it entirely. Do not write "### Fixed\n- None."
- "Internal" entries are optional. Include them if they help contributors understand what changed.

## Full CHANGELOG.md structure

```markdown
# Changelog

All notable changes to CacheManager.Redis are documented here.

## [X.Y.Z] — YYYY-MM-DD

### Added
- ...

## [X.Y.W] — YYYY-MM-DD

### Fixed
- ...
```
