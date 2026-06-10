# Gridlock Critters — Product Document
_Version: MVP 1.0 · Platform: iOS + Android · Date: 2026-06-10_

---

## Executive Summary

Gridlock Critters is a spatial puzzle game where you physically tilt your phone to slide a whole board of critters home through their matching-color gates — one wrong lean and the board gridlocks. It targets the fastest-growing lane in mobile (block/spatial puzzle, the same lane as the $42M-per-quarter Color Block Jam) but replaces the usual finger-drag with a fresh, hardware-native verb: tilt. The MVP is now code-complete and passing QA: a working core loop, a 30-level difficulty curve, and a navigable, save-and-resume meta — built to answer one question before we invest in full art and monetization: _is physical-tilt slide-jam fun and retainable?_

---

## 1. Product Overview

- **One-line pitch:** Tilt your phone to slide every critter home through its matching gate — one wrong lean and the board gridlocks.
- **Platform:** iOS + Android (Unity 3D)
- **Target audience:** Puzzle game players, ages 15–35, casual to mid-core
- **Genre:** Hybrid casual puzzle (spatial / slide-jam)
- **Core fantasy:** "I physically tilt my phone and watch critters slide into their matching gates — satisfying, spatial, and tactile."
- **Differentiator vs. existing puzzle games:** The dominant spatial-puzzle hit, Color Block Jam, grew from $175K to $42M in a single quarter on a proven core: drag colored blocks out through matching gates. Gridlock Critters keeps that proven, ad-legible core but swaps direct drag for a **fresh physical verb — tilt the whole board.** This is novel, instantly demonstrable in a 5-second ad, and uniquely tied to mobile hardware (the accelerometer). Where Color Block Jam's main weakness is a thin meta and difficulty-spike frustration, our tilt verb gives an instantly readable creative hook that competitors using finger-drag cannot copy on hardware. The cozy-critter theme also rides current cultural momentum (e.g. Capybara Go).

---

## 2. Core Mechanic

**What the player sees.** A small board — a wooden tray of square cells, like a tiny chessboard viewed from a slight angle. Sitting in the cells are colorful critters. Around the four edges of the tray are colored gates: openings in the walls. A gate's color tells you which critter is allowed to leave through it.

**What the player does.** The player tilts the actual phone — left, right, up, or down. That's the only control. No tapping, no dragging.

**What happens.** The whole board reacts at once. Every critter slides in the direction you tilted, all at the same time, until it bumps into a wall, another critter, or a gate. It feels exactly like tipping a tray of marbles.

**The matching rule.** A critter can only leave through a gate of its own color. A red critter slides out a red gate. If a critter slides up against a gate of the wrong color, that gate acts like a solid wall — the critter just stops there.

**Win condition.** Clear the board. When every critter has slid out through its matching gate, the level is complete. Most levels take 3 to 6 tilts and about 30–45 seconds.

**Why it feels good.** The loop has a clean, physical cause-and-effect chain: you tilt the phone → the board visibly leans in that direction → all the critters slide together with a smooth, weighty motion → matching critters pop out through their gate with a little sparkle and chime. Tilting, then watching the board obey, is the whole satisfaction.

**Simple example (2 critters).** A red critter and a blue critter share one row. The red gate is on the right wall; the blue gate is on the left wall.

```
   Tilt RIGHT  →

   [ R ][ B ][   ][   ]| RED GATE
                        (right wall)

   Result: both critters slide right.
   R reaches the RED gate and exits (pop!).
   B stops against the right wall — wrong color, can't exit.

   Then tilt LEFT  →
BLUE GATE |[   ][   ][ B ][   ]
(left wall)

   B slides left into the BLUE gate and exits.  Board clear → WIN.
```

The puzzle is figuring out the _order_ and _direction_ of tilts, since one tilt moves everything at once.

---

## 3. Mechanic Progression (3 Tiers over 30 Levels)

The game teaches itself one idea at a time. Each tier adds a single new element and then lets the player build on it.

### Tier 1 — Core (Levels 1–10)
**Mechanic: slide, gate, and critters blocking each other.** Players learn the basic verb (tilt → everything slides → matching critters exit) and quickly discover that critters get in each other's way. Because every critter moves on every tilt, freeing one critter can trap another.
- **New decision it adds:** _What order do I send them out in?_ Sliding the blue critter to its gate first might park the red critter behind a wall. The player has to sequence their tilts.
- **Example:** In Level 8, two critters share a column heading toward the same edge. Tilt down and the front one exits — but the back one is now jammed against the far wall with no gate on that side. You have to tilt sideways first to split them, then send each home separately.

### Tier 2 — Static Blocker (Levels 11–20)
**Mechanic: the immovable stone tile (`#`).** A stone block occupies a cell permanently. It never moves, and critters can't pass through it. It divides the board and blocks straight-line paths.
- **New decision it adds:** _How do I route around the obstacle?_ The shortest path is no longer available; the player must use a stone block as a wall to stop a critter at a useful spot.
- **Example:** In Level 14, a stone block sits in column C. Your red critter can't slide straight through to its gate on the far side — you need to route it around via two tilts, using the block to park it in the right lane before the final push.

### Tier 3 — Arrow Redirector (Levels 21–30)
**Mechanic: the arrow tile that turns a critter 90°.** When a sliding critter passes over an arrow tile, it changes direction to follow the arrow. This lets a single tilt produce an L-shaped path instead of a straight one.
- **New decision it adds:** _How do I use a turn to reach a gate I can't slide to directly?_ The player now plans bent paths and combines arrows with stone blocks and other critters.
- **Example:** In Level 24, a green critter can never reach its gate by sliding straight — but an arrow tile in its path turns it left toward the green gate. The player learns to read arrows as routing tools, lining up a critter so that one tilt sends it through the turn and out the gate.

---

## 4. Level Design Principles

The MVP was built against five plain rules:

- **P1 — Read it at a glance.** You understand what to do just by looking at the board. Colors, gates, and critters communicate the goal with no tutorial text. The first levels _are_ the tutorial.
- **P2 — Exits feel good.** When a critter reaches its gate, there's a clear payoff: a pop, a sparkle, and a cozy chime. Progress is always rewarded, never silent.
- **P3 — No ambiguity.** When you tilt, the result is always clean and predictable. Critters resolve deterministically — there's never confusion about where something will end up.
- **P4 — One new thing at a time.** Each block of levels introduces a single new element, then spends a few levels reinforcing and combining it before the next one appears.
- **P5 — Every level has a decision.** No level is a freebie. There's at least one meaningful choice where a wrong tilt teaches you something about how the board works.

**Difficulty curve overview** (30 levels, 3 tiers of 10):

| Range | Tier | Count | Difficulty |
|-------|------|-------|-----------|
| L1–10 | Core (slide + gate + blocking) | 10 | Mostly Easy, ramping to Medium |
| L11–20 | Static Blocker | 10 | Medium, ramping to Hard |
| L21–30 | Arrow Redirector | 10 | Hard, ramping to Very Hard (mastery) |

Within each tier the curve ramps smoothly, with a deliberate BREATHER after every difficulty SPIKE — no two spikes back to back.

**Level purpose taxonomy:**
- **INTRODUCE** — the first appearance of a new mechanic, in a deliberately simple board so the idea is unmistakable.
- **REINFORCE** — repeats the new mechanic in a slightly varied setup to cement understanding.
- **COMBINE** — mixes the new mechanic with earlier ones so they interact.
- **SPIKE** — a deliberately hard level that demands near-optimal play; the curve's peaks.
- **BREATHER** — an easier level placed right after a spike to relieve tension and restore confidence.
- **MASTERY** — late-tier levels that combine everything with a tight solution space; the test of true fluency.

---

## 5. Game Feel & Feedback

- **Board tilt animation:** When you tilt, the board physically leans about 15° in that direction _before_ the critters move. This sells the cause-and-effect — you tipped the tray, so things slide. It makes the action feel grounded and real.
- **Critter slide:** Critters don't teleport. They slide smoothly across the board and pack against walls or against each other, with a soft thud on contact and a tiny settle bounce. Longer slides feel weightier than short ones.
- **Exit animation:** When a critter reaches its matching gate, it continues through the opening, pops with a small scale burst, sparkles, and disappears with a cozy chime. The exit is the reward.
- **Win sequence:** When the last critter exits, a celebration plays across the board — a sparkle/confetti burst and a win chime — and the level-complete screen appears.
- **Camera:** A 3D perspective view angled at 45°, like looking down at a chessboard from across the table. You see depth: critters slide toward and away from you, not just left and right. The camera stays fixed — only the _board_ leans — so you never lose your orientation.
- **No permission needed:** Tilt uses the phone's built-in accelerometer. No camera access, no location, no special permissions. The game just works the moment it opens.

---

## 6. Meta Systems (Technical)

- **Level progression rule:** Level N unlocks when Level N-1 is completed. Level 1 is always unlocked. Completing level N adds it to `completedLevels` and unlocks N+1.
- **4 level-select states:**
  - **Locked** — grey button + lock icon, not interactable (tap = small no-op shake).
  - **Unlocked** — colored button showing the level number, tappable.
  - **Current** — the lowest unlocked-and-incomplete level; Unlocked styling plus a pulsing highlight/glow ring. Exactly one button at a time.
  - **Completed** — colored button + checkmark badge, still replayable.
- **Save system:** Single JSON blob in `PlayerPrefs` under key `gridlock_save_v1`. Stores `completedLevels` (`int[]`), `lastPlayedLevel` (`int`), and `totalPlayTime` (`float`). Write triggers: on level complete, on pause→LevelSelect/quit, and on app pause/quit.
- **Resume logic:** On app open, the game loads the **lowest-index incomplete level**, derived from `completedLevels` — _not_ the raw `lastPlayedLevel` (which may point at an already-cleared replay). If every level is complete, it opens LevelSelect instead.
- **Retry flow:** Resets the current level to its initial board state in-place. Instant, free, unlimited, no confirmation. Does not change `completedLevels`.
- **Undo:** One-step undo via the HUD button. The interface and an undo stack stub exist (`IBoardController.UndoLastMove`), but full one-step undo is deferred to the post-MVP polish pass.
- **No economy:** No coins, no lives, no energy, no stars, no move counter. Pure puzzle progression. The meta exists only to make the levels navigable and resumable.

---

## 7. Technical Architecture (Technical)

- **Engine:** Unity 3D (C#).
- **Input:** `Input.acceleration` (accelerometer; no permissions on iOS/Android). Low-pass filtered, ~0.35 tilt threshold, with a tilt cooldown and a re-arm requirement (device must return near neutral before the next tilt registers). `TiltInputHandler` enforces a ~0.6s cooldown in code and exposes `SetInputEnabled` for suspending tilt (used by the deferred accessibility D-pad).
- **Camera:** Position (0, 12, −10), Rotation (45°, 0, 0), FOV 50. Fixed throughout play — only the board model leans ~15° on tilt.
- **Tilt pipeline:** `TiltInputHandler` → `BoardController.ExecuteTilt` → `BoardTiltAnimator` (lean ~15°) → `SlideResolver` (logic) → `CritterPiece.SlideTo` (animation) → `WinChecker`.
- **SlideResolver:** Pure C# class. Processes critters from the destination wall inward (`CompareForOrder`) so a critter never moves into a cell a not-yet-processed critter is about to vacate. Handles critter-critter blocking, the StaticBlocker tile, the ArrowRedirector tile (with a `redirectedCells` HashSet loop guard plus a `maxSteps` safety bound), and treats a non-matching gate as a solid wall.
- **GameManager:** Single canonical state machine (Boot → MainMenu/LevelSelect → Playing → LevelComplete), living in `meta-code`. Holds the `ILevelManager` + `IBoardController` bridge references and applies `Time.timeScale` on pause. The earlier duplicate gameplay-side `GameManager` was collapsed into this one (QA C-1).
- **Gameplay↔meta bridge:** `LevelManager` implements `ILevelManager` and registers itself on `GameManager.Instance` in `Awake`; `BoardController` implements `IBoardController` (including an undo stub) and registers likewise. All meta UI (LevelSelect, LevelComplete, PauseMenu, HUD) routes through these bridge references.

**Key scripts:**

| Script | Role |
|--------|------|
| `TiltInputHandler` | Reads accelerometer, fires `OnTilt` event |
| `BoardController` | Tilt pipeline orchestrator; implements `IBoardController` |
| `SlideResolver` | Pure slide logic for all 3 mechanic types |
| `LevelManager` | Loads `LevelData` ScriptableObjects, instantiates board; implements `ILevelManager` |
| `SaveManager` | `PlayerPrefs` JSON persistence |
| `LevelProgressManager` | 30-level unlock state, 4 button states |
| `GameManager` | State machine + gameplay↔meta bridge |
| `WinChecker` | Detects all-critters-exited, drives `ChangeState(LevelComplete)` |

- **Mobile performance targets:** 60fps; object pool for critters (must be sized ≥6 for the hardest levels — currently Instantiate/Destroy, pool deferred); single draw call board mesh; no `GetComponent` in `Update`.

---

## 8. Out of Scope (MVP)

| Feature | Status | When |
|---------|--------|------|
| Monetization (ads / IAP) | Out of scope | Phase 6 (Full Version) |
| In-game currency / economy | Out of scope | Phase 6 |
| Full art & animation | Placeholder only | Phase 6 |
| Sound design | Functional SFX only | Phase 6 |
| Multi-step Undo (and full one-step Undo) | Deferred | Post-MVP polish |
| Leaderboard / social | Out of scope | TBD |
| Level editor | Out of scope | TBD |
| Level 26–30 `LevelData` assets | Need authoring | Before Phase 5 test |
| Tilt calibration (neutral-angle capture) | Deferred | Post-MVP |
| On-screen D-pad accessibility fallback | Deferred (hook exists) | Post-MVP |
| Build-time solvability validator (BFS) | Not implemented (hand-verified levels) | Post-MVP |

---

## 9. Open Issues & Risks

The three Critical defects from QA Pass 1 (C-1 duplicate `GameManager`, C-2 broken win chain, C-3 unregistered bridge) and H-3 (exit-animation path) have all been **fixed** in Fix Pass 1; QA verdict is now PASS. The remaining open items:

| ID | Severity | Description | Impact | Owner |
|----|----------|-------------|--------|-------|
| H-1 | High | 0-based index in code vs 1-based in meta-spec; `lastPlayedLevel` default `0` collides with "level 1" as a sentinel | Doc/spec divergence, no runtime crash; latent ambiguity for future `lastPlayedLevel` consumers. Fix: keep 0-based, update spec, use `-1` "none" sentinel | game-designer |
| H-2 | High | `TotalLevels = 30` but 30 `LevelData` assets must be authored; specs still say 25 | Levels 26–30 unplayable until authored — tapping an unlocked button 26+ loads nothing / resume hits a missing asset. Fix: author all 30, reconcile specs, guard LevelSelect to `min(TotalLevels, levels.Length)` | gameplay-dev |
| M-1 | Medium | `WinChecker` and gameplay `LevelManager` both react to the same win; dead static `NotifyWin()` bridge | Will double-fire `ChangeState(LevelComplete)` now that the win path works. Fix: pick one win owner (`WinChecker`) | gameplay-dev |
| M-2 | Medium | `LevelCompleteController.Show()` calls `MarkCompleted` on every overlay show (incl. `OnEnable` re-fire) | Re-marks/re-writes save on replay-complete; within spec but worth a `_shownForLevel` guard | meta-dev |
| L-2 | Low | `SaveManager.Load()` throws if `PlayerPrefs` holds a corrupt non-empty string | Hard crash on corrupt save. Fix: wrap `FromJson` in try/catch | meta-dev |
| L-4/Perf | Low | `LevelManager` uses Instantiate/Destroy per level; object pool deferred | Perf only, non-blocking for correctness; pool must be ≥6 before device perf test | gameplay-dev |
| L-5 | Low | `LoadLevel` `break`s the spawn loop on a null prefab, silently skipping all of that type | A single missing prefab silently breaks a level. Fix: `Debug.LogError` + skip | gameplay-dev |

Verified non-issues (kept for the record): M-3 (post-redirect occupancy is re-checked each iteration) and M-4 (stop-on-arrow underlay restores correctly).

---

## 10. Evaluation Criteria (Phase 5 Gate)

After playing the MVP, the product owner evaluates five questions:

1. **Fun without meta:** Is the core loop fun on its own — without any reward system, coins, or progression?
2. **Zero-tutorial clarity:** Do the first 5 levels communicate the game without any explanation?
3. **Satisfying exit:** Does it feel good when a critter exits through its gate?
4. **Difficulty curve:** Does the curve feel right? Where does it first get frustrating?
5. **Hook:** By Level 10, do you want to keep playing?

**Decision options after evaluation:**

| Decision | Meaning | Next step |
|----------|---------|-----------|
| GO | Core loop is fun, ship it | Phase 6: full art + monetization |
| ITERATE | Right concept, wrong levels/balance | Revise specific levels, re-evaluate |
| PIVOT | Core loop isn't fun enough | Back to research, pick new concept |
