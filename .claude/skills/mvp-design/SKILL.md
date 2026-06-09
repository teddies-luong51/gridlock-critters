---
name: mvp-design
description: Designs the MVP scope for a hybrid casual game — gameplay design, 20–30 levels with difficulty curve, and basic meta features only. No monetization, no full art, no production polish. Use this skill when scoping or building an MVP, designing levels for an MVP, applying MVP principles to a game concept, or evaluating whether a design meets MVP quality gates. Triggers: "MVP", "minimum viable", "prototype", "level design", "20 levels", "30 levels", "design the levels", "MVP scope", "build the MVP".
---

## MVP Scope Definition

An MVP exists to validate one thing: **is the core gameplay loop fun and retainable enough to build on?**

**MVP includes:**
- Core gameplay mechanic (fully implemented)
- 20–30 levels with structured difficulty curve
- Basic meta: level progression, save/load, level select screen
- Placeholder/minimal art (functional, not polished)
- Basic sound feedback (optional but recommended)

**MVP explicitly excludes:**
- Monetization (no ads, no IAP, no economy)
- Full art direction and polished assets
- Social features, leaderboards, events
- Advanced meta (upgrade trees, currencies, gacha)

The goal is a playable loop that can be evaluated in a single session. Ship lean, learn fast.

---

## The 5 MVP Principles

These are non-negotiable quality gates. A level design or mechanic that fails any principle must be revised before moving to implementation.

---

### Principle 1: Instantly Understandable Visual Core

The goal, action, and constraints must be communicated through visuals alone within the first 3 seconds — no tutorial text, no language dependency.

**Design checklist:**
- [ ] Goal is visible on screen before the player takes any action
- [ ] The primary action (tap, drag, swipe) is implied by the visual state
- [ ] Constraints (limits, boundaries, slots) are visually distinct
- [ ] A player who has never seen the game can attempt level 1 without reading anything

**When writing level specs, for each level state, answer:**
- What does the player *see* as the goal?
- What does the interface *imply* they should do?
- What would visually *block* or warn them of a constraint?

**Reference pattern — Pixel Flow:**
- Goal visible: colored blocks sitting inside a loop
- Action implied: fill slots with matching pieces
- Constraint visible: numbered tray slots with remaining count

---

### Principle 2: Clear Visual Transformation Payoff

Every successful action or level clear must produce a striking before→after visual change. The transformation must be readable at a glance.

**Transformation patterns to apply:**
| From | To | Example |
|------|----|---------|
| Chaos / disorder | Order / alignment | Sand Loop, Loop Sort |
| Incomplete / partial | Complete / filled | Pixel Flow block clear |
| Cluttered / dense | Clean / sparse | any sorting mechanic |
| Dull / grey | Colorful / vibrant | color-matching mechanics |

**Design checklist:**
- [ ] Before-state is visually "uncomfortable" or clearly incomplete
- [ ] After-state is visually satisfying and clearly resolved
- [ ] The transformation happens smoothly (animation, not instant swap)
- [ ] The payoff moment is distinct — player knows they succeeded by sight alone

**For each level spec:** describe the visual before-state and the visual after-state explicitly.

---

### Principle 3: Smooth Physics & Auto-Resolving Interactions

Gameplay motion should feel fluid and responsive. Chain reactions and secondary interactions should resolve automatically — the player triggers, the system completes.

**Design checklist:**
- [ ] Primary input is simple (1 gesture type per interaction)
- [ ] Secondary effects (settling, cascading, stacking) resolve on their own
- [ ] No interaction requires precise timing — outcomes are determined by position/logic, not reflexes
- [ ] Failed states feel "physics honest" — player understands why it went wrong from the visual

**Implementation note for gameplay-dev:**
- All interactive objects need easing curves (ease-in-out on movement, elastic on settle)
- Collision resolution must never freeze or stall — always reach a final state
- Frame budget for auto-resolve animations: 0.3–0.8 seconds
- Player input should never feel ignored — immediate visual acknowledgment on every tap/drag

**Reference pattern — Color Bus Trip:**
- Player assigns direction, buses move and queue automatically
- Pickup/drop-off resolves without further input
- Collisions produce visible queuing, not blocking

---

### Principle 4: Start Simple, Progressively Build Complexity

Level 1 introduces exactly one mechanic element. Each subsequent level either introduces one new element or reinforces existing elements at higher difficulty. Never introduce two new mechanics in the same level.

**Complexity ladder structure for 20–30 levels:**

```
Levels 1–5:   Tutorial zone — core mechanic only, high success rate
Levels 6–10:  Mechanic 2 introduced, combined with core
Levels 11–15: Mechanic 3 or first RNG/variation element
Levels 16–20: All mechanics combined, difficulty ramps
Levels 21–25: New variation or loop twist (if targeting 25+)
Levels 26–30: Mastery levels — tight margins, complex board states
```

**Level purpose taxonomy — every level must have exactly one:**
- `INTRODUCE` — first appearance of a mechanic or element
- `REINFORCE` — practice an already-introduced mechanic under moderate pressure
- `COMBINE` — first time two previously separate mechanics appear together
- `SPIKE` — intentional hard level (followed by an easier level for pacing)
- `BREATHER` — intentional easy level after a spike
- `MASTERY` — near-optimal play required

**Difficulty pacing rule:** Never place two SPIKE levels consecutively. Alternate: hard → breather → hard or hard → reinforce → hard.

**Reference pattern — Color Bus Trip mechanic ladder:**
1. Basic color sorting (INTRODUCE)
2. More colors + tighter timing (REINFORCE)
3. Disguise mechanic — buses change appearance (INTRODUCE RNG)
4. Key system — changes clearing priority (INTRODUCE decision layer)
5. Two-color stops — resolves to two destinations (COMBINE)

---

### Principle 5: Challenge Decision-Making with Controlled Randomness

Levels must require the player to think and adapt — not just execute a memorized sequence. At the same time, randomness must be bounded: the player must always have enough information to make a *reasonable* plan, even if the optimal solution isn't obvious upfront.

**Design checklist:**
- [ ] The level has at least one decision point where multiple valid approaches exist
- [ ] At least one "hidden" element that reveals itself during play and forces adaptation (not at the start)
- [ ] Failure is clearly caused by the player's decision, not by the game being unfair
- [ ] A player who fails can articulate *why* they failed and what they'd do differently

**Controlled randomness patterns:**
- **Delayed reveal:** A key piece or obstacle only becomes visible after the player takes 2–3 actions
- **Order variation:** Same elements but shuffled spawn order each attempt
- **Soft RNG gates:** A mechanic that has 2–3 possible outcomes, all of which the player can respond to if they planned ahead
- **Hidden dependency:** Two elements that look independent but interact (discovered mid-level)

**What to avoid:**
- Pure RNG that makes a level unwinnable regardless of decisions
- A single correct solution with no alternative paths (puzzle, not decision-making)
- Randomness the player has no way to anticipate or react to

---

## Level Spec Template

Each level in `_workspace/level-design-spec.md` must follow this format:

```markdown
### Level [N] — [Purpose Tag: INTRODUCE / REINFORCE / COMBINE / SPIKE / BREATHER / MASTERY]

**Difficulty:** Easy / Medium / Hard
**Est. completion time:** X–Y seconds
**New element (if INTRODUCE):** [mechanic or element name]

**Visual Before-State:**
[Describe what the player sees when the level loads — board state, pieces, constraints visible]

**Visual After-State (win):**
[Describe what the cleared/completed state looks like]

**Core Decision Point:**
[What is the key choice the player must make? What are the valid approaches?]

**Controlled Randomness (if any):**
[What element has variability or delayed reveal?]

**Failure Mode:**
[How does the player fail, and what does it visually communicate?]

**Principle Check:**
- [ ] P1: Goal/action/constraint visible immediately
- [ ] P2: Clear transformation payoff
- [ ] P3: Interactions auto-resolve smoothly
- [ ] P4: Single new element (or pure reinforcement)
- [ ] P5: At least one real decision point
```

---

## Basic Meta for MVP

Only implement what is needed to evaluate the core loop. No economy, no currency, no IAP hooks.

**Required meta features:**
1. **Level progression** — levels unlock sequentially, current level highlighted
2. **Level select screen** — shows completed (star/checkmark), current, locked levels
3. **Save system** — persist which levels are completed, restart from last unlocked
4. **Retry flow** — instant retry on fail, no wait time, no cost
5. **Level complete screen** — celebration moment (animation), "Next Level" button

**Explicitly excluded from MVP:**
- Stars / score rating per level (can add in full version)
- Soft/hard currency
- Lives or energy system
- Any ad placement hooks
- Any IAP hooks

The meta exists solely to make the 20–30 levels navigable and resumable. Nothing more.
