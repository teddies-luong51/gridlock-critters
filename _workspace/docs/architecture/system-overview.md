# Gridlock Critters — System Overview

_Version: MVP 1.0 · Engine: Unity 3D (C#) · Date: 2026-06-10_

---

## Summary

Gridlock Critters is a tilt-driven spatial puzzle built in Unity 3D. The phone's accelerometer is the only control: tilting the device fires a discrete `TiltDirection`, which flows through a single deterministic pipeline — input is read and debounced, the board model leans ~15° for feedback, a pure-C# resolver computes where every critter slides simultaneously (honoring critter-critter blocking, static blockers, arrow redirectors, and color-matched gates), critters animate to their resolved cells or exit through their gates, and a win check fires when the board is empty. Gameplay logic lives in `gameplay-code`; the state machine, save system, and UI live in `meta-code`. The two halves are decoupled through thin interfaces (`ILevelManager`, `IBoardController`) held by a single canonical `GameManager`, so meta UI never references gameplay classes directly.

---

## Component Map

```
   PHYSICAL DEVICE
        │ Input.acceleration (low-pass, ~0.35 threshold, ~0.6s cooldown, re-arm)
        ▼
  ┌──────────────────┐    OnTilt(TiltDirection)
  │ TiltInputHandler │ ───────────────────────────┐
  └──────────────────┘                            │
   (EditorTiltInput: arrow keys, same OnTilt path) │
                                                   ▼
                                       ┌────────────────────┐
                                       │  BoardController    │  (IBoardController)
                                       │  ExecuteTilt(dir)   │
                                       │  _isAnimating lock  │
                                       │  undo stacks        │
                                       └─────────┬──────────┘
                              board lean ┌────────┘
                                         ▼
                              ┌────────────────────┐
                              │ BoardTiltAnimator  │ (visual ~15° lean)
                              └────────────────────┘
                                         │ ResolveMoves(dir, grid, critterAt)
                                         ▼
                              ┌────────────────────┐
                              │   SlideResolver    │ (pure C#, deterministic)
                              │  List<SlideMove>   │
                              └─────────┬──────────┘
                              animate    │
                                         ▼
                              ┌────────────────────┐
                              │   CritterPiece     │ (SlideTo / PlayExitAnimation)
                              └─────────┬──────────┘
                              win check  │  OnTiltResolved(won)
                                         ▼
                       ┌──────────────────────────────────┐
                       │  BoardController.CheckWin()        │
                       │  → LevelManager.OnLevelComplete    │  (WinChecker role)
                       └─────────────────┬─────────────────┘
                                         │ ChangeState(LevelComplete)
                                         ▼
                              ┌────────────────────┐
                              │    GameManager     │ (state machine, meta-code)
                              │  OnStateChanged    │
                              └─────────┬──────────┘
                                        │
                                        ▼
                       ┌──────────────────────────────────┐
                       │  UI LAYER (meta-code)             │
                       │  LevelSelect · HUD · PauseMenu    │
                       │  LevelCompleteController          │
                       └──────────────────────────────────┘
```

`TiltInput → BoardController → SlideResolver → WinChecker → GameManager → UI` is the load-bearing spine. Everything else (LevelManager, SaveManager, GateController) hangs off it.

---

## Scene Structure

```
Bootstrap  →  MainMenu  →  Game  →  Loading
```

- **Bootstrap** — first scene loaded. Instantiates the persistent singletons (`GameManager`, `SaveManager`) which all `DontDestroyOnLoad`. Reads the save, computes the resume target via `GetResumeLevel()`, and routes onward (MainMenu, or straight into Game when resuming).
- **MainMenu** — title + LevelSelect grid (30 buttons, 4 states). Tapping a level transitions to Game with that index.
- **Game** — the playable scene. Holds the board, critters, input handler, and in-scene HUD/PauseMenu. The bulk of `gameplay-code` lives here.
- **Loading** — lightweight transition scene between Game and MainMenu / next level, masking instantiation cost.

The singletons survive scene swaps; per-level objects are spawned and destroyed inside the Game scene by `LevelManager`.

---

## GameObject Hierarchy (Game Scene)

```
Game (scene)
├── --- Persistent (DontDestroyOnLoad, carried from Bootstrap) ---
│   ├── GameManager          (state machine + bridge refs)
│   └── SaveManager          (PlayerPrefs JSON persistence)
│
├── Systems
│   ├── LevelManager         (ILevelManager; LevelData[] catalog, prefabs, spawnRoot)
│   ├── TiltInputHandler     (accelerometer → OnTilt; or EditorTiltInput in editor)
│   └── GateController       (logical gate registry: cell, wall, color)
│
├── Board
│   ├── BoardController      (IBoardController; logical grid, tilt pipeline)
│   ├── BoardTiltAnimator    (visual ~15° lean)
│   ├── BoardOrigin          (Transform anchor for cell (0,0))
│   └── BoardMesh            (single-draw-call tray mesh)
│
├── SpawnRoot                (parent for all per-level instances)
│   ├── Critter_0 … Critter_N (CritterPiece, tinted by CritterColor)
│   ├── Blocker_*            (StaticBlocker)
│   ├── Arrow_*              (ArrowRedirector)
│   └── GateVisual_*         (optional cosmetic gates)
│
├── Camera                   (pos 0,12,-10 · rot 45,0,0 · FOV 50, fixed)
│
└── UI (Canvas)
    ├── HUD                  (Undo button → IBoardController.UndoLastMove, Retry, Pause)
    ├── PauseMenu
    └── LevelCompleteController (overlay; MarkCompleted, Next/Retry)
```

---

## How gameplay-code and meta-code Connect

The two assemblies never reference each other's concrete types. They meet at three small contracts held by the canonical `GameManager` (which lives in `meta-code`):

- **`GameManager` (meta-code)** is the single source of truth for `GameState` and holds two bridge slots: `ILevelManager LevelManager` and `IBoardController BoardController`.
- **`LevelManager` (gameplay-code)** implements `ILevelManager`. On `Awake` it self-registers: `GameManager.Instance.LevelManager = this`. Meta UI (LevelSelect, LevelComplete) drives levels through this interface — `LoadLevel(int)`, `ReloadCurrentLevel()`, `CurrentLevelIndex`, and the `OnLevelComplete` event — without knowing the concrete class.
- **`BoardController` (gameplay-code)** implements `IBoardController`. On `Awake` it self-registers: `GameManager.Instance.BoardController = this`. The HUD's Undo button calls `IBoardController.UndoLastMove()` through this slot.

A `GameplayBridge` glue object (in `gameplay-code/Scripts/Core`) wires the win signal across the boundary: `BoardController` raises `OnTiltResolved(won)`; `LevelManager` forwards a `true` as `OnLevelComplete`; meta-side listeners call `GameManager.ChangeState(LevelComplete)` and `SaveManager.MarkCompleted(...)`. The duplicate gameplay-side `GameManager` was collapsed into the meta one (QA C-1); the gameplay `GameManager.cs` is now an empty stub that documents the move.

This indirection means gameplay-code can be developed, swapped, or stubbed independently of the UI, and the UI compiles against interfaces only.

---

## Data Flow: One Tilt, End to End

```
1. User physically tilts the phone left/right/up/down.
2. Input.acceleration changes; TiltInputHandler low-pass filters it.
3. Magnitude crosses ~0.35 threshold, cooldown elapsed, device re-armed
   → TiltInputHandler fires OnTilt(TiltDirection).
4. BoardController.ExecuteTilt(dir):
        - if _isAnimating → ignore (input locked)
        - else SaveStateForUndo() (snapshot grid + critter map)
        - StartCoroutine(TiltRoutine).
5. TiltRoutine sets _isAnimating = true; starts BoardTiltAnimator lean (~15°);
   waits slideLeadDelay so the lean visibly leads the slide.
6. SlideResolver.ResolveMoves(dir, grid, critterAt):
        - sort critters destination-wall-first (CompareForOrder)
        - per critter: walk cells, stop at wall / blocker / settled critter,
          follow arrow redirects (loop-guarded), exit through a matching gate.
        - returns List<SlideMove>.
7. AnimateMoves: update logical grid first (consistent state), then launch
   all CritterPiece.SlideTo / ExitRoutine coroutines simultaneously.
8. Each critter slides smoothly; exiting critters slide to the wall cell,
   through the gate, then PlayExitAnimation (pop + sparkle + chime).
9. Lean returns to neutral; CheckWin() = (_critterAt.Count == 0).
10. _isAnimating = false; OnTiltResolved(won) fires.
11. If won: LevelManager.OnLevelComplete → GameManager.ChangeState(LevelComplete)
    + SaveManager.MarkCompleted.
12. OnStateChanged broadcasts; UI layer responds (LevelComplete overlay shown).
```

Every step is deterministic and re-entrancy-safe: the `_isAnimating` lock guarantees one tilt resolves fully before the next is accepted.

---

## Mobile Performance Targets

- **60 fps** — `Application.targetFrameRate = 60` set in `GameManager.Awake`.
- **No `GetComponent` in `Update`** — component refs are cached/serialized; the resolver is pure C# with no `MonoBehaviour` lookups in the hot path. (One known offender: `LevelManager.TintRenderer` uses `GetComponentInChildren` at spawn time only, not per-frame — acceptable.)
- **Object pool for critters** — target: a reusable pool sized **≥6** (the hardest levels' critter count). Currently `LevelManager` uses `Instantiate`/`Destroy` per level; the pool is deferred (QA L-4/Perf, non-blocking for correctness).
- **Single draw call board mesh** — the tray renders as one mesh; only the board model leans, the camera is fixed (no per-frame camera math).
- **Allocation discipline** — slide resolution allocates small per-tilt lists/sets (`redirectedCells`, `moves`); acceptable at puzzle cadence, candidate for pooling if profiling flags GC spikes on device.
