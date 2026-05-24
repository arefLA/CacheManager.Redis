# Workflow: SEO Article / Technical Marketing Content (W-10)

Mode: **Content**
Produces: SEO Article Brief (approved) + markdown article in `docs/`

---

## Rules before writing a single word

1. Complete the SEO Article Brief (`docs/agent-os/templates/seo-article-brief.md`).
2. Get explicit approval on the brief.
3. Only then begin writing.

This prevents wasted work when the angle, keyword, or factual constraints are wrong.

## Steps

1. Enter Content mode. Identify:
   - Target keyword
   - Reader persona
   - What problem the article solves

2. Fill out the SEO Article Brief. Pay particular attention to **Factual Constraints**.

3. Present the brief. Wait for approval.

4. Write the article following the approved outline.
   - Place the primary keyword in: H1 title, first paragraph, at least one H2, meta description.
   - Every code sample must use actual CacheManager.Redis API (read the interfaces before writing samples).
   - Explain what the code does. Do not just paste blocks.

5. Flag any uncertainty:
   > "I am not certain whether X is the current behavior — please verify before publishing."

6. Save draft to `docs/<slug>.md`.

7. Human reviews. Do not publish to any external platform.

## What makes a good article for this package

| Good | Bad |
|------|-----|
| Explains a real problem the package solves | Generic Redis overview with the package mentioned once |
| Code samples that compile and run | Pseudocode or incomplete samples |
| Honest trade-offs (e.g., when NOT to use this package) | Pure marketing copy |
| Specific to .NET 6/8 and current package version | Vague references to ".NET" without version context |
| One clear topic per article | Everything about Redis in one piece |

## Tone

- Direct. Senior developers do not need encouragement.
- Specific. "The `SetAsync` method serializes using `System.Text.Json`" beats "the package handles serialization for you."
- No adjectives that assert quality without evidence: not "seamless", not "powerful", not "robust".
