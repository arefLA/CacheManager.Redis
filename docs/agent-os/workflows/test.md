# Workflow: Test Writing (W-07)

Mode: **Build** (after brief Investigation)
Produces: New test cases + Verification Note

---

## Existing test structure

```
test/CacheManager.Redis.Tests/
├── Attributes/        — custom test attributes
├── Extensions/        — extension method tests
├── Fakers/            — fake/mock implementations (NSubstitute)
├── Models/            — test model types
└── Services/          — core service tests
```

Tests use xUnit and NSubstitute. Follow existing patterns.

## Steps

1. **Investigate first** (brief): identify what is not tested.
   - Run `dotnet test --collect "Code Coverage"` if coverage tooling is configured.
   - Or read the test classes and identify missing scenarios.

2. Write Implementation Plan:
   - List each test method to be added
   - State what behavior each test asserts
   - Note any new fakes or helpers needed

3. Get approval.

4. Enter Build mode. Write the tests. Follow existing naming conventions:
   - `<Method>_<Scenario>_<ExpectedOutcome>`

5. Run `dotnet test`. All tests pass.

6. Write Verification Note listing each new test and what it asserts.

## Rules

- No skipped tests without an explanation comment.
- Do not test implementation details — test the public interface.
- Use `FakeCustomCacheManager` or NSubstitute patterns already in the project.
- Each test must be independently runnable (no shared mutable state).
- Do not add tests that duplicate existing coverage unless the existing test is wrong.
