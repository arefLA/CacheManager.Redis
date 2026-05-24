# Workflow: GitHub Release (W-13)

Mode: **Release**
Requires: Validated .nupkg, merged PR on main, written release body

---

## Prerequisites

- Package Validation Note is complete
- Release commit exists on `main` locally (version bump, changelog, README, CLAUDE.md — created in Step 7 of the Release Workflow)
- Tag `vX.Y.Z` exists locally (created in Step 7 of the Release Workflow)
- Release body (changelog) is written and reviewed

## Steps

1. Confirm you are on `main` at the correct commit:
   ```bash
   git log --oneline -3
   ```

2. Confirm the tag exists:
   ```bash
   git tag -l vX.Y.Z
   ```
   Must output `vX.Y.Z`. If the tag is missing, create it now: `git tag vX.Y.Z`.

3. **STOP. Confirm with human before pushing.**

4. Push the release commit to `main`:
   ```bash
   git push origin main
   ```

5. Push the tag:
   ```bash
   git push origin vX.Y.Z
   ```

6. **STOP. Confirm with human before creating the release.**

7. Create the GitHub release. Save the release body to a temp file first:
   ```bash
   # Write release body to a temp file
   cat > /tmp/release-body.md << 'EOF'
   <paste release body here>
   EOF
   
   gh release create vX.Y.Z \
     ./artifacts/CacheManager.Redis.X.Y.Z.nupkg \
     --title "vX.Y.Z" \
     --notes-file /tmp/release-body.md
   ```

8. Verify the release:
   ```bash
   gh release view vX.Y.Z
   ```
   Confirm: tag, title, body, attached .nupkg.

9. Report back. Do not proceed to NuGet publish without instruction.

## Error recovery

If the tag was pushed but the release failed:
- Do not create a new tag. Fix the release creation.
- Use `gh release edit vX.Y.Z` if the release exists but is wrong.
- If the tag itself is wrong, ask the human before deleting and recreating it.
