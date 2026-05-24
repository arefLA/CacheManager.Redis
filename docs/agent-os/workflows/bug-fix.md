# Workflow: Bug Fix (W-03)

Modes: **Investigate** → **Build**
Produces: Investigation Note, Implementation Plan, Verification Note

---

## Steps

### Phase 1: Investigate

1. Enter Investigate mode.
2. Read the bug report. Identify the claimed behavior and expected behavior.
3. Find the code path. Do not trust the report's diagnosis — verify it.
4. Write a reproduction: confirm the bug exists in a test or via code trace.
5. Identify the root cause. State it in one sentence.
6. Assess risk: what else touches this code? What tests exist?
7. Write Investigation Note.
8. Write Implementation Plan. Bug fixes must be minimal:
   - Fix the root cause
   - Add a test that would have caught the bug
   - Do not clean up surrounding code unless it is directly related

### Phase 2: Approval

Present the Investigation Note and Implementation Plan to the human.
Do not proceed to Build without explicit approval.

### Phase 3: Build

1. Enter Build mode.
2. Follow the Implementation Plan. Do not deviate.
3. Fix only what the plan describes.
4. Run `dotnet test` after the fix. All tests must pass.
5. Confirm the new test catches the bug (it should fail without the fix).
6. Write Verification Note.

### Phase 4: Review

Present the diff and Verification Note to the human.
Commit only on explicit instruction.

## Minimal fix rule

A bug fix is not an opportunity to refactor. If surrounding code is messy, note it in the Verification Note as a follow-up item. Do not change it in the same commit.
