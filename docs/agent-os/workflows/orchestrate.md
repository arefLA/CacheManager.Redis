# Workflow: Orchestrate (W-00)

Mode: **Orchestrate**
Produces: Updated `BACKLOG.md` status + all artifacts from the delegated mode

---

## Purpose

Orchestrate is the entry point when you have a backlog of work and want Claude to select, plan, and execute tasks end-to-end — stopping at every gate for human verification.

It does not replace the other modes. It selects the right mode for each task, confirms the approach with the human, manages transitions, and updates the backlog as work completes.

---

## Steps

1. Read `docs/agent-os/BACKLOG.md`. List all pending tasks by priority.

2. Present the list to the human. Ask which task to work on, or confirm starting the highest priority pending item.

3. Analyze the selected task:
   - What type is it? (see Task Type → Mode mapping below)
   - What mode and workflow should start?
   - Are there prerequisites? (e.g., a `release` task requires a Verification Note from a completed Build)

4. Confirm the plan with the human before starting:
   > "Task T-001 is a `bug`. I'll enter Investigate mode, trace the root cause, and produce an Investigation Note. Confirm to proceed."

5. Update `BACKLOG.md`: mark the task `in-progress`, record the start date.

6. Enter the appropriate mode. Follow the corresponding workflow. All gate rules from that mode apply without exception.

7. At every gate, stop and wait for human input:
   - After Investigation Note → wait for approval before writing Implementation Plan
   - After Implementation Plan → wait for approval before entering Build
   - After Verification Note → wait for instruction before entering Release
   - Before any external action (push, release, publish) → wait for explicit per-action confirmation

8. When the task is complete, update `BACKLOG.md`: mark as `complete`, record the released version (if applicable), and link the primary session artifact from `docs/agent-os/runs/`.

9. Return to the backlog. Ask whether to continue with the next task or stop.

---

## Task type → mode mapping

| Task type | Starting mode | Workflow |
|---|---|---|
| `bug` | Investigate | W-03: Bug Fix |
| `feature` | Investigate | W-04: Enhancement |
| `investigate` | Investigate | W-02: Investigation |
| `refactor` | Investigate | W-05: Refactor |
| `test` | Investigate (brief) | W-07: Test Writing |
| `docs` | Content | W-09: Documentation Writing |
| `sample` | Content | W-08: Sample Writing |
| `content` | Content | W-10: SEO Article |
| `release` | Release | W-12 → W-13 → W-14 |

---

## Rules

- Do not start a task without human confirmation of the approach.
- Do not skip any gate rule from the delegated mode — Orchestrate does not have elevated permissions.
- Do not mark a task `complete` without human confirmation of the outcome.
- If a task is blocked (missing prerequisite, ambiguous requirements, failed gate), mark it `blocked` in `BACKLOG.md` and report why.
- Only one task should be `in-progress` at a time unless the human explicitly starts parallel work.
- If a task's type is unclear, default to `investigate` — investigation is always safe.
