# Gridlock Critters — Gameplay Code

Unity C# implementation of the core tilt-slide-jam loop. Flat namespace (no `namespace` blocks).
Implements core-loop-spec.md against gdd.md. Drop `Scripts/` under `Assets/_Game/`.

## Files

### Scripts/Core
- `GameTypes.cs` — Shared enums (TiltDirection, CritterColor, CellState, WallSide) + TiltMath delta/wall helpers.
- `GameManager.cs` — Singleton game state machine (MainMenu/Playing/Paused/LevelComplete/GameOver) with OnStateChanged event; sets 60fps target.
- `LevelManager.cs` — Loads LevelData[] and instantiates critters/blockers/arrows/gates; OnLevelComplete event; Retry/Next helpers.
- `WinChecker.cs` — Listens to BoardController.OnTiltResolved, fires OnWin and drives GameManager to LevelComplete.

### Scripts/Input
- `TiltInputHandler.cs` — Singleton accelerometer reader; low-pass filter (smoothing 0.1), threshold 0.35, cooldown 0.6 + re-arm; fires OnTilt(TiltDirection).
- `EditorTiltInput.cs` — Editor-only arrow-key fallback calling BoardController.Instance.ExecuteTilt.

### Scripts/Board
- `BoardController.cs` — Singleton pipeline: board lean -> resolve moves -> simultaneous slide/exit animation -> win check; owns the logical grid + cell/world mapping; blocks input via _isAnimating.
- `SlideResolver.cs` — Pure deterministic resolver; ResolveMoves(dir, grid, critterAt) -> List<SlideMove>; processes from destination wall inward; handles blockers, critter-stop, matching-gate exit, arrow redirects with loop guard.
- `BoardTiltAnimator.cs` — PlayTilt coroutine leaning the board mesh 15° (SmoothStep lerp); tiltDuration 0.15 / hold 0.05 / return 0.2.
- `CritterPiece.cs` — Per-critter MonoBehaviour; CritterColor; SlideTo (OutCubic + elastic settle) and PlayExitAnimation (scale up then vanish); tracks Vector2Int grid position.
- `GateController.cs` — Gate registry keyed by (cell, wall); GetGateAt + CanExit; non-matching color treated as solid.
- `StaticBlocker.cs` — Marks a cell impassable; CellType = StaticBlocker.
- `ArrowRedirector.cs` — Arrow tile with redirectDirection; CellType = ArrowRedirector.

### Scripts/Data
- `LevelData.cs` — ScriptableObject: levelIndex, gridWidth/Height, CritterSpawn[]/GateData[]/BlockerData[]/ArrowData[].

### Scenes
- `GameScene-Setup.md` — Camera values, GameObject hierarchy, script wiring, layers, coordinate convention.

### Prefabs
- `Prefabs-Setup.md` — Critter / StaticBlocker / ArrowRedirector / gate prefab setup; pooling note (>=6).

## Related
- `../gameplay-issues.md` — TODOs, edge cases, and QA verification targets.
