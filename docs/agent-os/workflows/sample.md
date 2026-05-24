# Workflow: Sample Writing (W-08)

Mode: **Content**
Produces: Working sample code in `Sample/`

---

## What the sample is for

The `Sample/` project is an ASP.NET Core web application that demonstrates real usage of `CacheManager.Redis`. It is the first thing a new user looks at after reading the README. It must compile, run, and demonstrate correct usage — not a toy "hello world."

## Current structure

```
Sample/
├── Program.cs           — DI registration with AddRedisCacheManager
├── Book.cs              — example model
├── Controllers/         — controller using IRedisCacheManager<Book>
├── appsettings.json     — Redis connection string config
└── Sample.csproj        — project file
```

Follow this structure. Do not add significant complexity without good reason.

## Steps

1. Enter Content mode. Define the sample scenario clearly:
   - What use case is being demonstrated?
   - Is this a new scenario or an update to an existing one?
   - What package version does it target?

2. Read the current `Sample/` files before writing anything.

3. Read the current interface at `src/CacheManager.Redis/Interfaces/IRedisCacheManager.cs` to confirm available methods.

4. Write or update the sample.

5. Verify it compiles:
   ```bash
   dotnet build Sample/Sample.csproj
   ```
   Must have 0 errors.

6. If the sample references the package via NuGet (not local project reference), confirm the version in `Sample.csproj` matches the current released version.

## Rules

- Every code path shown in the sample must use the actual package API.
- Do not demonstrate features that do not exist in the released version. If the sample is being written ahead of a release, note this explicitly.
- Keep the sample simple. Its job is to show the pattern, not to be a full application.
- The sample controller should demonstrate: `TryGet`, `SetAsync`, and at least one async `GetAsync` call.
- Do not add sample code that requires infrastructure beyond a Redis connection (no databases, no external APIs).

## When the sample needs updating

The sample must be updated when:
- A new API method is added that users should see used
- Registration options change
- The README shows a different usage pattern than the sample demonstrates

Flag sample updates in the Verification Note when completing a Build session that changes the public API.
