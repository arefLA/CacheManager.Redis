# Agent OS v1 — CacheManager.Redis

This directory contains the operating specification for working on CacheManager.Redis with Claude as a disciplined coding partner. It defines how work is explored, implemented, and released.

## Start here

Read [`AGENT-OS.md`](AGENT-OS.md). It is the complete spec. Everything else is a supporting file.

## How to start a session

State the mode and task at the top of your prompt:

```
Mode: Investigate
Task: GetAsync returns null even when a key exists in Redis
Context: Reported by user, happens only with nullable reference types
```

Available modes: `Orchestrate` | `Explore` | `Investigate` | `Build` | `Release` | `Content`

If you do not specify a mode, Claude defaults to **Explore** and will not make changes.

## Directory structure

```
docs/agent-os/
├── AGENT-OS.md          ← full spec: modes, workflows, gating rules, release discipline
├── README.md            ← this file
├── workflows/           ← detailed procedure for each workflow
├── templates/           ← artifact templates (fill these in during sessions)
└── runs/                ← session artifacts: investigation notes, plans, verification notes
```

## Quick reference: which mode for what

| I want to... | Mode |
|---|---|
| Work through my task backlog | Orchestrate |
| Understand the codebase | Explore |
| Investigate a bug or idea | Investigate |
| Write or fix code | Build |
| Bump version, pack, publish | Release |
| Write docs, samples, SEO articles | Content |

## Quick reference: required artifacts

| Before... | You need... |
|---|---|
| Starting Build | Approved Implementation Plan |
| Starting Release | Verification Note + complete Release Checklist + on `main` |
| Publishing to NuGet | GitHub release live + Package Validation Note |
| Writing an SEO article | Approved SEO Article Brief |

## Important safety rule

Claude must not run external actions such as `git push`, `gh release create`, or `dotnet nuget push` without explicit confirmation for each action.

## Templates

| Template | When |
|---|---|
| `exploration-summary.md` | Structured Explore output |
| `task-entry.md` | Every Investigate, Build, Release, and Content session; optional for Explore |
| `investigation-note.md` | Every Investigate session |
| `implementation-plan.md` | Before every Build session |
| `verification-note.md` | After every Build session |
| `release-checklist.md` | Every Release session |
| `changelog-entry.md` | Every release |
| `seo-article-brief.md` | Before every SEO article |
If unsure which mode to use, start in **Investigate**.
