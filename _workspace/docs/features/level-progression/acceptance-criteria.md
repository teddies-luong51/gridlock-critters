# Level Progression — Acceptance Criteria
_Last updated: 2026-06-10_

## Overview
Levels unlock sequentially as the player completes them, and the game resumes at the lowest incomplete level across sessions.

## Linked Docs
- spec: ../level-progression/meta-spec.md
- architecture: ../../architecture/data-architecture.md

## Definition of Done
- [ ] Spec reviewed and signed off
- [ ] Code written and peer-reviewed
- [ ] Behaviour verified in Unity Editor

## ✅ Test Environment Gate
All of the following must pass before merging to the test branch:

- [ ] Level 1 is always unlocked on a fresh install.
- [ ] Level N unlocks only after Level N-1 is completed.
- [ ] Completing a level saves immediately (write to disk within the same frame as completion).
- [ ] Resume loads the lowest incomplete level via `GetResumeLevel()`, not the raw `lastPlayedLevel`.
- [ ] LevelSelect shows the correct state for each button (Locked / Unlocked / Current / Completed).
- [ ] Completing level 30 does not crash or attempt to load level 31.
- [ ] `LevelSelect` only builds buttons up to `min(TotalLevels, levels.Length)` — no unlocked button points at a missing LevelData asset.

## 🚀 Production Gate
All of the following must pass before shipping to production:

- [ ] Kill the app mid-level → reopen → the correct resume level loads.
- [ ] Complete a level → kill the app → reopen → that level shows as Completed in the select screen.
- [ ] Full 30-level chain: complete all levels → all show Completed, no progression bugs, no off-by-one gaps.
- [ ] Exactly 30 authored `LevelData` assets exist (matches `TotalLevels = 30`); no empty/black level on resume.
- [ ] Resume + unlock chain verified on both iOS and Android devices.
- [ ] Returning from a completed level refreshes LevelSelect button states immediately on device.

## Known Risks / Edge Cases
- **H-2:** `LevelProgressManager.TotalLevels = 30` but `meta-spec.md`/`core-loop-spec.md` still say 25. Must ensure 30 LevelData assets exist at build time; `LoadLevel` guards `levelIndex >= levels.Length` by logging an error and returning (player taps unlocked button 26+ and nothing happens). Reconcile specs to 30.
- **H-1 (index base):** progress code is 0-based; `meta-spec.md` documents 1-based. `lastPlayedLevel` default `0` collides with "level index 0 = level 1" — "none played" is indistinguishable from "played level 1." `GetResumeLevel` works because it ignores `lastPlayedLevel`, but any future consumer of `lastPlayedLevel == 0` as a sentinel will misbehave. Recommended: keep 0-based, fix spec, use `-1` "none" sentinel.
- **Cross-boundary chain (C-2/C-3, fixed):** completion must propagate critter-exit → `GameManager.ChangeState(LevelComplete)` → `MarkCompleted` → `OnProgressChanged` → `LevelSelect.Refresh`. Re-test end-to-end after the Fix Pass.
