# Gridlock Critters — Meta Spec (MVP)

*Implementation spec for meta-dev. Source: `gdd.md` §7. Date: 2026-06-09.*
*Scope: navigation + save/resume only. 25 levels, no economy.*

---

## 0. Screen Map & Transitions

```
[Boot] → resume logic → [LevelSelect]  ⇄  [GameplayLevel N]  ⇄  [PauseMenu (overlay)]
                              ▲                    │
                              │                    ▼
                              └──────────── [LevelComplete (overlay)]
```

- **Boot:** load save → run Resume Flow (§6) → push either LevelSelect or directly into a GameplayLevel.
- **LevelSelect:** grid of 25 level buttons. Tap unlocked/completed button → load that GameplayLevel.
- **GameplayLevel N:** the core-loop scene. Has a Pause button (top-right).
- **PauseMenu:** overlay over GameplayLevel (game paused, time frozen).
- **LevelComplete:** overlay fired when last critter exits.

---

## 1. Level Progression

- **Sequential unlock.** Level `1` always unlocked. Level `N` is unlocked iff `(N == 1) || completedLevels.Contains(N-1)`.
- Completing level `N` adds `N` to `completedLevels` and unlocks `N+1`.
- **Current level** = lowest level number that is unlocked AND not in `completedLevels` (the next thing to play). Highlighted on LevelSelect.
- **Completed levels** are marked and remain replayable (replaying does not change save state beyond `lastPlayedLevel`).
- Total levels: `25` (constant `TOTAL_LEVELS = 25`).

---

## 2. Level Select Screen Layout

- **Grid:** 5×5 = 25 buttons. Row-major, level `1` at top-left, level `25` at bottom-right. Scrolling not required (fits portrait screen).
- Title bar: "Select Level". No currency/lives bars (MVP).

**Button states:**

| State | Condition | Visual | Interactable |
|-------|-----------|--------|--------------|
| `Locked` | not unlocked | grey button + lock icon, no number | No (tap = small shake/no-op) |
| `Unlocked` | unlocked, not completed | colored button, level number shown | Yes |
| `Completed` | in `completedLevels` | colored button + checkmark badge, number shown | Yes (replay) |
| `Current` | the current level (§1) | `Unlocked` styling + pulsing highlight/glow ring | Yes |

- A button can be both `Completed` and replayable; `Current` highlight applies to exactly one button.

---

## 3. Save Data Schema

- **Storage:** single JSON file, key `gridlock_save_v1`. Written via `PlayerPrefs` string or `Application.persistentDataPath/save.json` (meta-dev's choice; one file).
- **Write triggers:** on level complete, on pause→LevelSelect/quit, on app pause/quit.

```json
{
  "completedLevels": [1, 2, 3],
  "lastPlayedLevel": 4,
  "totalPlayTime": 372.5
}
```

| Field | Type | Meaning | Default |
|-------|------|---------|---------|
| `completedLevels` | `int[]` | level numbers cleared; order-insensitive, deduplicated, 1-based | `[]` |
| `lastPlayedLevel` | `int` | last level the player entered (loaded into gameplay) | `0` (none) |
| `totalPlayTime` | `float` | cumulative seconds spent in GameplayLevel scenes | `0.0` |

- `lastPlayedLevel` updates the moment a GameplayLevel is loaded (not on completion).
- `totalPlayTime` accumulates only while a level is active and not paused.
- No save = treat as all defaults (fresh install → level 1).

---

## 4. Retry Flow

- **Instant, free, unlimited.** No cost, no wait, no ad, no confirmation dialog.
- Sources: PauseMenu "Retry" button, or in-fail soft state (player choice — there is no hard fail screen per GDD §5).
- Action: reset current level to its initial board state in-place (reload/reset the same GameplayLevel scene). Does **not** change `completedLevels`. `lastPlayedLevel` stays the same level.
- Undo (one-step, unlimited) is handled in gameplay scene, not meta — out of scope for this spec beyond noting both exist.

---

## 5. Level Complete Screen

Overlay fired when the last critter exits.

**Appears:**
- Celebration animation trigger: `OnLevelComplete` → play confetti/sparkle burst + cozy win chime (functional VFX/SFX only, MVP).
- **Completion checkmark** (large, centered). **No star rating, no score, no move count** for MVP.
- Buttons:
  - **"Next Level"** — primary. Loads level `N+1`. Hidden/disabled if `N == 25` (replaced by "Level Select").
  - **"Level Select"** — returns to LevelSelect screen.

**On show:** add `N` to `completedLevels` (dedup), unlock `N+1`, write save.

---

## 6. Resume Flow

On app relaunch / boot:

1. Load save. If none → go to LevelSelect (level 1 is the current level).
2. Compute `resumeLevel = current level` (§1) = lowest unlocked, not-completed level.
3. Go **directly into GameplayLevel `resumeLevel`** (skip LevelSelect), loading that level fresh.
4. If all 25 completed → go to LevelSelect (no incomplete level to resume).

*Note: resume targets the next incomplete level, derived from `completedLevels`, not raw `lastPlayedLevel` (which may point at an already-cleared replay).*

---

## 7. Pause Menu

- Trigger: Pause button (top-right) in GameplayLevel. Freezes game time; `totalPlayTime` accumulation stops while paused.
- Overlay with 3 buttons:
  - **"Resume"** — close overlay, unfreeze, return to current play state.
  - **"Retry"** — reset current level (§4).
  - **"Level Select"** — write save, exit to LevelSelect screen.
- No "Quit to title" (no title screen in MVP), no settings beyond the input-mode toggle (lives in gameplay/settings, not pause core).

---

## 8. Explicitly Excluded from MVP

Per GDD §8. Meta-dev must **not** implement:

- Stars-per-level / move-count rating / any per-level score.
- Soft currency, hard currency, any economy.
- Lives, energy/stamina, fail-cost, timers-as-gate.
- Ads (rewarded/interstitial/banner), IAP, fail-offers, packs.
- Social features, friend lists, leaderboards, daily events, streaks.
- Collection/cosmetics, profile, settings beyond input toggle.

The meta exists only to make 25 levels navigable and resumable.
