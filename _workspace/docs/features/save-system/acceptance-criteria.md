# Save System — Acceptance Criteria
_Last updated: 2026-06-10_

## Overview
Persists player progress (completed levels, last played level, total play time) to PlayerPrefs as JSON, with safe defaults on missing or corrupt data.

## Linked Docs
- spec: ../../architecture/data-architecture.md _(not yet written — schema below)_
- architecture: ../../architecture/data-architecture.md

**Save schema:** key `"gridlock_save_v1"` — fields: `completedLevels` (int[]), `lastPlayedLevel` (int), `totalPlayTime` (float).

## Definition of Done
- [ ] Spec reviewed and signed off
- [ ] Code written and peer-reviewed
- [ ] Behaviour verified in Unity Editor

## ✅ Test Environment Gate
All of the following must pass before merging to the test branch:

- [ ] `SaveManager.Save()` writes valid JSON to PlayerPrefs under `gridlock_save_v1`.
- [ ] `SaveManager.Load()` returns the correct data after an app restart.
- [ ] Corrupted or missing save returns a safe default with no crash.
- [ ] `MarkCompleted(n)` adds `n` to `completedLevels` and does not create a duplicate entry.
- [ ] `GetResumeLevel()` returns 0 on a fresh save.
- [ ] `GetResumeLevel()` returns the correct next level after partial completion.

## 🚀 Production Gate
All of the following must pass before shipping to production:

- [ ] Save survives an app update (schema version key `_v1` checked; unknown/newer versions degrade safely).
- [ ] Save works on both iOS and Android.
- [ ] `totalPlayTime` accumulates correctly across multiple sessions.
- [ ] A deliberately corrupted non-empty PlayerPrefs string does not hard-crash on load.
- [ ] No data loss when the app is killed immediately after a `Save()` call.

## Known Risks / Edge Cases
- **L-2:** `SaveManager.Load()` uses `loaded ?? new SaveData()`. `JsonUtility.FromJson` returns null for empty/whitespace but **throws** on a corrupt non-empty string rather than returning null. Wrap `FromJson` in try/catch and fall back to default to avoid a hard crash on corrupt save.
- **H-1 (index base):** `lastPlayedLevel` default `0` collides with 0-based "level index 0." Consider a `-1` "none" sentinel so "no level played" is distinguishable from "played level 1."
- **Schema divergence:** `save-schema.json` documents 0-based while `meta-spec.md` is 1-based — reconcile before adding any new consumer of these fields.
