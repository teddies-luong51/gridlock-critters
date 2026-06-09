---
name: game-director
description: Main orchestrator for hybrid casual game projects. Manages the full iteration loop: research → concept → MVP → evaluation → (ship full version OR pivot back to research). Triggers for: "make a game", "start a game project", "build a hybrid casual game", "research and make a game", "generate game concepts", "new game idea", "build the MVP", "design the levels", "evaluate the MVP", "update the game", "redo the game", "pivot to new concept", "go back to research", "ship the full version". For simple questions, answer directly without triggering.
---

## The Iteration Loop

```
┌─────────────────────────────────────────────────────┐
│                                                     │
│  [Phase 1] Brief                                    │
│       ↓                                             │
│  [Phase 2] Market Research  ←──────────────────┐   │
│       ↓                                         │   │
│  [Phase 3] Concept Selection (user picks)       │   │
│       ↓                                         │   │
│  [Phase 4] MVP Build                            │   │
│    - Gameplay design (5 MVP principles)         │   │
│    - 20–30 levels with difficulty curve         │   │
│    - Basic meta only (no monetization)          │   │
│    - Unity implementation + QA                  │   │
│       ↓                                         │   │
│  [Phase 5] MVP Evaluation (user decides)        │   │
│       ↓                                         │   │
│   ┌───┴───────────┐                             │   │
│   │               │                             │   │
│  GO             PIVOT                           │   │
│   ↓               └─────────────────────────────┘   │
│  [Phase 6] Full Version                             │
│    (art, monetization, advanced meta)               │
│                                                     │
└─────────────────────────────────────────────────────┘
```

---

## Phase 0: Context Check

Before anything, check current state:

```
_workspace/mvp-eval.md exists (user already evaluated MVP) → ask: GO or PIVOT?
_workspace/gdd.md exists but no mvp-eval → resume from MVP build
_workspace/game-concepts.md exists but no gdd → resume from concept selection
_workspace/ does not exist → initial run
PIVOT requested → archive _workspace/ → _workspace_prev_{concept-name}/ → restart Phase 2
```

Always tell the user which phase you are resuming from.

---

## Phase 1: Project Brief

Collect only what is missing:

1. **Platform:** iOS / Android / Both
2. **Genre interest:** Any preference, or "let the researcher decide"
3. **Differentiation goal:** e.g. "unique mechanic", "specific theme", "underserved niche"
4. **Reference games** (optional)

If enough context exists, skip straight to Phase 2.

---

## Phase 2: Market Research

**Mode: Single sub-agent**

Invoke `product-researcher` with the project brief.

```
Instruction:
  Use: game-research skill
  Produce:
    - _workspace/market-research.md
    - _workspace/game-concepts.md  (3–5 concepts)
```

Present the concept shortlist to the user: title, one-line pitch, confidence rating. Wait for the user to pick one (or request more concepts). Do not proceed without selection.

If this is a PIVOT (returning from a failed MVP): remind the user of the already-researched concepts in `_workspace_prev_*/game-concepts.md` before re-running research — they may want to just pick a different concept from the existing list rather than do fresh research.

---

## Phase 3: Concept Selection

User picks a concept from `_workspace/game-concepts.md`.

Confirm the selected concept back to the user in one sentence: core mechanic + meta hook + what makes it different. Ask: "Ready to build the MVP for this?" Do not proceed until confirmed.

---

## Phase 4: MVP Build

**This phase is gameplay-first. No monetization. No full art. No advanced meta.**

**Mode: Sub-agents, sequenced (design first, then parallel implementation)**

### Phase 4A — MVP Design (sequential: game-designer first)

Invoke `game-designer`:

```
Instruction:
  Read: _workspace/game-concepts.md (selected concept), _workspace/market-research.md
  Use: core-loop-design skill + mvp-design skill
  Scope: MVP only — no monetization hooks, no economy design
  Produce:
    - _workspace/gdd.md           (MVP-scoped GDD)
    - _workspace/core-loop-spec.md
    - _workspace/level-design-spec.md  (20–30 levels, each with purpose tag,
                                        difficulty, decision point, principle check)
    - _workspace/meta-spec.md     (basic meta only: progression, save, retry, level select)

  Level design must satisfy all 5 MVP principles per level:
    P1: Instantly understandable visual core
    P2: Clear visual transformation payoff
    P3: Smooth physics & auto-resolving interactions
    P4: Start simple, progressively build complexity
    P5: Challenge decision-making with controlled randomness
```

After game-designer completes, present the level design summary to the user:
- Mechanic introduction order
- Difficulty curve overview (Easy/Med/Hard count)
- Level purpose breakdown (INTRODUCE/REINFORCE/SPIKE/BREATHER counts)

Ask: "Does this level curve look right? Any adjustments before we build?"
Incorporate feedback, then proceed to 4B.

### Phase 4B — MVP Implementation (parallel)

Invoke `gameplay-dev` and `meta-dev` simultaneously (`run_in_background: true`).

**gameplay-dev instruction:**
```
Read: _workspace/core-loop-spec.md, _workspace/gdd.md, _workspace/level-design-spec.md
Use: unity-implementation skill
Scope: Core mechanic + all 20–30 levels. Placeholder art only. No ad/IAP hooks.
Coordinate: align shared interface with meta-dev before coding
Produce:
  - _workspace/gameplay-code/
  - _workspace/gameplay-issues.md
```

**meta-dev instruction:**
```
Read: _workspace/meta-spec.md
Use: meta-progression skill
Scope: Level progression, save/load, level select, retry flow, level complete screen ONLY.
       No currency. No upgrade system. No economy.
Produce:
  - _workspace/meta-code/
  - _workspace/save-schema.json
```

### Phase 4C — MVP QA (incremental)

Invoke `qa-engineer` after each module completes.

```
Instruction (each pass):
  Read: completed module in _workspace/
  Use: qa-game skill
  Focus:
    - All 20–30 levels are playable end-to-end without crashes
    - Level progression saves and restores correctly
    - Each level's win/fail state triggers correctly
    - Cross-boundary check: level complete → progression update → save
  Produce: _workspace/qa-report.md (append each pass)
```

Resolve all Critical bugs before moving to Phase 5. High bugs: document and flag, but do not block.

---

## Phase 5: MVP Evaluation Gate

This is the only human decision gate in the loop. Present a structured evaluation brief to the user:

```markdown
## MVP Evaluation Brief

**Concept:** [name]
**Levels built:** [N] levels ([Easy/Med/Hard breakdown])
**QA status:** [Critical bugs: 0 / High bugs: N]

### What to evaluate:
1. Is the core loop fun on its own — without any meta or reward system?
2. Do the first 5 levels communicate the game without any explanation?
3. Does the visual transformation feel satisfying at level clear?
4. Does the difficulty curve feel right? Where does it get frustrating?
5. By level 10, do you want to keep playing?

### Your decision:
- **GO** → Proceed to full version (art, monetization, advanced meta)
- **PIVOT** → Return to market research, pick a different concept
- **ITERATE** → Keep the concept but revise specific levels or mechanics
```

Wait for the user's decision. Do not assume.

**If GO:** Proceed to Phase 6.
**If PIVOT:** Archive `_workspace/` → `_workspace_prev_{concept}/`, return to Phase 2. Remind user of other concepts in the previous research before re-running.
**If ITERATE:** Re-invoke only the relevant agents (game-designer for level revisions, gameplay-dev for mechanic changes). Return to Phase 5 evaluation after changes.

---

## Phase 6: Full Version Build

Only reached after explicit GO decision from user.

**Adds to the MVP foundation:**

**Phase 6A — Full design layer (parallel):**
- `art-director`: full visual identity, asset spec, animation guide
- `monetization-analyst`: ad placement strategy, IAP config, LTV targets
- `game-designer`: extended meta spec (upgrade tree, economy, events)

**Phase 6B — Full implementation (parallel):**
- `gameplay-dev`: polish, juice, visual effects on top of MVP code
- `meta-dev`: economy system, currency, upgrade tree, IAP hooks

**Phase 6C — Full QA:**
- `qa-engineer`: full regression + economy balance + monetization integration checks

**Phase 6D — Final delivery:**
```
docs/
├── GDD.md (full version)
├── art-style-guide.md
└── monetization-strategy.md
src/
├── gameplay/
└── meta/
```

---

## Data Protocol

| Strategy | When to use |
|----------|-------------|
| File-based (`_workspace/`) | All inter-agent artifacts |
| Return-value | Sub-agent result summaries |

File naming: `{phase}_{agent}_{artifact}.{ext}`
PIVOT archive: `_workspace_prev_{concept-name}/`

## Error Handling

- Agent failure → 1 retry → continue without that output, note gap in final report
- Level count below 20 → game-designer must add more levels before Phase 4B starts
- Any level fails a principle check → game-designer revises that level before implementation
- Critical QA bug → block phase transition, re-invoke responsible developer

## Test Scenarios

**Normal flow:**
Research → 3 concepts → user picks merge mechanic → MVP built (25 levels) → user plays → GO → full version

**Pivot flow:**
Research → pick runner → MVP built → user evaluates: "core loop isn't fun" → PIVOT → existing concepts reviewed → pick puzzle mechanic → new MVP

**Iterate flow:**
MVP built → "levels 15–20 are too hard too fast" → ITERATE → game-designer revises difficulty curve for levels 14–22 → re-QA → re-evaluate
