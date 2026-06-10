# Arrow Redirector — Acceptance Criteria
_Last updated: 2026-06-10_

## Overview
A fixed tile with a set direction (↑↓←→); a critter sliding onto it immediately changes to that direction and continues sliding until the next obstacle.

## Linked Docs
- spec: _none yet — see Overview above and core-loop-spec.md §4 (slide resolution)_
- architecture: ../../architecture/data-architecture.md

## Definition of Done
- [ ] Spec reviewed and signed off
- [ ] Code written and peer-reviewed
- [ ] Behaviour verified in Unity Editor

## ✅ Test Environment Gate
All of the following must pass before merging to the test branch:

- [ ] A critter sliding onto an arrow tile changes direction immediately.
- [ ] The critter continues sliding in the new direction until the next obstacle.
- [ ] Loop guard: an arrow pointing back at the critter's source (already-used this resolve) → critter stops ON the tile (`redirectedCells` HashSet + `maxSteps` bound, no ping-pong).
- [ ] The arrow does not move and acts as a pass-through (not a wall) for critters that land on it; it is solid only in the sense that it always redirects.
- [ ] Two arrows in sequence: the critter follows both redirects.
- [ ] The critter exits through the correct gate after redirection.
- [ ] A critter that **stops on** an arrow cell leaves the arrow underlay state consistent when it later moves (no cell-state leak — see M-4).

## 🚀 Production Gate
All of the following must pass before shipping to production:

- [ ] L21 (intro level) plays correctly end-to-end on device.
- [ ] L28 (spike level with arrow chain) plays correctly end-to-end on device.
- [ ] Arrow direction is visually clear at the 45° camera angle.
- [ ] Arrow tiles render correctly on small (iPhone SE) and large (Pro Max) screens.
- [ ] Arrow + gate interaction verified on device: redirect toward a matching gate exits; toward a non-matching gate stops at the wall.

## Known Risks / Edge Cases
- **H-3 (fixed):** exit animation previously slid to the wrong wall for arrow-redirected critters. `SlideMove` now carries `exitFromCell` + `exitDir`; re-verify the redirect-then-exit visual path.
- **M-3 (verified non-issue):** occupancy is re-checked every loop iteration including post-redirect, so redirecting onto an occupied cell is handled.
- **M-4 (verified non-issue):** stop-on-arrow marks the cell `Critter` over the arrow underlay; restore checks `_arrowAt` and restores correctly. Re-verify if cell-state code changes.
- **Loop guard bound:** confirm `maxSteps` is never the actual stop reason in shipped levels — it is a safety net, not intended gameplay.
