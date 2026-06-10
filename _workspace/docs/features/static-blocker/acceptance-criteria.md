# Static Blocker — Acceptance Criteria
_Last updated: 2026-06-10_

## Overview
An immovable grey stone tile (`#`) that critters cannot slide through from any direction; sliding critters stop in the cell immediately before it.

## Linked Docs
- spec: _none yet — see Overview above and core-loop-spec.md §4 (slide resolution)_
- architecture: ../../architecture/data-architecture.md

## Definition of Done
- [ ] Spec reviewed and signed off
- [ ] Code written and peer-reviewed
- [ ] Behaviour verified in Unity Editor

## ✅ Test Environment Gate
All of the following must pass before merging to the test branch:

- [ ] A critter sliding toward a blocker stops in the cell immediately before it.
- [ ] A blocker cannot be moved by any tilt (Left/Right/Up/Down).
- [ ] The blocker is correctly registered in the `SlideResolver` grid at level load.
- [ ] Multiple blockers in a row act as a continuous wall segment.
- [ ] A blocker does not interfere with a critter exiting through a matching gate (gate on board edge, blocker interior).
- [ ] A critter approaching a blocker from each of the four directions stops correctly.

## 🚀 Production Gate
All of the following must pass before shipping to production:

- [ ] L11 (intro level) plays correctly end-to-end on device.
- [ ] L15 (spike level) plays correctly end-to-end on device.
- [ ] No visual z-fighting between blocker and board tiles at the 45° camera angle.
- [ ] Blocker rendering is correct on both small (iPhone SE) and large (iPhone 14 Pro Max) screens.
- [ ] Blocker behaviour verified on a physical Android device.

## Known Risks / Edge Cases
- **L-5 (prefab null):** `LevelManager.LoadLevel` does `if (blockerPrefab == null) break;` — a single missing prefab silently skips ALL blockers of that type. Prefer `Debug.LogError` + skip so a missing asset is caught, not hidden.
- **Grid registration:** blocker must be present in the `SlideResolver` occupancy grid before the first tilt resolves; verify against scene-load order (related to L-1).
- **Non-matching gate parity:** a non-matching gate already behaves as a solid wall (`GateController.CanExit`), so blocker stop-logic and gate stop-logic should be consistent.
