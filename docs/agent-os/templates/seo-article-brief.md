# SEO Article Brief: <title>

Date: YYYY-MM-DD
Status: pending approval | approved
Target package version: v<X.Y.Z>

---

## Target Keyword

Primary: `<keyword>` — e.g., "redis cache dotnet"
Secondary: `<keyword>`, `<keyword>` — e.g., "strongly typed redis cache", "IDistributedCache"

## Audience

Who is reading this. What they already know. What problem they are trying to solve right now.

Example: "A .NET backend developer integrating Redis into an ASP.NET Core API for the first time. They know C# and DI. They have not used StackExchange.Redis or IDistributedCache before."

## Article Goal

What the reader should be able to do after finishing the article.

Example: "Configure CacheManager.Redis in a new ASP.NET Core project and store/retrieve a typed object from Redis within 15 minutes."

## Factual Constraints

These are hard rules for this article. Do not deviate.

- [ ] All code samples must compile and run against CacheManager.Redis v<X.Y.Z>
- [ ] Do not claim performance numbers — this package has not published benchmarks
- [ ] Do not compare to other packages unless the comparison is demonstrably accurate
- [ ] If any feature is planned but not yet shipped, mark it explicitly: `(planned)`
- [ ] Do not use the words "enterprise", "battle-tested", or "production-grade" without evidence

## Outline

1. Introduction — hook, primary keyword placement
2. <section title>
3. <section title>
4. <section title>
5. Conclusion — recap + CTA (e.g., link to NuGet, GitHub)

## Code Samples Required

List each code scenario that needs a working example:

- [ ] Registration in Program.cs
- [ ] Basic get/set with `IRedisCacheManager<T>`
- [ ] <additional scenario>

## Word Count Target

~<N> words. Aim for depth over length. Do not pad.

## Output Location

`docs/<filename>.md`

---

**This brief must be approved before article writing begins. Do not start the article without approval.**
