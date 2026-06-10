# Gridlock Critters — Gameplay Architecture

_Version: MVP 1.0 · Scope: tilt pipeline, slide resolution, mechanics · Date: 2026-06-10_

---

## 1. The Tilt Pipeline (step by step)

The pipeline turns one physical tilt into a fully resolved, animated board state. Every stage names the class that owns it.

| # | Stage | Class | What it does |
|---|-------|-------|--------------|
| 1 | Read accelerometer | `TiltInputHandler` | Samples `Input.acceleration` every frame, applies a low-pass filter to suppress jitter. |
| 2 | Detect a tilt | `TiltInputHandler` | When filtered magnitude crosses ~0.35 on an axis, the cooldown (~0.6s) has elapsed, and the device has re-armed (returned near neutral since the last tilt), it picks the dominant axis/sign and fires `OnTilt(TiltDirection)`. `SetInputEnabled(bool)` can suspend it (reserved for the deferred accessibility D-pad). |
| 3 | Receive the tilt | `BoardController.ExecuteTilt(dir)` | Subscribed to `OnTilt` (bound in `OnEnable` and re-bound in `Start` to survive ordering). No-ops if `_isAnimating` or `_grid == null`. |
| 4 | Snapshot for undo | `BoardController.SaveStateForUndo()` | Pushes a clone of `_grid` and a copy of `_critterAt` onto the undo stacks *before* mutating anything. |
| 5 | Lock + lean | `BoardController.TiltRoutine` → `BoardTiltAnimator.PlayTilt(dir)` | Sets `_isAnimating = true`, starts the ~15° visual lean, then waits `slideLeadDelay` (0.05s) so the lean visibly leads the slide. |
| 6 | Resolve | `SlideResolver.ResolveMoves(dir, _grid, _critterAt)` | Pure-C# computation of every critter's destination/exit. Returns `List<SlideMove>`. |
| 7 | Apply + animate | `BoardController.AnimateMoves` | Mutates the logical grid first (so all concurrent animations read consistent state), then launches every `CritterPiece.SlideTo` / `ExitRoutine` coroutine simultaneously and waits for all to finish. |
| 8 | Settle lean | `BoardController.TiltRoutine` | Waits for the lean coroutine to return to neutral. |
| 9 | Win check + unlock | `BoardController.CheckWin()` | `_critterAt.Count == 0`; sets `_isAnimating = false`; fires `OnTiltResolved(won)`. |

The whole thing is one coroutine (`TiltRoutine`), so the `_isAnimating` lock cleanly spans the entire resolve-and-animate window.

---

## 2. SlideResolver Algorithm

`SlideResolver` is a pure, deterministic, `MonoBehaviour`-free class. `Configure(gates, arrows)` is called once per level with the gate registry and arrow lookup. `ResolveMoves` does the work.

### Processing order: destination-wall-outward

Critters are sorted by `CompareForOrder` so the critter **nearest the destination wall is processed first**:

| Tilt | Sort |
|------|------|
| Right | descending x |
| Left | ascending x |
| Down | descending y |
| Up | ascending y |

This guarantees a critter never slides into a cell that a not-yet-processed critter is about to vacate — each critter packs against already-settled state. The resolver works on a mutable copy (`occupied`) of `critterAt`, removing/adding entries as each critter settles.

### Per-critter slide (`Slide`)

Walking one cell at a time in the move direction:

- **Off-board (wall reached):** compute the `WallSide` for the current move direction. Ask `_gates.CanExit(cell, wall, critterColor)`.
  - matching gate → `exits = true`, record `exitColor` and `exitDir` (the *post-redirect* travel direction).
  - solid wall or **non-matching gate → stop at current cell** (a wrong-color gate behaves exactly like a wall).
- **Critter-critter blocking:** if the next cell is in `occupied` and holds a *different* critter, stop at the current cell.
- **StaticBlocker:** a blocker cell is never `Empty` in the grid and never enters `occupied`/`critterAt`, so the slide cannot step into it — it stops one cell short, dividing the board.
- **ArrowRedirector:** when the critter *enters* an arrow cell, read `arrow.RedirectDirection` and switch `moveDir`/`delta` to it, producing an L-shaped path within a single tilt.
  - **Loop guard:** a `HashSet<Vector2Int> redirectedCells` records each arrow already used this resolve. If the critter would land on an arrow it has already used, it **stops on the tile** instead of redirecting again (prevents ping-pong between two arrows).
  - **Safety bound:** `maxSteps = (cols + rows) * 4` caps the walk regardless, a hard backstop against any pathological loop.

### Output

Each moving critter emits a `SlideMove`:
- non-exit: `from`, `to` (final cell), `exits = false`.
- exit: `from`, `exitFromCell` (last on-board cell before the gate), `exits = true`, `exitColor`, `exitDir`.
- no movement → no `SlideMove` is emitted.

`BoardController.AnimateMoves` restores an arrow's underlay state when a critter vacates an arrow cell (`if (_arrowAt.ContainsKey(m.from)) SetCell(m.from, ArrowRedirector)`), so arrows persist after a critter passes over them.

---

## 3. BoardController State

- **Logical model:** `_grid` is a `CellState[,]`; three dictionaries map cells to live objects — `_critterAt`, `_arrowAt`, `_blockerAt`. `_activeCritters` is the working list.
- **`_isAnimating` lock:** set true at the top of `TiltRoutine`, false only after win check. While true, `ExecuteTilt` and `UndoLastMove` both no-op. This is the single mechanism that serializes tilts and prevents mid-animation corruption.
- **Undo stacks:** two parallel stacks — `_undoStack` (cloned `CellState[,]`) and `_critterUndoStack` (copied `_critterAt`). `SaveStateForUndo` pushes before each tilt; `UndoLastMove` pops both, restores `_grid` and `_critterAt`, re-renders surviving critters to their stored cells, and **revives** any critter that had exited (`ResetVisual` + re-add to `_activeCritters`). The plumbing is multi-step capable; per the product doc, full one-step undo polish is deferred, but the implementation here is functional.

---

## 4. The Three Mechanic Types (and how the resolver handles each)

| Mechanic | Cell representation | Resolver handling |
|----------|--------------------|--------------------|
| **Slide + gate + critter blocking** (Tier 1) | `CellState.Critter`, gates in `GateController` | Critters walk until blocked by a settled critter or a wall; matching gate exits, non-matching gate stops. Destination-wall ordering makes the simultaneous slide deterministic. |
| **StaticBlocker** (Tier 2) | `CellState.StaticBlocker`, in `_blockerAt` | Never enters `occupied`; the slide walk simply can't step onto it, so the critter stops one cell short. Acts as an internal wall that routes paths. |
| **ArrowRedirector** (Tier 3) | `CellState.ArrowRedirector`, in `_arrowAt` | Underlays a cell (only marked if the cell is `Empty` at build). On entry, switches the critter's move direction to `arrow.RedirectDirection`; `redirectedCells` guards loops; underlay is restored when the critter leaves. Produces bent, single-tilt paths. |

A blocker and an arrow on the same cell can't coexist: `BuildBoard` only marks an arrow cell if it isn't already a blocker.

---

## 5. CritterPiece Lifecycle

```
spawn   →   idle   →   slide   →   exit
```

- **Spawn:** `LevelManager.LoadLevel` instantiates `critterPrefab`, calls `Init(id, color, cell)`, `ResetVisual()`, positions it via `board.CellToWorld(cell)`, and tints it by `CritterColor`. It's registered into `_critterAt` / `_activeCritters` and its cell marked `CellState.Critter` in `BuildBoard`.
- **Idle:** sits at its grid cell awaiting a tilt; carries `Color` and `GridPosition`.
- **Slide:** on a resolved non-exit move, `SlideTo(CellToWorld(to), worldSpeed)` animates it smoothly to the new cell (`worldSpeed = critterSlideCellsPerSecond * cellSize`, spec 9 cells/s). `SetGridPosition(to)` keeps logic and visuals in sync.
- **Exit:** `ExitRoutine` slides the critter to `exitFromCell` (last on-board cell), then through the gate via `ExitWorld(exitFromCell, exitDir)` (using the post-redirect direction), then `PlayExitAnimation()` (pop + sparkle + chime). It's removed from `_critterAt` and `_activeCritters`. Undo can revive an exited critter.

---

## 6. WinChecker: knowing all critters have exited

The win condition is simply **`_critterAt.Count == 0`** — when the last critter exits, its `SlideMove.exits` removes its entry, leaving the dictionary empty. `BoardController.CheckWin()` evaluates this at the end of each `TiltRoutine` and passes the result through `OnTiltResolved(won)`.

The "WinChecker" role is fulfilled by this chain:
```
BoardController.CheckWin()  →  OnTiltResolved(won)
   →  LevelManager.HandleTiltResolved(won)  →  OnLevelComplete (if won)
   →  meta listener  →  GameManager.ChangeState(LevelComplete) + SaveManager.MarkCompleted
```
There is exactly one authoritative win owner (`BoardController`'s check, surfaced via `LevelManager`); the earlier double-fire risk (QA M-1, dead static `NotifyWin()` bridge) is resolved by keeping a single path.

---

## 7. Editor Testing: EditorTiltInput

For development without a physical device, `EditorTiltInput` maps the keyboard **arrow keys** to `TiltDirection` and fires the **same `OnTilt` event** `TiltInputHandler` uses. Because `BoardController.ExecuteTilt` subscribes to that event regardless of source, the entire pipeline (lock, lean, resolve, animate, win) runs identically in the editor as on device. This makes the resolver and animation fully testable on desktop, and the pure-C# `SlideResolver` is additionally unit-testable in isolation with hand-built `CellState[,]` grids.
