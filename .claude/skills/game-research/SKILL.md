---
name: game-research
description: Researches the hyper-casual and hybrid casual mobile game market, analyzes top-performing titles, identifies trends and best practices, and generates initial game concepts with gameplay ideas. Use this skill whenever market research, competitor analysis, trend analysis, game concept ideation, or "what games are popular" is requested. Triggers include: "research the market", "what's trending in mobile games", "analyze top games", "find best practices", "generate game concepts", "what should our game be", "research hyper casual", "research hybrid casual", "game idea", "concept ideation". Do NOT trigger for game design execution (GDD writing) — that belongs to core-loop-design.
---

## Research Framework

Run in this order — breadth first, then depth, then synthesis:

```
Step 1: Market Landscape    → What is the overall shape of the market?
Step 2: Top Titles Deep Dive → What are the 2–3 best benchmarks to learn from?
Step 3: Trend Signals       → What is rising, what is declining?
Step 4: Best Practices      → What patterns appear in all successful titles?
Step 5: Concept Synthesis   → Translate findings into 3–5 game concepts
```

## Step 1: Market Landscape

Cover these dimensions for hyper-casual and hybrid casual separately:

| Dimension | What to capture |
|-----------|----------------|
| Market size & growth | Revenue trajectory, install volumes, key regions |
| Genre breakdown | Which sub-genres dominate (runner, merge, puzzle, idle, etc.) |
| Platform split | iOS vs Android share, key differences |
| Monetization mix | Ad-heavy vs IAP balance across sub-genres |
| Top publishers | Who ships the most volume and what they focus on |

**Key sources to check (use WebSearch if available):**
- App Store / Google Play top charts (Games → Casual / Hyper Casual)
- Sensor Tower, data.ai (AppAnnie), GameAnalytics public reports
- Deconstructor of Fun, GameRefinery blog posts

## Step 2: Top Titles Deep Dive

Pick 2–3 benchmark titles relevant to the target genre. For each title analyze:

```markdown
### [Title Name] — [Publisher] — [Genre]
- **Downloads / Revenue tier:** (e.g. 100M+ installs, top 10 grossing)
- **Core loop (30-sec cycle):** Action → Feedback → Reward
- **Meta layer:** What keeps players past day 3?
- **Monetization:** Primary ad type, IAP positioning, rewarded video placement
- **Visual style:** Art direction in one sentence
- **Retention signals:** Day1 / Day7 benchmarks if public
- **What it does exceptionally well:**
- **What it does poorly / player complaints (from reviews):**
- **Differentiable gap:** What would a competitor need to do to beat it?
```

## Step 3: Trend Signals

Label each signal clearly as **Rising**, **Stable**, or **Declining**:

| Signal | Status | Evidence |
|--------|--------|----------|
| Merge mechanics | Rising | [evidence] |
| Endless runner | Declining | [evidence] |
| Hybrid idle + builder | Rising | [evidence] |
| Match-3 | Stable | [evidence] |
| ... | | |

Also flag: emerging mechanics (less than 12 months old), cross-genre hybrids gaining traction, themes with cultural momentum (IP, seasonal, viral topics).

## Step 4: Best Practices

Synthesize recurring patterns from successful titles into rules:

**Core Loop Best Practices:**
- Session length sweet spot: 2–5 minutes per session
- Fail state must be clear and fast (under 3 seconds to restart)
- New mechanic introduction: one per 5 levels maximum
- Tutorial: must feel like play, not instruction

**Meta Layer Best Practices:**
- First upgrade must happen within first 3 minutes of play
- Idle/offline reward activates re-engagement loop
- Collection mechanic (characters, items) significantly boosts D7+ retention
- Streak systems add 15–30% Day3 retention lift on average

**Monetization Best Practices:**
- Rewarded video placement at natural "I want more" moments — not forced
- Interstitial cap: max 1 per 3 minutes or 3 levels
- Starter IAP pack ($0.99–$1.99) converts 3–5x better than $4.99 as first purchase
- Ad removal IAP performs best at $2.99–$3.99 price point

**Avoid:**
- Paywalls that block progress in first 20 levels
- Ads during active gameplay (immersion break = uninstall)
- Too many currencies (1 soft + 1 hard is the proven standard)

## Step 5: Concept Synthesis

Generate 3–5 game concepts. Each concept must follow this template:

```markdown
## Concept [N]: [Working Title]

**Genre:** [primary genre] + [meta layer type]
**One-line pitch:** [what you do + why it's satisfying — max 20 words]

**Core mechanic:** [describe the 30-second loop in plain language]
**Meta hook:** [what progression system keeps players returning day 3+]
**Target audience:** [who plays this — demographics + psychographics]
**Visual direction:** [one sentence style reference]

**Market fit:**
- Similar to: [2 existing titles] but differentiated by [specific angle]
- Gap it fills: [what the market is missing that this provides]

**Monetization angle:** [primary revenue driver + key IAP hook]

**Risk flags:**
- [Risk 1 — e.g. "crowded sub-genre, needs strong visual identity"]
- [Risk 2 — e.g. "mechanic requires tutorial investment"]

**Confidence:** High / Medium / Low
**Rationale:** [1–2 sentences on why this concept has legs]
```

**Concept quality gates — reject a concept if:**
- It is a near-clone of a dominant title with no differentiator
- The core loop takes more than 5 minutes to explain
- There is no clear monetization hook in the first session
- The target audience is "everyone"

## Output Files

Always write two files:

**`_workspace/market-research.md`**
Structure: Market Landscape → Top Titles → Trend Signals → Best Practices

**`_workspace/game-concepts.md`**
Structure: Executive Summary (pick the top concept and say why) → Concepts 1–N

For deeper genre-specific research patterns, see `references/genre-research-playbooks.md`.
