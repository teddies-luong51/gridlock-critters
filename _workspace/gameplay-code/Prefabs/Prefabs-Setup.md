# Prefab Setup Instructions

## Critter.prefab
- Root GameObject: `Critter`
- Component: `CritterPiece.cs`
- Child mesh: flat-shaded critter model (placeholder capsule/cube for MVP)
- A `Renderer` on root or child so LevelManager.TintRenderer can color it per CritterColor
- Layer: `Critters`
- Inspector defaults: overshootFraction 0.04, settleDuration 0.08, exitScaleUp 1.25, exitDuration 0.25
- Pooling note: object pool must hold >= 6 (Level 30 / hardest levels have 6 critters)

## StaticBlocker.prefab
- Root GameObject: `StaticBlocker`
- Component: `StaticBlocker.cs`
- Child mesh: solid crate/rock placeholder
- No tinting needed

## ArrowRedirector.prefab
- Root GameObject: `ArrowRedirector`
- Component: `ArrowRedirector.cs`
- Child mesh: flat arrow tile that sits UNDER critters (slightly below cell surface so a critter can rest on it)
- `redirectDirection` set per-instance by LevelManager.Configure from LevelData

## Gate visual (optional gateVisualPrefab)
- Cosmetic colored opening placed in the tray wall
- Tinted by LevelManager per gate color
- Logical gate behavior lives entirely in GateController (data), not the prefab

## BoardRoot / BoardMesh (scene, not prefab, but layout note)
- BoardMesh is the only thing that leans (assigned as BoardTiltAnimator.boardTransform)
- BoardOrigin empty marks world position of cell (0,0)
- Walls are children of BoardMesh so they lean together
