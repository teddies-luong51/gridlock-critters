# GameScene — Setup Instructions

Scene that runs the core loop: board + critters + tilt input + win handling.

## 1. Camera

| Property | Value |
|----------|-------|
| Position | (0, 12, -10) |
| Rotation | (45, 0, 0) |
| Projection | Perspective |
| Field of View | 50 |
| Near clip | 0.3 |
| Far clip | 100 |
| Tag | MainCamera |

The board sits at world origin. The 45° downward angle gives the chessboard-from-across-the-table look. Camera is FIXED — it never rotates with the tilt; only the board mesh leans.

## 2. GameObject Hierarchy

```
GameScene
├── [GameManager]            -> GameManager.cs, WinChecker.cs
├── [BoardRoot]              -> BoardController.cs, BoardTiltAnimator.cs, GateController.cs
│   │                           (boardTransform on BoardTiltAnimator = BoardMesh)
│   ├── BoardMesh            -> the visible wooden tray mesh (this is what leans)
│   ├── BoardOrigin          -> empty Transform at cell (0,0) world anchor
│   └── SpawnRoot            -> empty; parent for runtime critters/blockers/arrows/gates
├── [LevelManager]           -> LevelManager.cs
├── [InputHandlers]
│   ├── TiltInput            -> TiltInputHandler.cs
│   └── EditorTiltInput      -> EditorTiltInput.cs   (Editor-only, harmless in build)
└── [UI_HUD]                 -> Canvas (Undo / Retry / Level label) — meta-dev owns
```

## 3. Script Assignment & Wiring

| GameObject | Script | Inspector wiring |
|------------|--------|------------------|
| [GameManager] | GameManager | — |
| [GameManager] | WinChecker | binds to BoardController.OnTiltResolved at runtime |
| BoardRoot | BoardTiltAnimator | `boardTransform` = BoardMesh; tiltAngle 15, tiltDuration 0.15, holdDuration 0.05, returnDuration 0.2 |
| BoardRoot | GateController | — |
| BoardRoot | BoardController | `tiltAnimator` = BoardTiltAnimator, `gateController` = GateController, `boardOrigin` = BoardOrigin, `cellSize` = 1, `critterSlideCellsPerSecond` = 9, `slideLeadDelay` = 0.05 |
| [LevelManager] | LevelManager | `levels` = LevelData[] array; `critterPrefab`, `blockerPrefab`, `arrowPrefab`, optional `gateVisualPrefab`; `board` = BoardController; `gateController` = GateController; `spawnRoot` = SpawnRoot; `colorTints` = 6 colors indexed by CritterColor |
| TiltInput | TiltInputHandler | tiltThreshold 0.35, smoothing 0.1, cooldown 0.6 |
| EditorTiltInput | EditorTiltInput | — |

## 4. Boot Order

1. GameManager.Awake sets `Application.targetFrameRate = 60` and singleton.
2. TiltInputHandler.Awake sets singleton.
3. BoardController.OnEnable/Start subscribes to TiltInputHandler.OnTilt (double-guarded so order doesn't matter).
4. A bootstrap caller (or LevelManager.Start) calls `LevelManager.Instance.LoadLevel(index)`.

## 5. Layers

| Layer | Used by |
|-------|---------|
| Default | board mesh, gates |
| Critters | critter prefabs (for selective raycast/VFX if needed) |
| UI | HUD canvas |

Physics is not used for resolution (pure logical grid), so the collision matrix can disable all pairs except any you add for cosmetic touch effects.

## 6. Coordinate Convention

- Grid (0,0) = top-left, +x = right, +y = down (row 1 = y0), per core-loop-spec §1.
- World mapping (BoardController.CellToWorld): worldX = originX + x*cellSize; worldZ = originZ − y*cellSize; worldY = originY.
- So grid Down (toward player / near wall) = world −z, matching the 45° camera.
