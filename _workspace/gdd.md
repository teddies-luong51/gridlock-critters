# Gridlock Critters — Game Design Document (MVP)

*Concept 5 from `game-concepts.md`. MVP-scoped per `mvp-design` skill. Date: 2026-06-09.*
*Author: game-designer. Consumers: game-director, gameplay-dev, meta-dev.*

---

## 1. Game Overview

| Field | Value |
|-------|-------|
| **Name** | Gridlock Critters |
| **Genre** | Spatial / slide-jam puzzle with a physical-tilt twist |
| **One-line pitch** | Tilt your phone to slide every critter home through its matching gate — one wrong lean and the dock gridlocks. |
| **Platform** | Mobile (iOS + Android), portrait orientation, Unity 3D |
| **Core fantasy** | "I am physically tipping a board of animals, and they all obey gravity at once — I solve the jam by choosing *which way the world leans*." |
| **Session length** | 30–45 seconds per level, 2–5 min per sitting |
| **MVP goal** | Validate that physical-tilt slide-jam is fun, legible, and retainable across a 25-level curve. |

**Why this concept (from research):** Spatial/block puzzle is the fastest-growing, highest-revenue hybrid-casual lane (Color Block Jam: $175K → $42M/quarter). Gridlock Critters keeps that proven, ad-legible core but swaps direct drag for a **fresh physical verb — tilt** — which is novel, instantly demonstrable in a 5-second ad creative, and uniquely tied to mobile hardware (accelerometer). The cozy-critter theme rides current cultural momentum (Capybara Go).

---

## 2. Core Loop (the 30-second cycle)

```
   SEE                TILT               SLIDE              EXIT              RESOLVE
 (read board)  →  (lean phone L/R/U/D) → (all critters → (matched critters → (board state
                                          shift at once)   leave via gate)    settles)
       ▲                                                                          │
       └──────────────────────── repeat until all critters exited ───────────────┘
                                            │
                                            ▼
                                    WIN (all exited)
```

**Action → Feedback → Reward, beat by beat:**
1. **Action:** Player physically tilts the phone in one of four directions (Left / Right / Up / Down).
2. **Immediate feedback:** The 3D board *leans* ~15° toward the tilt direction (board-tilt animation), and **all critters slide simultaneously** in that direction with an ease-out curve. Slide sound + soft thud on collision.
3. **Resolution:** Each critter travels until it hits a wall, another critter, or a gate. If it reaches a **matching-color gate** on the destination edge, it **exits** through it (pop + sparkle + cozy chime). Non-matching gates act as solid wall.
4. **Reward:** Exited critters animate out and the dock visibly empties. When the **last critter exits**, the level-complete celebration fires.

The whole cycle from first tilt to "board fully cleared" is **3–6 tilts, 30–45 seconds**. Failure (gridlock) is recognized instantly and retry is one tap.

---

## 3. Input Model

**Primary input: physical phone tilt via accelerometer.** (LOCKED)

| Parameter | Value | Rationale |
|-----------|-------|-----------|
| Directions | Left, Right, Up, Down (4-way, axis-aligned) | Matches grid axes; no diagonal ambiguity. |
| Detection | Compare device tilt angle on X and Y axes against a threshold | Whichever axis crosses threshold first, and by the larger magnitude, wins. |
| Tilt threshold | **~15° from the neutral holding angle** | Deliberate lean required; prevents accidental micro-tilts. Calibrated to a baseline captured at level start (player's natural holding angle = neutral). |
| Dominant-axis lock | Only the **single largest-magnitude axis** registers per gesture | Prevents diagonal/ambiguous moves; one clean direction per tilt. |
| Cooldown | **0.35 s** input lock after a registered tilt | Lets the slide animation read; prevents a single physical lean from firing repeatedly. |
| Re-arm | Device must return within ~8° of neutral before the next tilt registers | Forces a deliberate "lean → return → lean" rhythm, like tipping a tray. |
| Accessibility fallback | On-screen D-pad (4 arrows) toggle in settings | For players who can't/won't tilt (lying down, accessibility). Same resolution logic. |

**Calibration:** At each level load, a 0.5 s "hold steady" captures the neutral angle so the game works whether the player holds the phone flat, at 45°, or upright.

---

## 4. Camera & Perspective

**3D perspective, fixed 45° angled chess-board view.** (LOCKED)

- The board is a 3D tray of square cells viewed from a **45° elevated angle**, like looking down at a chessboard from across the table. Depth is readable; critters are clearly 3D objects sitting in cells.
- **Camera is fixed** — it does **not** rotate with the tilt. Only the **board model leans ~15°** toward the tilt direction so the player feels the physical cause-and-effect without losing spatial orientation.
- Gates are visible as **colored openings in the four surrounding walls** of the tray. Wall color = the color of critter that may exit there.
- Player always sees: the full grid, every critter (color-coded), every gate (color-coded, on the four edges), and the tray walls. No scrolling, no hidden cells. Entire puzzle fits one screen.

**Visual identity:** Bright, high-contrast flat-shaded critters (ad-legible) on a warm wooden tray. The tilt-lean animation is the signature "money shot" for UA creative.

---

## 5. Win / Fail Conditions (per level)

**Win:** Every critter has exited through its matching-color gate. Board is empty → level complete.

**Fail (gridlock):** No tilt direction can change the board state in a way that progresses toward a solution — i.e., the player is stuck. For MVP we treat fail softly:
- There is **no hard loss screen** and **no move limit** in MVP.
- The player may **Undo** the last tilt (one-step undo, unlimited) or **Retry** the level from scratch at any time, both instant and free.
- A level is only ever "failed" in the sense that the player chooses to retry. This keeps frustration low and maximizes the "try again" loop the genre depends on.

*(A formal move-counter / star rating is explicitly deferred to the full version — see §8.)*

---

## 6. Progression Structure (25 levels, 5 chapters)

Levels are grouped into **5 chapters of 5 levels each**, each chapter layering one new mechanic on top of the last (per MVP Principle 4 — one new element at a time).

| Chapter | Levels | Theme / Grid | New mechanic introduced | Difficulty band |
|---------|--------|--------------|-------------------------|-----------------|
| **1 — The Dock** | 1–5 | 5×5 | Core: tilt → slide → exit through matching gate | Easy |
| **2 — Traffic** | 6–10 | 5×5 | **Blocking** — critters block each other; exit order matters | Easy → Medium |
| **3 — Detours** | 11–15 | 5×5 / 6×6 | **Multi-step chains** — a critter must be repositioned before it can reach its gate | Medium |
| **4 — Rush Hour** | 16–20 | 6×6 | **Gate scarcity** — fewer gates than critters on an axis; forces sequencing | Medium → Hard |
| **5 — Gridlock** | 21–25 | 6×6 | **Mastery** — all mechanics combined, tight solution space | Hard |

**Difficulty curve:** smooth ramp with deliberate BREATHER levels after every SPIKE (no two SPIKEs consecutive). See `level-design-spec.md` for per-level tags and the full curve.

**Pacing target:** Levels 1–5 each clearable in <30 s, near-100% success. Chapter 5 levels demand near-optimal play (30–60 s, multiple retries acceptable).

---

## 7. Basic Meta (MVP — no currency, no economy)

Per `mvp-design` Basic Meta. Detailed in `meta-spec.md`.

1. **Sequential unlock** — clear level N to unlock N+1. Current level highlighted.
2. **Level select screen** — a grid of 25 buttons: locked (lock icon) / unlocked (number) / completed (checkmark).
3. **Save / resume** — persists which levels are completed and the last level played. Relaunch resumes at the last incomplete level.
4. **Retry & Undo** — instant, free, unlimited. Undo = one-step; Retry = full reset.
5. **Level-complete screen** — celebration animation + "Next Level" and "Level Select" buttons.

That is the entire meta. Its only job is to make the 25 levels navigable and resumable.

---

## 8. Out of Scope for MVP

Explicitly **excluded** (deferred to full version):
- **Monetization:** no ads (IAA), no IAP, no fail-offers, no starter packs, no season pass.
- **Economy:** no soft/hard currency, no boosters, no lives, no energy.
- **Meta depth:** no critter collection, no habitat/sanctuary build, no cosmetic variants, no streaks, no daily events, no leaderboards.
- **Scoring:** no stars per level, no move-counter rating, no timers shown as score.
- **Art production:** placeholder flat-shaded critters and a single wooden tray; no full art direction, no polished animations beyond functional slide/lean/exit, no narrative.
- **Audio:** minimal functional SFX only (slide, thud, exit, win); no music score, no VO.

The MVP ships the **core loop + 25 levels + navigable meta** and nothing else, so we can answer one question: *is physical-tilt slide-jam fun and retainable enough to build the full collection meta on?*
