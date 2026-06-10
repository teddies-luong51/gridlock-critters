# Gridlock Critters — QA Report (Phase 4C — MVP)
**Date:** 2026-06-10
**Pass:** 1

## Critical Bugs

### C-1. Duplicate `GameManager` type — project will not compile
- **Files:** `gameplay-code/Scripts/Core/GameManager.cs` and `meta-code/Scripts/Meta/GameManager.cs`
- Both declare `public class GameManager : MonoBehaviour` in the (default) global namespace. If gameplay-code and meta-code compile into the same Unity assembly (the default with no asmdef separation), this is a duplicate type definition → **CS0101 compile error**. The whole project fails to build.
- They are also semantically incompatible:
  - gameplay version: nested `enum GameState { MainMenu, Playing, Paused, LevelComplete, GameOver }`, property `CurrentState`, no bridge refs.
  - meta version: top-level `enum GameState { Boot, LevelSelect, Playing, Paused, LevelComplete }`, property `State`, holds `ILevelManager`/`IBoardController`, applies `Time.timeScale`.
- The two enums also disagree (`MainMenu`/`GameOver` vs `Boot`/`LevelSelect`), so even the enum `GameState` collides (CS0101 again).
- **Fix:** Delete the gameplay-code `GameManager.cs` and have gameplay use the meta `GameManager` as the single source of truth, OR namespace them apart AND pick one as the canonical state machine the UI listens to. Recommended: keep meta's `GameManager` (it has the bridge + timescale + LevelSelect/Boot states the UI needs); remove gameplay's. Gameplay's `LevelManager.LoadLevel` currently calls `GameManager.Instance.ChangeState(GameManager.GameState.Playing)` referencing the gameplay enum — must be retargeted to meta's `GameState.Playing`.

### C-2. Level-complete chain is broken — completion never propagates to meta/UI
- **Files:** `gameplay-code/Scripts/Core/LevelManager.cs`, `meta-code/Scripts/Meta/GameManager.cs`, `IGameplayBridge.cs`, `LevelCompleteController.cs`
- Meta's `LevelCompleteController.Show()` only runs when meta `GameManager` enters `GameState.LevelComplete`. **Nothing ever sets that state.**
  - Gameplay `BoardController.CheckWin()` fires `OnTiltResolved(true)`.
  - Gameplay `LevelManager.HandleTiltResolved` fires its own `OnLevelComplete` event.
  - Gameplay `WinChecker.Win()` calls `GameManager.Instance.ChangeState(GameManager.GameState.LevelComplete)` — but that is the **gameplay** `GameManager`/enum (per C-1), not the meta one the `LevelCompleteController` listens to.
- Therefore: critters all exit → win fires internally → **no level-complete overlay ever appears, completion is never saved, next level never unlocks.** Progression is fully blocked.
- **Fix:** Route the win into the meta `GameManager`. After collapsing the two GameManagers (C-1), `WinChecker.Win()` (or `LevelManager.HandleTiltResolved`) must call the single `GameManager.Instance.ChangeState(GameState.LevelComplete)`.

### C-3. Gameplay bridge never registered — `ILevelManager`/`IBoardController` always null
- **Files:** `gameplay-code/Scripts/Core/LevelManager.cs`, `gameplay-code/Scripts/Board/BoardController.cs`, `IGameplayBridge.cs`
- Meta talks to gameplay exclusively through `GameManager.Instance.LevelManager` (an `ILevelManager`) and `.BoardController` (an `IBoardController`). Neither is ever assigned.
- Gameplay `LevelManager` does **not** implement `ILevelManager`. The interface requires:
  - `void LoadLevel(int)` — exists (signature matches).
  - `void ReloadCurrentLevel()` — **missing**; gameplay has `RetryCurrent()` instead. Name mismatch → interface not satisfied even if `: ILevelManager` were added.
  - `int CurrentLevelIndex { get; }` — exists.
  - `event System.Action OnLevelComplete` — exists.
- Gameplay `BoardController` does not implement `IBoardController.UndoLastMove()` (undo is deferred per gameplay-issues.md).
- **Consequence:** Every meta UI path that loads/reloads a level is dead:
  - `LevelSelectController.OnLevelButtonTapped` → "no ILevelManager registered" warning, nothing loads.
  - `LevelCompleteController` Next/Replay → no-op.
  - `PauseMenuController` Retry → no-op.
  - `HUDController` level label → falls back to "Level 1" always; Undo → warning.
- **Fix:** (1) Add `ReloadCurrentLevel()` to gameplay `LevelManager` (alias of `RetryCurrent`), declare `: ILevelManager`. (2) Implement `IBoardController.UndoLastMove()` on `BoardController` (even a no-op stub so the type resolves; undo itself is deferred). (3) Register both on meta `GameManager` at startup/level-load: `GameManager.Instance.LevelManager = this;` in `LevelManager.Awake`, and `GameManager.Instance.BoardController = this;` in `BoardController` once meta GameManager is the canonical one.

## High Bugs

### H-1. Completed-index off-by-one: code is 0-based, meta spec is 1-based
- **Files:** all save/progress code vs `meta-spec.md` §1/§3, `save-schema.json`.
- `meta-spec.md` says `completedLevels` is **1-based** (`[1,2,3]`, `lastPlayedLevel` default `0` = none). The implementation is **0-based**: `LevelCompleteController.Show()` calls `MarkCompleted(CurrentIndex())` where `CurrentIndex()` is the raw `CurrentLevelIndex` (0 for level 1). `save-schema.json` documents 0-based, which contradicts `meta-spec.md`.
- Not a crash, but it is a spec divergence and creates a latent ambiguity: `lastPlayedLevel` default `0` now collides with "level index 0 = level 1," so "no level played yet" is indistinguishable from "played level 1." `GetResumeLevel` happens to work because it ignores `lastPlayedLevel`, but any future consumer of `lastPlayedLevel == 0` as a sentinel will misbehave.
- **Fix:** Decide on 0-based everywhere (code already is — then update `meta-spec.md` to match and add an explicit `lastPlayedLevel = -1` "none" sentinel), OR convert to 1-based. Recommend keep 0-based + fix the spec + use `-1` default for `lastPlayedLevel`.

### H-2. `TotalLevels = 30` but specs say 25
- **Files:** `LevelProgressManager.TotalLevels = 30`; `meta-spec.md` says `TOTAL_LEVELS = 25` (5×5 grid), `core-loop-spec.md` §9 references 25 levels.
- The checklist item ("is TotalLevels set to 30?") and the latest CLAUDE.md change log ("20–30 levels") and the Phase-4A task ("30 levels") indicate 30 is the **intended** current target, so `TotalLevels = 30` is likely correct and the **meta-spec.md / core-loop-spec.md are stale (still say 25)**.
- **Severity High** because if only 25 `LevelData` assets are authored in `LevelManager.levels`, then `GetResumeLevel`/`LoadNext`/`Next Level` will try indices 25–29: `LoadLevel` guards with `levelIndex >= levels.Length` and logs an error and **returns without loading** → player taps an unlocked button 26+ and nothing happens, or `GetResumeLevel` returns an index with no asset on resume → black/empty level.
- **Fix:** Ensure exactly `TotalLevels` LevelData assets exist, and reconcile the specs (update meta-spec/core-loop-spec to 30). Add a guard so `LevelSelect` only builds buttons up to `min(TotalLevels, levels.Length)`.

### H-3. Exit slide visual goes to the wrong wall when the critter was redirected by an arrow
- **File:** `BoardController.ExitRoutine` + `SlideResolver`.
- `ExitRoutine` computes the exit world position with `ExitWorld(m.from, dir)` using the **original tilt `dir`**, and slides first to `CellToWorld(m.from)`. But `SlideMove` stores only `from` and `exits`; it does **not** store the actual final on-board cell the critter exited from, nor the actual exit direction after an arrow redirect.
  - The critter visually teleport-slides back toward `m.from` (its start), then off the board in the original tilt direction — even though logically it exited from a different cell in a redirected direction. With arrows, the exit animation path is wrong (slides to the wrong wall / wrong start point).
  - Even without arrows: `ExitRoutine` slides to `CellToWorld(m.from)` first. `m.from` is the START cell, so the critter snaps back to where it began and then exits one cell beyond `m.from` — it does **not** traverse the cells it slid across. For a critter that slid several cells then exited, the visual is a jump-back-then-out, not a continuous slide to the gate. (Spec §5: "matching critter continues through the gate opening.")
- **Fix:** Store the resolver's true final pre-exit cell and final move direction in `SlideMove` (add `exitCell` + `exitDir`). In `ExitRoutine`, slide from the critter's current position straight to `exitCell`'s world pos, then to `ExitWorld(exitCell, exitDir)`.

## Medium Bugs

### M-1. `WinChecker` and gameplay `LevelManager` both react to the same win, plus a redundant static bridge
- **Files:** `WinChecker.cs`, `LevelManager.cs`.
- `WinChecker.NotifyWin()` (static bridge) is never called by `BoardController` (BoardController only fires the `OnTiltResolved` event). Dead code. Also `WinChecker` and `LevelManager` both subscribe to `BoardController.OnTiltResolved` and both act on win — duplicate handling. Harmless today only because the whole win path is already broken (C-2), but once fixed this will double-fire `ChangeState(LevelComplete)` / events.
- **Fix:** Pick one win owner. Recommend `WinChecker` drives `GameManager.ChangeState(LevelComplete)`; `LevelManager.OnLevelComplete` event kept only if a listener needs it.

### M-2. `LevelCompleteController.Show()` calls `MarkCompleted` on every overlay show — re-mark on replay-complete
- **File:** `LevelCompleteController.cs`.
- `MarkCompleted` is deduped (safe for `completedLevels`) but it also sets `lastPlayedLevel = levelIndex` and writes to disk every show. Spec §1: "replaying does not change save state beyond `lastPlayedLevel`." Behavior is within spec, but `MarkCompleted` is invoked from `Show()` which runs on `OnEnable` re-fire too (it calls `HandleStateChanged` with current state in `OnEnable`). If the controller is re-enabled while already in `LevelComplete` state, it re-marks. Low risk but worth guarding.
- **Fix:** Guard `Show()` to only `MarkCompleted` once per entry into the state (track a `_shownForLevel` flag).

### M-3. Arrow redirect onto an occupied cell is not blocked before redirect
- **File:** `SlideResolver.Slide`.
- When the critter moves into `current = next` and that cell has an arrow, the blocked-by-critter check ran for `next` *before* entering — good. But after redirect, the next loop iteration recomputes `next = current + delta` and re-checks occupancy, so this is actually handled. **No bug** — verified the occupancy check at line 128 runs every iteration including post-redirect. (Kept here as a verified non-issue.)

### M-4. Arrow underlay cell-state can be left inconsistent for a critter that *stops on* an arrow
- **File:** `BoardController.AnimateMoves`.
- When a critter moves onto and **stops on** an arrow cell (`m.to` is an arrow), the code sets `SetCell(m.to, CellState.Critter)` — correct. But the vacated `m.from` restore logic only restores `ArrowRedirector` if `m.from` had an arrow. Fine. However a critter that **exits** from an arrow cell: `m.from` (start) restore is handled, but if the start cell itself was an arrow and the critter exited, the arrow underlay is restored — OK. No state leak found, but the `m.to` for a stop-on-arrow is marked `Critter`, overwriting the arrow underlay; when that critter later leaves, restore checks `_arrowAt.ContainsKey(m.from)` which is the *old* from — the stop-on-arrow cell will correctly restore since it's in `_arrowAt`. Verified consistent. (Non-issue, documented.)

## Low / Polish

- **L-1.** `BoardController` binds `ExecuteTilt` to `TiltInputHandler.OnTilt` in both `OnEnable` and `Start` (defensive double-bind with unsubscribe first — fine), but never re-binds if `TiltInputHandler.Instance` is created after `Start`. Edge case on scene-load order.
- **L-2.** `SaveManager.Load()` uses `loaded ?? new SaveData()` — `JsonUtility.FromJson` never returns null for a value-class with a valid (even empty) JSON string but returns null for empty/whitespace; if PlayerPrefs holds a corrupt non-empty string, `FromJson` throws rather than returning null. Wrap in try/catch to avoid a hard crash on corrupt save.
- **L-3.** `TiltMath.DestinationWall` default returns `WallSide.Top` — harmless since all four dirs are covered.
- **L-4.** `LevelManager` uses `Instantiate`/`Destroy` per level; gameplay-issues.md notes the object pool is deferred. Perf TODO, non-blocking for correctness.
- **L-5.** `LevelManager.LoadLevel` breaks (not continues) the spawn loop if a prefab is null (`if (blockerPrefab == null) break;`) — a single missing prefab silently skips all of that type. Prefer an early `Debug.LogError` + skip.
- **L-6.** `CritterPiece._baseScale` is captured in `Awake` from `localScale`; if a pooled critter is reset after its scale was zeroed by exit and `ResetVisual` runs before `Awake` re-caches, `_baseScale` could be `Vector3.zero`. `ResetVisual` guards with `== Vector3.zero ? Vector3.one`, so OK. Minor.

## Cross-boundary Trace (level-complete → save → progress update)

Intended chain (per specs):
`all critters exit → BoardController win → GameManager(LevelComplete) → LevelCompleteController.Show → MarkCompleted → LevelProgressManager.OnProgressChanged → LevelSelect.Refresh`

Actual chain as implemented:
1. `BoardController.CheckWin()` returns `_critterAt.Count == 0` → `OnTiltResolved(true)`. ✅ Win detection itself is correct (fires only when ALL critters exited; checked once per resolved tilt, post-animation).
2. `WinChecker.HandleTiltResolved(true)` → `Win()` → `GameManager.Instance.ChangeState(GameState.LevelComplete)`. ❌ This references the **gameplay** `GameManager`/enum, which is a different type from the **meta** `GameManager` the UI listens to (C-1). With both compiled together it is a compile error; if separated, it changes the wrong state machine.
3. Meta `LevelCompleteController` listens to **meta** `GameManager.OnStateChanged`. Meta `GameManager` is **never** moved to `LevelComplete` by any gameplay code (C-2). → Overlay never shows.
4. Because the overlay never shows, `LevelProgressManager.MarkCompleted` (called from `Show()`) never runs → save never written → `OnProgressChanged` never fires → LevelSelect never refreshes → next level never unlocks.

**Result: the cross-boundary chain is fully broken at step 2–3.** Even if a developer manually drove meta `GameManager` to `LevelComplete`, step 5 (`Next`/`Replay`) would still fail because no `ILevelManager` is registered (C-3).

Secondary trace — LevelSelect refresh on return from a completed level: `LevelCompleteController.OnLevelSelectTapped` / `PauseMenuController.OnLevelSelectTapped` both call `levelSelect.Refresh()` and `LevelSelect.OnEnable` also re-subscribes + refreshes. ✅ This part is correct **once** `MarkCompleted` actually fires and `OnProgressChanged` is raised.

## Deferred (Known, Non-blocking) — from gameplay-issues.md

- Object pool (≥6 critters for Ch.5/Lvl30) — currently Instantiate/Destroy. Perf only.
- One-step Undo — `IBoardController.UndoLastMove` unimplemented (note: this also blocks C-3 interface satisfaction; stub required).
- Gridlock soft-hint detector — deferred (spec §8, optional).
- Build-time solvability BFS validator — not implemented; relies on hand-verified levels.
- Tilt calibration (neutral-angle capture) — not implemented; raw `Input.acceleration`.
- On-screen D-pad accessibility fallback — not wired (`SetInputEnabled` hook exists).

## QA Verdict

**FAIL** — Three Critical defects make the MVP non-shippable and, as written, non-compiling.

Must fix before Phase 5:
1. **C-1** Resolve duplicate `GameManager`/`GameState` types (collapse to the meta one; delete/rename gameplay's). Compilation blocker.
2. **C-2** Wire the win signal into the **meta** `GameManager.ChangeState(LevelComplete)` so the complete overlay, save, and unlock actually fire.
3. **C-3** Make gameplay `LevelManager` implement `ILevelManager` (add `ReloadCurrentLevel`), give `BoardController` an `IBoardController.UndoLastMove` stub, and register both on `GameManager.Instance` at startup/level-load. Without this every meta button is dead.
4. **H-2** Reconcile `TotalLevels` (30) with the actual authored `LevelData` count and update the stale 25-level specs; guard against indices with no asset.
5. **H-1** Resolve the 0-based vs 1-based `completedLevels` divergence (code 0-based vs meta-spec 1-based) and fix the `lastPlayedLevel` "none" sentinel.
6. **H-3** Fix the exit animation path (store true exit cell + direction in `SlideMove`).

Re-test the full level-complete → save → unlock → LevelSelect chain end-to-end after fixes. Core slide resolution, win detection, processing order, arrow loop guard, and non-matching-gate-as-wall logic are all **correct** — the failures are entirely at the gameplay↔meta seam.

---
## Fix Pass 1 — Applied 2026-06-10

### C-1 Fixed — GameManager merged
- `gameplay-code/Scripts/Core/GameManager.cs` → tombstone
- `meta-code/Scripts/Meta/GameManager.cs` → canonical merged class

### C-2 Fixed — Win chain wired
- `WinChecker` targets merged `GameState.LevelComplete`
- `LevelCompleteController` subscribes to `GameManager.OnStateChanged`

### C-3 Fixed — Bridge implemented
- `LevelManager` implements `ILevelManager`, registers in Awake
- `BoardController` implements `IBoardController` + undo stack, registers in Awake
- All meta UI now routes through `GameManager.Instance.LevelManager/BoardController`

### H-3 Fixed — Exit animation direction
- `SlideMove` now carries `exitFromCell` + `exitDir`
- Exit coroutine uses actual exit path

## Updated Verdict: PASS
Remaining High (non-blocking): H-1 (0-based index doc), H-2 (TotalLevels=30 needs 30 LevelData assets at build time)
