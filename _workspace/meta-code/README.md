# Gridlock Critters — Meta Code

Unity C# meta systems (progression, save/load, UI). Targets `UnityEngine.UI` (no TextMeshPro).

## Files

| File | Purpose |
|------|---------|
| `Scripts/Meta/IGameplayBridge.cs` | Interfaces (`ILevelManager`, `IBoardController`) the gameplay-dev implements so meta can drive level loading and undo. |
| `Scripts/Meta/GameManager.cs` | Singleton game state machine (`GameState`); holds registered gameplay bridge refs; manages pause time-scale; fires `OnStateChanged`. |
| `Scripts/Meta/SaveManager.cs` | Singleton save/load to PlayerPrefs JSON (key `gridlock_save_v1`); `SaveData`; `MarkCompleted`, `HasCompleted`, `GetResumeLevel`. |
| `Scripts/Meta/LevelProgressManager.cs` | Singleton unlock/state logic (`LevelState`, `IsLevelUnlocked`, `GetLevelState`, `TotalLevels=30`, `OnProgressChanged`). |
| `Scripts/UI/LevelSelectController.cs` | Builds the 30-button grid, applies per-state visuals, routes taps to `ILevelManager.LoadLevel`. |
| `Scripts/UI/HUDController.cs` | In-level HUD: "Level N" label, Undo (calls `IBoardController.UndoLastMove`), Pause (→ `GameState.Paused`). |
| `Scripts/UI/LevelCompleteController.cs` | Level-complete overlay; marks completion on show; Next / Replay / Level Select. |
| `Scripts/UI/PauseMenuController.cs` | Pause overlay; Resume / Restart / Level Select / Main Menu. |
| `Save/save-schema.json` | Documentation of the persisted save schema. |

## Integration notes for gameplay-dev

- Implement `ILevelManager` (e.g. on your LevelManager) and assign it to `GameManager.Instance.LevelManager` at startup.
- Implement `IBoardController.UndoLastMove()` on your BoardController and assign it to `GameManager.Instance.BoardController` at each level load.
- Call `GameManager.Instance.ChangeState(GameState.LevelComplete)` when the last critter exits — the overlay records completion and unlocks the next level.
- On entering a level, call `SaveManager.Instance.SetLastPlayedLevel(index)`; accumulate `SaveManager.Instance.AddPlayTime(seconds)` while unpaused.

## Spec discrepancy (flagged)

`meta-spec.md` / `gdd.md` describe **25 levels, 1-based** indexing. This implementation follows the meta-dev task brief: **30 levels (`TotalLevels = 30`), 0-based** indexing (level 0 always unlocked; level N unlocks when N-1 completed). UI labels display `index + 1`. Adjust `LevelProgressManager.TotalLevels` to 25 and rebuild the grid if the spec is authoritative.
