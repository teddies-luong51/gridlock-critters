# Gridlock Critters — Data Architecture

_Version: MVP 1.0 · Scope: level data, save schema, state, interfaces · Date: 2026-06-10_

---

## 1. LevelData ScriptableObject

Each level is authored as a `LevelData` ScriptableObject asset, consumed by `LevelManager` to spawn the board. One asset per level; the catalog is the serialized `LevelData[] levels` array on `LevelManager`.

| Field | Type | Meaning |
|-------|------|---------|
| `gridWidth` | `int` | Number of columns (cells along x). |
| `gridHeight` | `int` | Number of rows (cells along y; +y goes down toward the near wall). |
| `critters[]` | array | Each entry: `{ CritterColor color; Vector2Int cell; }` — a critter's color and start cell. |
| `gates[]` | array | Each entry: `{ Vector2Int cell; WallSide wall; CritterColor color; }` — a colored exit on a wall side; only a matching critter may pass. |
| `blockers[]` | array | Each entry: `{ Vector2Int cell; }` — an immovable stone tile. |
| `arrows[]` | array | Each entry: `{ Vector2Int cell; TiltDirection direction; }` — a redirector that turns a passing critter to `direction`. |

`LevelManager.LoadLevel(index)` reads these arrays in order — blockers, arrows, gates (registered into `GateController`, optional cosmetic visual), then critters — instantiating from the corresponding prefabs, positioning via `BoardController.CellToWorld`, and tinting critters/gates by `CritterColor`. It then calls `board.BuildBoard(gridWidth, gridHeight, critters, arrows, blockers, gateController)` to assemble the logical model.

**Coordinate convention:** top-left = `(0,0)`, `+x` = right, `+y` = down (`TiltMath.Delta`). Authoring tools must follow this.

---

## 2. Save Schema

Persisted by `SaveManager` as a single JSON blob in `PlayerPrefs`.

- **Key:** `"gridlock_save_v1"`
- **Container:** `[System.Serializable] class SaveData`, written with `JsonUtility.ToJson`.

| Field | Type | Default | Meaning |
|-------|------|---------|---------|
| `completedLevels` | `int[]` | `new int[0]` | Set of completed level indices (0-based, deduped). The authoritative source of progression. |
| `lastPlayedLevel` | `int` | `0` | The last level entered/completed. Informational; **not** used for resume. |
| `totalPlayTime` | `float` | `0f` | Cumulative gameplay seconds, accrued via `AddPlayTime`. |

**Write triggers:** `MarkCompleted` (on level complete), `SetLastPlayedLevel` (on entering a level), `AddPlayTime` — each calls `Save()` (`PlayerPrefs.SetString` + `PlayerPrefs.Save`). Per the product doc, writes also fire on pause→LevelSelect/quit and app pause/quit.

**Load:** `Load()` runs in `Awake`. If the key is absent it returns a fresh `SaveData`; otherwise it deserializes (falling back to a default if `FromJson` yields null). Known gap (QA L-2): a corrupt non-empty string throws — should be wrapped in try/catch post-MVP.

---

## 3. Resume Logic

`GetResumeLevel()` returns the **lowest level index not present in `completedLevels`** — *not* `lastPlayedLevel` (which may point at an already-cleared replay):

```
for i in 0 .. TotalLevels-1:
    if not HasCompleted(i): return i
return TotalLevels - 1   // all complete → clamp to last
```

`HasCompleted(i)` is `completedLevels.Contains(i)`. On app open, Bootstrap calls this to decide where to drop the player. If every level is complete, the product spec routes to LevelSelect instead of replaying the last level.

> Note (QA H-1): indices are 0-based in code but 1-based in the meta spec, and the `lastPlayedLevel` default of `0` collides with "level 1" as a sentinel. The recommended fix keeps 0-based and uses `-1` as the "none" sentinel.

---

## 4. GameState Enum — States & Transitions

`GameState` (7 values) is owned by the single `GameManager`. `ChangeState` sets the state, applies pause time-scale (`Time.timeScale = Paused ? 0 : 1`), and broadcasts `OnStateChanged`.

| State | Meaning |
|-------|---------|
| `Boot` | Initial state; singletons spin up, save loads, resume target computed. |
| `MainMenu` | Title screen. |
| `LevelSelect` | 30-button grid (Locked / Unlocked / Current / Completed). |
| `Playing` | Active gameplay; tilt input live. |
| `Paused` | Time frozen (`timeScale = 0`); pause menu shown. |
| `LevelComplete` | Board cleared; completion overlay shown, save marked. |
| `GameOver` | Reserved (no fail state in this puzzle MVP). |

```
Boot ──► MainMenu ──► LevelSelect ──► Playing ◄──► Paused
                          ▲              │
                          │              ▼
                          └──────── LevelComplete ──► (Next) Playing
                                            └──────► LevelSelect
```

`LevelManager.LoadLevel` transitions to `Playing`; the win chain transitions to `LevelComplete`; the pause button toggles `Playing`↔`Paused`. `GameOver` exists in the enum but is unused in MVP.

---

## 5. ILevelManager / IBoardController Interfaces

These two interfaces are the only contract between `meta-code` and `gameplay-code`. They keep the UI compiling against abstractions so gameplay internals can change freely.

**`ILevelManager`** (implemented by gameplay `LevelManager`):

| Member | Why it exists |
|--------|---------------|
| `void LoadLevel(int levelIndex)` | LevelSelect / resume launches a specific level. |
| `void ReloadCurrentLevel()` | Retry — rebuilds the current level in place. |
| `int CurrentLevelIndex { get; }` | Meta needs the active index for save/next/UI. |
| `event Action OnLevelComplete` | The win signal meta subscribes to (→ `ChangeState`, `MarkCompleted`). |

**`IBoardController`** (implemented by gameplay `BoardController`):

| Member | Why it exists |
|--------|---------------|
| `void UndoLastMove()` | The HUD Undo button reverts the last tilt without touching gameplay internals. |

Both implementors **self-register** on `GameManager.Instance` in `Awake` (`LevelManager` → `.LevelManager`, `BoardController` → `.BoardController`), so meta code reaches gameplay only through the `GameManager` singleton and these interfaces — never a concrete gameplay type.

---

## 6. ScriptableObject-Driven Level Authoring Workflow

1. **Create asset** — `Create → Gridlock → LevelData`, producing a new `LevelData` `.asset`.
2. **Set grid** — `gridWidth` / `gridHeight`.
3. **Place elements** — fill `critters[]`, `gates[]`, `blockers[]`, `arrows[]` by cell using the top-left-origin, +y-down convention. Tier dictates which arrays are used (Tier 1: critters + gates; Tier 2: + blockers; Tier 3: + arrows).
4. **Register in catalog** — add the asset to `LevelManager.levels[]` at its index (index = level − 1, 0-based).
5. **Test in editor** — enter Play, use `EditorTiltInput` arrow keys to verify solvability and difficulty; the full pipeline runs identically to device.
6. **Iterate** — tweak the asset and re-test; no code changes, no recompile — data-only level design.

**Authoring status (QA H-2):** `TotalLevels = 30` but assets for levels 26–30 still need authoring, and older specs reference 25; LevelSelect should clamp to `min(TotalLevels, levels.Length)` until all 30 exist. A build-time BFS solvability validator is not implemented — levels are hand-verified (deferred post-MVP). A missing prefab currently breaks a spawn loop silently (QA L-5) — should log and skip.
