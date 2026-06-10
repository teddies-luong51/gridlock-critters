# Core Tilt Mechanic — Acceptance Criteria
_Last updated: 2026-06-10_

## Overview
Tilting the board in one of four directions slides all critters until they hit a wall, another critter, a static blocker, a non-matching gate, or are redirected by an arrow — and lets matching critters exit through their gate.

## Linked Docs
- spec: ../core-tilt-mechanic/core-loop-spec.md
- architecture: ../../architecture/data-architecture.md

## Definition of Done
- [ ] Spec reviewed and signed off
- [ ] Code written and peer-reviewed
- [ ] Behaviour verified in Unity Editor

## ✅ Test Environment Gate
All of the following must pass before merging to the test branch:

- [ ] Tilt Left / Right / Up / Down each slide all critters in the correct direction.
- [ ] Critters stop against walls, other critters, and StaticBlockers (cell immediately before the obstacle).
- [ ] A non-matching gate stops the critter (treated as a solid wall, no exit).
- [ ] A matching, open gate allows the critter to slide through and exit.
- [ ] `BoardController._isAnimating` lock prevents a second tilt during the lean+slide+exit pipeline.
- [ ] Arrow redirect: a critter sliding onto an arrow changes direction mid-slide and continues to the next obstacle.
- [ ] Arrow loop guard: a critter whose redirect would send it back onto an already-used arrow this resolve stops ON the tile (no ping-pong, `maxSteps` never reached).
- [ ] Simultaneous resolution order is destination-wall-inward; no critter moves into a cell a not-yet-processed critter is about to vacate.

## 🚀 Production Gate
All of the following must pass before shipping to production:

- [ ] Tested on a physical iOS device — accelerometer tilt input confirmed for all four directions.
- [ ] Tested on a physical Android device — accelerometer tilt input confirmed.
- [ ] 60fps maintained on iPhone 12 and a mid-range Android during a rapid tilt sequence.
- [ ] Low-pass filter (or equivalent) prevents false tilts from hand shake.
- [ ] Longest 6-cell slide completes within the 0.3–0.8s budget at 9 cells/sec.
- [ ] TiltInputHandler 0.6s cooldown + return-to-neutral re-arm confirmed on device.
- [ ] Exit animation reads correctly at the 45° camera angle (slide to gate cell, through gate, pop).

## Known Risks / Edge Cases
- **L-1 (scene-load order):** `ExecuteTilt` binds to `TiltInputHandler.OnTilt` in `OnEnable`/`Start` but does not re-bind if `TiltInputHandler.Instance` is created after `Start`. Verify scene init order.
- **Calibration deferred:** raw `Input.acceleration` thresholds are used; the 0.5s "hold steady" neutral-angle capture (GDD §3) is NOT implemented. Players holding the phone at a non-flat angle may experience drift — flagged for next pass.
- **Accessibility D-pad deferred:** `SetInputEnabled` hook exists but the on-screen 4-arrow fallback is not wired.
- **H-3 (fixed):** exit animation now stores true exit cell + direction in `SlideMove`; re-verify redirected-then-exit paths slide to the correct wall.
