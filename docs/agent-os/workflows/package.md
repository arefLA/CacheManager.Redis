# Workflow: Package (W-12)

Mode: **Release**
Produces: Validated .nupkg + Package Validation Note

---

## Prerequisites

- Release Checklist pre-release section is complete
- Version in `.csproj` is correct
- `dotnet test` passes

## Steps

1. Build in Release configuration to confirm there are no errors or warnings:
   ```bash
   dotnet build src/CacheManager.Redis/CacheManager.Redis.csproj -c Release
   ```
   Must have 0 errors and 0 warnings.

   Note: because `<GeneratePackageOnBuild>true</GeneratePackageOnBuild>` is set in the `.csproj`, this build also produces a `.nupkg` in `src/CacheManager.Redis/bin/Release/`. **Ignore that output entirely.** It goes to `bin/` which is `.gitignore`d and is not a valid release artifact. Use only the output from the explicit `dotnet pack` in the next step.

2. Pack to the `./artifacts` directory (also `.gitignore`d — safe to produce locally):
   ```bash
   dotnet pack src/CacheManager.Redis/CacheManager.Redis.csproj -c Release -o ./artifacts
   ```

3. Inspect the package:
   ```bash
   unzip -l ./artifacts/CacheManager.Redis.X.Y.Z.nupkg
   ```
   Verify:
   - `CacheManager.Redis.X.Y.Z.nuspec` — check version, description, tags
   - `README.md` is present
   - `lib/net6.0/` and `lib/net8.0/` are present
   - No unexpected files

4. Optionally install locally to test:
   ```bash
   dotnet nuget add source ./artifacts --name local-test
   dotnet add Sample/Sample.csproj package CacheManager.Redis --version X.Y.Z --source local-test
   dotnet build Sample/
   # Then revert the reference
   ```

5. Write Package Validation Note:
   ```markdown
   # Package Validation Note: vX.Y.Z
   Date: YYYY-MM-DD
   
   - Package ID: CacheManager.Redis confirmed
   - Version: X.Y.Z confirmed
   - README: present and correct
   - Target frameworks: net6.0, net8.0 confirmed
   - Dependencies: Ardalis.GuardClauses 5.x, Microsoft.Extensions.Caching.StackExchangeRedis 8.x confirmed
   - Artifact path: ./artifacts/CacheManager.Redis.X.Y.Z.nupkg
   - Build warnings: none
   - Ready to publish: YES
   ```

6. Stop. Do not push or publish without instruction.

## .csproj metadata check

Before packing, confirm these fields in the `.csproj` are accurate:
```xml
<Title>Redis Cache Manager</Title>
<Description>...</Description>
<PackageTags>redis, cache manager, ...</PackageTags>
<Version>X.Y.Z</Version>
<PackageReadmeFile>README.md</PackageReadmeFile>
```
