# Workflow: NuGet Publish (W-14)

Mode: **Release**
Requires: GitHub release live, validated .nupkg

---

## Prerequisites

- GitHub release `vX.Y.Z` is live and confirmed
- `.nupkg` path is known and validated
- NuGet API key is available (in environment or will be provided)

## Steps

1. Confirm the .nupkg is the right one:
   ```bash
   ls -la ./artifacts/CacheManager.Redis.X.Y.Z.nupkg
   ```

2. **STOP. Confirm with human before running push.**

3. Push to NuGet:
   ```bash
   dotnet nuget push ./artifacts/CacheManager.Redis.X.Y.Z.nupkg \
     --api-key $NUGET_API_KEY \
     --source https://api.nuget.org/v3/index.json \
     --skip-duplicate
   ```

   `--skip-duplicate` prevents error if the version was already partially uploaded.
   Never hardcode the API key in a command that will appear in session history.

4. Wait for NuGet indexing. This typically takes 5–15 minutes.

5. Verify:
   - Package page: `https://www.nuget.org/packages/CacheManager.Redis/X.Y.Z`
   - README renders correctly
   - Dependencies are listed correctly
   - Target frameworks are listed

6. Write post-release note in `docs/agent-os/runs/`:
   ```markdown
   # Post-Release Note: vX.Y.Z
   Date: YYYY-MM-DD
   
   - GitHub release: https://github.com/arefLA/CacheManager.Redis/releases/tag/vX.Y.Z — confirmed live
   - NuGet: https://www.nuget.org/packages/CacheManager.Redis/X.Y.Z — confirmed live
   - README renders correctly on NuGet: YES
   - Known issues: none
   ```

## API key security

- Never paste the API key directly into a command shown to the user if the session is being recorded or shared.
- Prefer `$NUGET_API_KEY` environment variable.
- If the human will type the key interactively, structure the command so the key is the last argument or piped from stdin.
