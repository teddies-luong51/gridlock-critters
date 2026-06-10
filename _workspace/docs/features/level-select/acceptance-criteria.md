# Level Select — Acceptance Criteria
_Last updated: 2026-06-10_

## Overview
A scrollable grid of 30 level buttons, each showing one of four states reflecting the player's progression.

## Linked Docs
- spec: ../level-select/ _(no spec yet — 4 states described below)_
- architecture: ../../architecture/data-architecture.md

**Button states:** Locked (grey, non-tappable) / Unlocked (white) / Current (yellow border) / Completed (green checkmark).

## Definition of Done
- [ ] Spec reviewed and signed off
- [ ] Code written and peer-reviewed
- [ ] Behaviour verified in Unity Editor

## ✅ Test Environment Gate
All of the following must pass before merging to the test branch:

- [ ] Fresh install: Level 1 = Current, Levels 2–30 = Locked.
- [ ] After completing Level 1: Level 1 = Completed, Level 2 = Current, the rest = Locked.
- [ ] Tapping a Locked button does nothing.
- [ ] Tapping any Unlocked / Current / Completed button loads that level.
- [ ] Returning from a completed level refreshes button states immediately (`LevelSelect.Refresh` on return + `OnEnable` re-subscribe).
- [ ] Scrolling through 30 buttons is smooth with no frame drops.

## 🚀 Production Gate
All of the following must pass before shipping to production:

- [ ] 30 buttons render correctly on iPhone SE (small) and iPhone 14 Pro Max (large).
- [ ] 30 buttons render correctly on Android 5" and 6.7" screens.
- [ ] Button states persist after an app restart.
- [ ] Tapping a Current/Completed button on device routes through `GameManager.Instance.LevelManager` and actually loads the level.
- [ ] Scroll performance holds 60fps with all 30 buttons instantiated on a mid-range Android.

## Known Risks / Edge Cases
- **H-2:** `LevelSelect` must only build buttons up to `min(TotalLevels, levels.Length)`. With `TotalLevels = 30` but fewer authored `LevelData` assets, an unlocked button can point at a missing asset → tap does nothing or loads a black/empty level.
- **C-3 (fixed):** `OnLevelButtonTapped` previously failed with "no ILevelManager registered." `LevelManager` now implements `ILevelManager` and registers in `Awake`; re-verify every tap loads on device.
- **Refresh timing:** refresh-on-return is correct only once `MarkCompleted` fires and `OnProgressChanged` is raised (depends on the C-2 win-chain fix). Re-test the completed→return→refresh path end-to-end.
