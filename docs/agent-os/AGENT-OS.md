# CacheManager.Redis Agent OS v1

---

## What This Is

Agent OS v1 is the operating specification for working on CacheManager.Redis with Claude as an agentic coding partner. It defines how Claude should behave across all types of work in this repository: engineering, testing, documentation, and release.

This is not an automation framework. It does not run autonomously. A human reviews and approves all significant outputs before they are committed, published, or released.

This is a discipline spec. Its purpose is to prevent sloppy changes, ensure consistency across sessions, make release work repeatable, and give Claude enough structure to operate without constant correction.

---

## Design Principles

**Separation before action.** Investigation is separate from implementation. Implementation is separate from release. Do not combine modes in one step unless explicitly instructed.

**Artifacts first.** Every significant action produces a written artifact before work begins. No code changes without an Implementation Plan. No release without a Release Checklist. This creates a review trail and forces thinking before typing.

**Small surface, real guardrails.** This repo is small. The OS is small. But lightweight does not mean permissive. The gating rules are real and must be followed even when they feel like overhead.

**Technical honesty.** All content—documentation, samples, SEO articles—must reflect actual package behavior. No invented benchmarks. No fake capability claims. No roadmap promises without explicit marking.

**Human gates real risk.** Claude does not push, publish, or release without explicit human confirmation. File edits and local commands are pre-approved. External actions (git push, NuGet publish, GitHub release) require explicit instruction each time. For any state-changing command (`git add`, `git commit`, `git tag`, `git push`, `dotnet pack`, `dotnet nuget push`, `gh release create`), Claude shows the exact command before running it.

---

## Operating Model

Agent OS v1 uses a **mode-based** model. Each session has one active mode. Modes enforce behavioral boundaries — what Claude is allowed to read, write, and run.

State the mode and task at the start of every session. Claude enters that mode and stays in it. Switching modes requires an explicit instruction.

**To start a session:**
```
Mode: [Orchestrate | Explore | Investigate | Build | Release | Content]
Task: <what you want done>
Context: <any relevant background or links>
```

If no mode is specified, Claude defaults to **Explore** and will not make changes.

## Branch Model

All development work happens on `dev`. PRs target `main`. When a PR is merged:

- `main` receives the changes
- Release mode runs **only from `main`** after the merge

Do not run Release mode from `dev`. Do not bump the version on `dev`.

---

## Modes

### Mode: Orchestrate

**Purpose:** Read the task backlog, select a task, determine the appropriate mode and workflow, and manage execution end-to-end — stopping at every gate for human verification.

**Allowed to:**
- Read any file in the repo
- Read and update `docs/agent-os/BACKLOG.md` (task status and artifact links only)
- Suggest which mode and workflow to apply to a selected task
- Transition into another mode after human confirmation

**Not allowed to:**
- Skip any gate rule belonging to the delegated mode
- Edit source files, README, or `.csproj` directly (those belong to the delegated mode)
- Start a task without human confirmation of the approach
- Mark a task complete without human confirmation

**Required input:** `docs/agent-os/BACKLOG.md` with at least one pending task, or a task provided inline by the human.

**Output:** Updated `BACKLOG.md` status + all artifacts produced by the delegated mode.

**Handoff:** After completing a task, return to the backlog and ask whether to continue with the next task or stop.

---

### Mode: Explore

**Purpose:** Understand the codebase. Answer structural and behavioral questions. Map dependencies. Identify where things live.

**Allowed to:**
- Read any file in the repo
- Run `dotnet build` (to confirm build state), `grep`, `find`, `git log`, `git diff`
- Produce a written summary or map

Explore mode runs read-only and non-state-changing local validation commands only.

**Not allowed to:**
- Edit any file
- Run `dotnet test` (use Investigate mode if test output is needed)
- Begin implementation or produce an Implementation Plan
- Switch modes unilaterally
- Run `dotnet pack`, push to git, or touch external systems

Explore may flag likely bugs, design issues, or improvement opportunities, but must stop short of proposing implementation details.

**Required input:** A question or area to explore.

**Output:** A concise written summary. If the user asks for a structured note, produce an Exploration Summary using the template at `docs/agent-os/templates/exploration-summary.md`.

**Handoff:** If exploration reveals a bug or opportunity, say so and stop. Do not automatically switch to Investigate. Wait for instruction.

---

### Mode: Investigate

**Purpose:** Investigate a specific bug, behavior, or proposed change. Produce an Investigation Note and optionally an Implementation Plan.

**Allowed to:**
- Everything Explore allows
- Run targeted tests, add temporary debug output to understand behavior
- Read test cases, inspect serialization behavior, trace call paths

**Not allowed to:**
- Commit any changes
- Make permanent edits to source files (the only file writes allowed are saving Investigation Notes and Implementation Plans to `docs/agent-os/runs/`)
- Leave temporary diagnostic edits in source files — any temporary debug output added during investigation must be removed before the session ends
- Open PRs or create issues

**Required input:** A specific bug report, behavior question, or enhancement idea.

**Output:**
1. An Investigation Note (see Templates) — always required
2. An Implementation Plan (see Templates) — required if the investigation concludes with actionable changes

**Handoff:** After producing the Implementation Plan, stop and wait for approval before switching to Build.

---

### Mode: Build

**Purpose:** Implement approved changes. This includes bug fixes, features, refactors, tests, and samples.

**Allowed to:**
- Edit source files in `src/`, `test/`, `Sample/`
- Update `README.md`, `src/CacheManager.Redis/README.md`, and `docs/` when required to keep public documentation accurate with implemented behavior changes (new public methods, changed options, changed setup, changed exceptions)
- Run `dotnet build` and `dotnet test`
- Create new files within established patterns

**Not allowed to:**
- Perform unrelated documentation rewrites, restructuring, or cleanup not required by the implemented behavior
- Bump version numbers (that belongs to Release)
- Push to git
- Merge branches
- Change `.csproj` metadata (description, tags, version) — defer to Release
- Run `dotnet build -c Release` — this triggers `GeneratePackageOnBuild` and produces an unvalidated `.nupkg` in `bin/`. Use `dotnet build` (Debug) during Build mode.

**Required input:** An approved Implementation Plan. Build mode must not start without one.

**What counts as a "non-trivial change" requiring an Investigation Note and Implementation Plan:**
- Any public API or interface change
- Any behavior change affecting cache semantics, serialization, key generation, expiration, or the `[Cacheable]` attribute
- Any change to DI registration behavior
- Any new file in `src/` or a new test suite in `test/`
- Any refactor touching multiple classes or layers

**What does NOT require a full plan (inline description in the session is sufficient):**
- Typo or comment fixes
- Whitespace or formatting corrections
- Localized one-line bug fixes with an obvious cause and no API impact (e.g., a wrong null check, a missing guard)
- Test-only additions that do not require behavior changes in `src/`

**Output:**
1. Working code that passes `dotnet test`
2. A Verification Note confirming what was tested and how (see Templates)

**Handoff:** After producing the Verification Note, stop. Do not automatically proceed to Release.

---

### Mode: Release

**Purpose:** Prepare and execute a release. This covers version bumping, changelog, pack, GitHub release, and NuGet publish.

**Allowed to:**
- Edit version in `.csproj`
- Edit `CHANGELOG.md`
- Edit `CLAUDE.md` (version number update)
- Run `dotnet pack`
- Validate the `.nupkg` artifact
- Produce the GitHub release body
- Run `dotnet nuget push` when explicitly confirmed

**Not allowed to:**
- Make feature changes or fix bugs (that's Build)
- Push to git or run `gh release create` without explicit human confirmation per action
- Publish to NuGet without explicit human confirmation per publish

**Required input:** A completed Verification Note from Build, a Release Checklist (see Templates), and explicit instruction to start.

**Output:**
1. Version bump commit
2. Updated `CHANGELOG.md`
3. Validated `.nupkg`
4. GitHub release body draft
5. Post-release verification note

**Handoff:** After each external action (push, gh release, nuget push), stop and report status. Do not chain external actions without confirmation.

---

### Mode: Content

**Purpose:** Write documentation, samples, SEO articles, and release notes.

**Allowed to:**
- Create and edit files in `docs/`, `Sample/`, `src/CacheManager.Redis/README.md`
- Edit the main `README.md`
- Produce markdown drafts for review

**Not allowed to:**
- Fabricate benchmarks, invent capabilities, or make roadmap claims without explicit marking
- Edit source code in `src/` or `test/`
- Publish content to external platforms

**Required input:** A task description specifying the content type, audience, and scope.

**Two content classes:**

1. **Product documentation** — `README.md`, `src/CacheManager.Redis/README.md`, `docs/`, `Sample/`. Must reflect actual package behavior exactly. No simplification that introduces inaccuracy.
2. **Marketing and SEO content** — articles, tutorials, comparison posts, release summaries. May simplify structure for readability but must remain technically accurate. Subject to the Marketing and SEO rules in this spec.

**Output:** A markdown draft. For SEO articles, also produce an SEO Article Brief (see Templates) before writing.

**Handoff:** All content must be reviewed by the human before being committed. Claude flags any claims it is unsure about.

---

## Workflow Catalog

### W-00: Orchestrate Workflow

1. Enter **Orchestrate** mode.
2. Read `docs/agent-os/BACKLOG.md`. Present pending tasks.
3. Human selects a task (or confirms the top priority item).
4. Determine starting mode and workflow from the task type → mode mapping in `workflows/orchestrate.md`.
5. Confirm the approach with the human before starting.
6. Update task status to `in-progress` in `BACKLOG.md`.
7. Execute in the delegated mode, following all gates and gating rules.
8. Mark task `complete` in `BACKLOG.md` when done. Return to backlog.

---

### W-01: Repo Exploration

1. Enter **Explore** mode.
2. Read the directory structure, `.csproj`, key interfaces, and `README.md`.
3. If asked for a map, produce an Exploration Summary.
4. Stop. Do not infer tasks.

---

### W-02: Investigation Before Implementation

1. Enter **Investigate** mode with a clear question or bug report.
2. Read relevant source, tests, and history.
3. Reproduce or confirm the behavior.
4. Write the Investigation Note.
5. If changes are warranted, write the Implementation Plan.
6. Stop. Wait for approval.

---

### W-03: Bug Investigation and Fix

1. **Investigate** mode: reproduce the bug, trace the root cause, write Investigation Note.
2. Get approval on the Investigation Note.
3. Write Implementation Plan (scope: minimal fix only, no unrelated cleanup).
4. Get approval on the Implementation Plan.
5. **Build** mode: implement the fix, run `dotnet test`, write Verification Note.
6. Human reviews diff and Verification Note.
7. Commit on human instruction.

---

### W-04: Enhancement Proposal

1. **Investigate** mode: research what exists, what is missing, and whether the enhancement fits the library's scope.
2. Write Investigation Note covering: current behavior, gap, proposed behavior, interface changes, breaking change risk.
3. Write Implementation Plan only if the human approves the direction.
4. Proceed to **Build** only after plan approval.

---

### W-05: Refactor Workflow

1. **Investigate** mode: identify what is being refactored and why. Write Investigation Note.
2. Implementation Plan must explicitly list:
   - Files touched
   - Public API changes (none expected in a safe refactor)
   - Test coverage before and after
3. **Build** mode: make changes, ensure tests still pass, write Verification Note.
4. Do not refactor beyond the stated scope.

---

### W-06: Implementation Workflow

1. Confirm Implementation Plan is approved.
2. **Build** mode: follow the plan in order. Do not scope-creep.
3. After each logical unit (not necessarily each file), run `dotnet test`.
4. Write Verification Note when complete.
5. Do not bump version, update changelog, or modify `.csproj` metadata during Build.

---

### W-07: Test-Writing Workflow

1. **Investigate** mode: identify what is untested or under-tested.
2. Write brief Implementation Plan: which behaviors, which test classes, which scenarios.
3. **Build** mode: write tests. Follow existing patterns in `test/CacheManager.Redis.Tests/`.
4. All new tests must pass. No skipped tests without explanation.
5. Verification Note: list each new test method and what it asserts.

---

### W-08: Sample-Writing Workflow

1. **Content** mode.
2. Samples live in `Sample/`. Follow the existing ASP.NET Core structure.
3. Every sample must compile and run against the current package.
4. Do not invent sample scenarios that require unreleased features.
5. Verify the sample builds: `dotnet build Sample/`.

---

### W-09: Documentation-Writing Workflow

1. **Content** mode.
2. For package-level docs (README embedded in NuGet), edit `src/CacheManager.Redis/README.md`.
3. For repo README, edit the root `README.md`.
4. For standalone guides, create files in `docs/`.
5. All API usage in docs must match current interfaces in `src/CacheManager.Redis/Interfaces/`.
6. Verify accuracy by reading the interface before writing.

---

### W-10: SEO Article / Technical Marketing Content Workflow

1. **Content** mode.
2. Start with an SEO Article Brief (see Templates). Get approval before writing.
3. Write the article. Every code sample must work against the actual package.
4. Apply SEO discipline rules (see Marketing and SEO section).
5. Output goes to `docs/`. Human reviews before publishing anywhere.

---

### W-11: Release Note / Changelist Workflow

1. Stay in **Release** mode. No need to switch to Content. Release mode can edit `CHANGELOG.md` and produce the GitHub release body draft.
2. Run `git log <previous-tag>..HEAD --oneline` to get the commit list.
3. Group commits using the changelog template headings: **Added**, **Changed**, **Fixed**, **Internal**.
4. Write the changelog entry in `CHANGELOG.md` using the template.
5. Write the GitHub release body (same content, formatted for GitHub).
6. Human reviews before it is attached to a release.

---

### W-12: Package Workflow

1. **Release** mode.
2. Confirm Release Checklist is complete.
3. Confirm version in `.csproj` matches the intended release version.
4. Run `dotnet pack src/CacheManager.Redis/CacheManager.Redis.csproj -c Release -o ./artifacts`.
5. Inspect the `.nupkg` contents: confirm version, README, and dependencies are correct.
6. Write Package Validation Note.
7. Stop and report. Do not publish without instruction.

---

### W-13: GitHub Release Workflow

1. **Release** mode.
2. Confirm the `.nupkg` has been validated.
3. Confirm the changelog entry is written.
4. Confirm the tag exists: `git tag -l v<version>` — must output the tag name.
5. Push the release commit and tag: `git push origin main` then `git push origin v<version>` — each requires explicit confirmation.
6. Create the GitHub release using `gh release create v<version>` with the release body — requires explicit confirmation.
7. Verify the release appears at `https://github.com/arefLA/CacheManager.Redis/releases`.

---

### W-14: NuGet Publish Workflow

1. **Release** mode.
2. Confirm the GitHub release is live.
3. Confirm the `.nupkg` path.
4. Run `dotnet nuget push` — requires explicit confirmation with the API key.
5. Verify the package appears on NuGet at `https://www.nuget.org/packages/CacheManager.Redis`.
6. Write post-publish verification note.

---

## Gating Rules

### Before Build starts:

- [ ] Investigation Note exists and has been reviewed
- [ ] Implementation Plan exists and has been explicitly approved
- [ ] The plan does not include scope beyond what was approved

### Before Release starts:

- [ ] PR from `dev` to `main` is merged
- [ ] Currently on `main` (`git branch --show-current`)
- [ ] Working tree is clean — no uncommitted changes outside intended release metadata files
- [ ] `./artifacts/` is empty or absent — no stale `.nupkg` from a prior run
- [ ] `dotnet test` passes with no failures
- [ ] Verification Note exists for the changes being released
- [ ] Changelog entry has been drafted
- [ ] Version number has been agreed (no automatic bumps)
- [ ] `CLAUDE.md` version updated to the new version number
- [ ] README accurately describes the release version's features

### Before NuGet publish:

- [ ] GitHub release is live with the correct tag
- [ ] Package Validation Note has been written and saved to `runs/`
- [ ] `.nupkg` has been locally validated (version, contents, dependencies)
- [ ] Package README matches the source README
- [ ] Human has confirmed the publish command

### Gate failure rule

If a gate condition is not met, Claude must:
1. Stop immediately.
2. State which specific condition is unmet.
3. Not suggest bypassing the gate unless the human explicitly requests a change to the spec.

---

## Artifact Definitions

### Investigation Note

**Purpose:** Record what was found during investigation. Inputs for the Implementation Plan.

**Required contents:**
- Task or bug ID/title
- What was investigated (files, methods, test cases)
- What was found (root cause or gap)
- Reproduction steps if a bug
- Risk surface (what else could be affected)

**When required:** Before any Implementation Plan is written.

**Who produces it:** Claude in Investigate mode.

---

### Implementation Plan

**Purpose:** Define exactly what will be changed before any code is written.

**Required contents:**
- Problem statement (one sentence)
- Proposed solution (concrete, not abstract)
- Files to be changed
- New files to be created (if any)
- Public API changes (or confirmation there are none)
- Test plan: what new tests are needed
- Out of scope (explicit list of what is NOT being done)

**When required:** Before Build mode starts for any non-trivial change.

**Who produces it:** Claude in Investigate mode, approved by the human.

---

### Verification Note

**Purpose:** Confirm that what was built matches what was planned and that tests pass.

**Required contents:**
- Reference to the Implementation Plan
- Summary of changes made
- Test results (`dotnet test` output summary)
- Any deviations from the plan and why
- Known gaps or follow-up items

**When required:** After every Build session, before Release can start.

**Who produces it:** Claude in Build mode.

---

### Release Checklist

**Purpose:** Gate the release. No release proceeds without this being complete.

**Required contents:** See template.

**When required:** At the start of every Release mode session.

**Who produces it:** Claude in Release mode, confirmed by human.

---

### Package Validation Note

**Purpose:** Confirm the `.nupkg` is correct before publish.

**Required contents:**
- Package ID confirmed: `CacheManager.Redis`
- Version number confirmed
- Target frameworks confirmed
- README included and correct
- Dependencies list confirmed
- No stale build artifacts

**When required:** After `dotnet pack`, before push or publish.

**Who produces it:** Claude in Release mode.

---

### Changelog Entry

**Purpose:** Human-readable record of changes in a version.

**Required contents:** See template.

**When required:** For every release.

**Who produces it:** Claude in Release mode.

---

### SEO Article Brief

**Purpose:** Align on topic, target keywords, and factual constraints before writing.

**Required contents:** See template.

**When required:** Before any SEO or marketing article is written.

**Who produces it:** Claude in Content mode, approved by human before article writing begins.

---

## Suggested Repository Structure

```
CLAUDE.md                        ← session bootstrap (read automatically by Claude Code)
CHANGELOG.md                     ← version history
docs/
├── agent-os/
│   ├── AGENT-OS.md              ← this file (main spec)
│   ├── README.md                ← quick-start reference
│   ├── BACKLOG.md               ← task backlog (read by Orchestrate mode)
│   ├── runs/                    ← session artifacts (committed)
│   │   └── YYYY-MM-DD-<slug>-<type>.md
│   ├── workflows/               ← detailed procedure for each workflow
│   │   ├── orchestrate.md       ← W-00
│   │   ├── explore.md           ← W-01
│   │   ├── investigate.md       ← W-02
│   │   ├── bug-fix.md           ← W-03
│   │   ├── enhance.md           ← W-04
│   │   ├── refactor.md          ← W-05
│   │   ├── implement.md         ← W-06
│   │   ├── test.md              ← W-07
│   │   ├── sample.md            ← W-08
│   │   ├── docs-writing.md      ← W-09
│   │   ├── seo-content.md       ← W-10
│   │   ├── release-notes.md     ← W-11
│   │   ├── package.md           ← W-12
│   │   ├── github-release.md    ← W-13
│   │   └── nuget-publish.md     ← W-14
│   └── templates/               ← artifact templates (fill in per session)
│       ├── task-entry.md
│       ├── exploration-summary.md
│       ├── investigation-note.md
│       ├── implementation-plan.md
│       ├── verification-note.md
│       ├── release-checklist.md
│       ├── changelog-entry.md
│       └── seo-article-brief.md
└── redis-cache-dotnet-guide.md  ← existing SEO article
```

### `runs/` naming convention

Every artifact saved to `runs/` must follow this pattern:

```
YYYY-MM-DD-<slug>-<type>.md
```

Types: `investigation`, `plan`, `verification`, `release-checklist`, `package-validation`, `post-release`, `task`, `explore`

Examples:
```
2026-04-12-null-key-handling-investigation.md
2026-04-12-add-remove-overload-plan.md
2026-04-12-add-remove-overload-verification.md
2026-02-13-v2.1.1-release-checklist.md
2026-02-13-v2.1.1-package-validation.md
2026-04-12-initial-codebase-explore.md
```

Artifacts in `runs/` are committed to the repo. They are the audit trail.

---

## Templates

Templates live in `docs/agent-os/templates/`. Each template is its own file — use those files directly. Do not edit the templates themselves; copy the contents into a new file in `runs/` for each session.

| Template file | When to use |
|---|---|
| [`task-entry.md`](templates/task-entry.md) | Every Investigate, Build, Release, and Content session; optional for Explore |
| [`exploration-summary.md`](templates/exploration-summary.md) | When an Explore session produces a structured output |
| [`investigation-note.md`](templates/investigation-note.md) | Every Investigate session |
| [`implementation-plan.md`](templates/implementation-plan.md) | Before every Build session |
| [`verification-note.md`](templates/verification-note.md) | After every Build session |
| [`release-checklist.md`](templates/release-checklist.md) | Every Release session |
| [`changelog-entry.md`](templates/changelog-entry.md) | Every release |
| [`seo-article-brief.md`](templates/seo-article-brief.md) | Before every SEO article |

---

## Release Workflow

This is the authoritative release procedure. Follow it in order. Do not skip steps.

**All steps in this workflow happen on `main` after the PR from `dev` is merged.** Do not start any step — including the version bump and changelog — while on `dev`. The PR should contain only the feature/fix work. Release prep (version bump, changelog, README verify, pack) is a separate commit on `main`.

### Step 1: Confirm readiness

Run the Release Checklist. All pre-release items must be checked before proceeding.

```bash
dotnet test
# Must output: X passed, 0 failed
```

### Step 2: Bump version

Edit `src/CacheManager.Redis/CacheManager.Redis.csproj`:
```xml
<Version>X.Y.Z</Version>
```

Follow semantic versioning:
- Patch (X.Y.**Z**): bug fixes only
- Minor (X.**Y**.0): new non-breaking functionality
- Major (**X**.0.0): breaking API changes

### Step 3: Update changelog

Add entry to `CHANGELOG.md` at the root. Use the changelog template.

### Step 4: Verify README accuracy

- Root `README.md`: confirm all code samples work with the new version
- `src/CacheManager.Redis/README.md`: confirm it matches what ships in the NuGet package
- Check that `<Version>` in `.csproj` matches what README says about current version

### Step 5: Verify sample

```bash
dotnet build Sample/Sample.csproj
```

If the sample references a local package path, confirm the reference is correct.

### Step 6: Pack

Note: `CacheManager.Redis.csproj` has `<GeneratePackageOnBuild>true</GeneratePackageOnBuild>`, which means `dotnet build -c Release` already produces a `.nupkg` in `bin/`. Use the explicit `dotnet pack` command below to produce a clean artifact in `./artifacts` instead, and always use this path for publishing — never the `bin/` output.

```bash
dotnet pack src/CacheManager.Redis/CacheManager.Redis.csproj -c Release -o ./artifacts
```

Inspect the output:
```bash
# Verify version, README, and target frameworks inside the .nupkg
unzip -l ./artifacts/CacheManager.Redis.X.Y.Z.nupkg
```

Write the Package Validation Note.

### Step 7: Commit and tag

This step runs on `main` after the PR from `dev` is merged. Confirm you are on `main`:

```bash
git branch --show-current  # must be: main
```

```bash
git add src/CacheManager.Redis/CacheManager.Redis.csproj src/CacheManager.Redis/README.md CHANGELOG.md README.md CLAUDE.md
git commit -m "release: v<version>"
git tag vX.Y.Z
```

**Stop. Human confirms before push.**

### Step 8: Push release commit and tag

Step 7 created a new commit on `main` (version bump, changelog, README, CLAUDE.md). Push the branch, then push the tag. Each requires explicit confirmation.

```bash
git push origin main
```

**Requires explicit confirmation.**

```bash
git push origin vX.Y.Z
```

**Requires explicit confirmation.**

### Step 9: GitHub release

```bash
gh release create vX.Y.Z ./artifacts/CacheManager.Redis.X.Y.Z.nupkg \
  --title "vX.Y.Z" \
  --notes-file <changelog-entry-file>
```

**Requires explicit confirmation.**

### Step 10: NuGet publish

```bash
dotnet nuget push ./artifacts/CacheManager.Redis.X.Y.Z.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

**Requires explicit confirmation. Never hardcode the API key.**

### Step 11: Post-release verification

- Confirm NuGet page shows the new version (may take a few minutes)
- Confirm package README renders correctly on NuGet
- Confirm GitHub releases page is correct
- Write post-release note in `docs/agent-os/runs/`

---

## Marketing and SEO Workflow

### Purpose

Produce technical content that helps .NET developers find and use CacheManager.Redis. The content must be accurate, useful, and grounded in what the package actually does.

### Rules

**Truth first.**
- Every code sample must compile and run against the stated package version.
- Do not claim performance numbers unless you have measured them in a reproducible way and can cite the conditions.
- Do not describe features that do not exist in the released package.
- If a feature is planned but not shipped, mark it explicitly: `(planned for v3.0)`.

**No inflation.**
- Do not describe the package as "enterprise-grade", "battle-tested", or similar unless you have evidence.
- Do not compare against other packages (StackExchange.Redis, EasyCaching, etc.) unless the comparison is accurate, fair, and you have tested both.
- Do not invent user quotes or testimonials.

**SEO discipline.**
- Target one primary keyword per article. Do not stuff secondary keywords unnaturally.
- The keyword must appear in the title, the first paragraph, at least one H2, and the meta description.
- Do not write articles whose only purpose is to rank — every article must genuinely help a developer solve a problem.

**Article brief required.**
- Every SEO article starts with an approved brief. No exceptions.
- The brief defines the factual constraints before writing begins.

**What good SEO content looks like for this package:**
- "How to use Redis caching in ASP.NET Core" — with real code using CacheManager.Redis
- "Strongly-typed Redis cache in .NET with System.Text.Json" — explains the serialization model
- "Reducing Redis boilerplate in .NET 8" — compares before/after, factual
- "IRedisCacheManager vs IDistributedCache" — honest comparison, both have trade-offs

**What to avoid:**
- "Why CacheManager.Redis is the best Redis library" (superlative, unprovable)
- "10x faster than raw Redis" (invented benchmark)
- "Used by thousands of developers" (unverifiable claim)

---

## v1 Boundaries

Agent OS v1 explicitly does not cover:

- **No autonomous operation.** Claude does not run, publish, or push anything without a human in the loop.
- **No issue triage.** Claude does not read or respond to GitHub issues autonomously.
- **No social media.** No Twitter/X, LinkedIn, or other posting.
- **No external monitoring.** No NuGet download stats, no GitHub traffic analysis.
- **No multi-repo work.** This OS applies only to the CacheManager.Redis repository.
- **No background jobs.** All sessions are synchronous and human-initiated.
- **No CI/CD integration.** No GitHub Actions workflows are defined or modified by this OS.
- **No automatic dependency updates.** Dependency bumps go through Build mode with an Investigation Note.
- **No hidden git operations.** Every git command is shown and confirmed before running.
- **No automatic branch management.** Claude does not create, merge, or delete branches without instruction.

These are v2 candidates, not omissions.

---

## Recommended First Iteration Plan

The following tasks were completed during Agent OS v1 setup. They are recorded here for reference.

1. ~~**Commit this spec.**~~ Done — `docs/agent-os/` is committed.

2. ~~**Write the initial `CHANGELOG.md`.**~~ Done — `CHANGELOG.md` is backfilled through v2.1.1.

3. **Run an initial Explore session.** Not yet done. Run Mode: Explore to produce a codebase map and save it to `docs/agent-os/runs/YYYY-MM-DD-initial-explore.md`. This gives future sessions a starting point without re-deriving structure.

4. **Dry-run the Release Checklist against v2.1.1.** Not to re-release, but to find any gaps in the checklist before the next real release. Mark any failures as follow-up tasks.

5. **Run one real task end-to-end.** Pick one thing — a missing test, a small bug, a doc gap — and run it through: Investigate → Build → Content (if docs need updating). This validates that the OS works in practice, not just on paper.

---

## Design Rationale

**Why 6 modes.**

The work in this repo falls into two concerns: writing code and writing words. Explore and Investigate are pre-work for writing code. Build is the execution. Release is deployment. Content covers everything text-based. Orchestrate sits above all of them — it is the task coordination layer that reads the backlog, selects work, and manages mode transitions without bypassing any gate. Six modes cover the full surface. Any fewer and the separation between investigation and implementation would collapse; removing Orchestrate would put all task selection and sequencing on the human with no structured support.

**Why modes in one conversation, not separate agents.**

Separate agents add context switching cost and cold-start overhead. In a small repo worked on by one person, the value of isolated agents is low. Modes within a single conversation preserve context while still enforcing behavioral boundaries. The mode constraint is behavioral, enforced by convention — not by a runtime.

**Why this is not overengineered.**

The spec is one main file, fourteen workflow docs, eight templates, and a `CLAUDE.md` bootstrap. It has no configuration format, no DSL, no agent registry, no orchestration graph. It works by convention and instruction. A senior developer can read the whole thing in twenty minutes and start using it the same day. That is the right level of complexity for a package at this size and stage.

**What can be added in v2.**

If the repo grows — multiple contributors, more complex release branching, CI integration, or a documentation site — v2 additions could include:

- GitHub Actions hooks that enforce artifact presence before merge
- An automated changelog generator (still human-reviewed)
- A dedicated `docs-site` workflow if a documentation site is added
- Issue triage rules if the issue volume grows
- A `CONTRIBUTING.md` that references this OS for contributors
