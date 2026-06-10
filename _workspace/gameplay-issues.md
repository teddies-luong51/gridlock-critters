# Gridlock Critters — Gameplay Implementation Issues / QA Notes

Flagged by gameplay-dev for QA. Edge cases, TODOs, and tuning items.

## Edge cases handled in code
- **Arrow redirect loop guard** — if a redirected direction would send a critter back onto an arrow tile it has already used this resolve, the critter STOPS on the tile instead of ping-ponging forever. Implemented via `redirectedCells` HashSet in `SlideResolver.Slide` plus a hard `maxSteps` safety bound.
- **No-op tilts** — if no critter changes cell and none exits, `ResolveMoves` returns an empty list. The board lean still plays (visual feedback) but no slide. Not a fail.
- **Simultaneous resolution** — critters are processed from the destination wall inward (`CompareForOrder`) so a critter never moves into a cell a not-yet-processed critter is about to vacate. Matches core-loop-spec §4.
- **Non-matching gate = solid wall** — `GateController.CanExit` requires color match AND open; otherwise the critter stops as if hitting a wall.
- **Input lock** — `BoardController._isAnimating` blocks all tilts during the lean+slide+exit pipeline. TiltInputHandler also enforces 0.6s cooldown + re-arm (device must return near neutral).

## TODOs / deferred
- **Object pool** — Level 30 (and the hardest Chapter 5 levels) have 6 critters; the critter object pool MUST be sized >= 6. Current LevelManager uses Instantiate/Destroy; swap to a pool before perf testing on device.
- **Undo / Retry** — `LevelManager.RetryCurrent()` exists; one-step UNDO (snapshot the pre-tilt board state stack, per spec §8) is NOT yet implemented. Owner: gameplay-dev next pass or meta-dev.
- **Gridlock soft-hint detector** — deferred per spec §8 (optional). Not implemented.
- **Level solvability validator** — build-time BFS over tilt-states (spec §9) not implemented; relies on hand-verified level data for now.
- **Calibration** — TiltInputHandler uses raw `Input.acceleration` thresholds. The 0.5s "hold steady" neutral-angle capture from GDD §3 is NOT yet implemented; add a calibration pass at level load for players holding the phone at non-flat angles.
- **Accessibility D-pad** — TiltInputHandler.SetInputEnabled exists to suspend tilt; the on-screen 4-arrow D-pad fallback (GDD §3) is UI work, not yet wired.

## QA verification targets
- Verify exit animation timing (critter slides to wall cell, then through gate, then pop) reads correctly at 45° camera.
- Verify arrow + gate interaction: a critter redirected by an arrow toward a matching gate should exit; toward a non-matching gate should stop at the wall.
- Verify multi-critter packing against a wall with mixed colors (Chapter 4 gate-scarcity levels).
- Confirm slide speed (9 cells/sec) keeps the longest 6-cell slide within the 0.3–0.8s budget.
