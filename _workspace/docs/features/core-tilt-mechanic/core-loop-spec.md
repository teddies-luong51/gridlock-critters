# Gridlock Critters — Core Loop Technical Spec

*For gameplay-dev. Implements the GDD core loop. Date: 2026-06-09.*
*Renderer: Unity 3D. Input: accelerometer tilt. LOCKED decisions per task brief honored throughout.*

---

## 1. Grid Definition

The board is an axis-aligned rectangular grid of square cells inside a walled tray.

- **Dimensions:** `cols` × `rows`. MVP uses 5×5 and 6×6.
- **Column index:** letters `A, B, C, D, E (, F)` left → right.
- **Row index:** numbers `1, 2, 3, 4, 5 (, 6)` **top → bottom** (row 1 is the top/far edge in the 45° view, row N is the bottom/near edge).
- **Cell address:** `<Col><Row>`, e.g. `A1` = top-left, `E5` = bottom-right on a 5×5.
- **Internal coordinates:** `(x, y)` where `x` = 0-based column (A=0), `y` = 0-based row (row 1 = 0). Top-left = (0,0).

**Edges (where gates live):**
- **Top edge** — beyond row 1 (the far wall). A critter exiting Up leaves through a top-edge gate.
- **Bottom edge** — beyond row N (near wall). Exit Down.
- **Left edge** — left of column A. Exit Left.
- **Right edge** — right of column N. Exit Right.

**Direction → coordinate delta:**
| Direction | Δx | Δy | Destination wall |
|-----------|----|----|------------------|
| Left  | −1 | 0  | Left edge  |
| Right | +1 | 0  | Right edge |
| Up    | 0  | −1 | Top edge   |
| Down  | 0  | +1 | Bottom edge |

---

## 2. Critter Data Model

```csharp
enum CritterColor { Red, Blue, Green, Yellow, Purple, Orange }

enum CritterState { OnBoard, Sliding, Exiting, Exited }

class Critter {
    int        id;        // unique per level
    CritterColor color;
    Vector2Int cell;      // current (x, y); valid while OnBoard
    CritterState state;   // drives animation + win check
}
```

- Each cell holds at most **one** critter (no stacking).
- `color` determines which gates this critter may exit through.
- A critter occupies exactly one cell until it exits.

---

## 3. Gate Data Model

```csharp
enum Edge { Top, Bottom, Left, Right }

class Gate {
    Edge         edge;       // which wall
    int          slotIndex;  // position along that wall
    CritterColor color;      // only this color may pass
}
```

- **slotIndex** maps to the perpendicular axis of the edge:
  - Top / Bottom gate: `slotIndex` = column index (0..cols−1).
  - Left / Right gate: `slotIndex` = row index (0..rows−1).
- A gate is a **colored opening** in the wall at that slot.
- A critter may exit through a gate **iff** the gate is on the destination wall, aligned to the critter's slot, **and** `gate.color == critter.color`.
- A gate of a **non-matching color is a solid wall** to that critter (it stops there as if the wall were closed). (LOCKED)
- Multiple gates of different colors may exist on the same wall at different slots. A given slot has at most one gate.

---

## 4. Tilt Resolution Rules (exact algorithm)

When a tilt in direction `D` is registered, resolve the **entire board simultaneously** with this deterministic algorithm. The key invariant: **process critters in order starting from the destination wall and moving inward**, so that a critter never moves into a cell a not-yet-processed critter is about to vacate within the same tilt — each critter slides as far as it can given everything ahead of it has already settled.

### 4.1 Processing order
- For **Right**: process critters in **descending x** (rightmost first).
- For **Left**: process in **ascending x** (leftmost first).
- For **Down**: process in **descending y** (bottommost first).
- For **Up**: process in **ascending y** (topmost first).

(Ties on the moving axis don't matter; they're on different lines perpendicular to motion.)

### 4.2 Per-critter slide
For each critter `c` in processing order:
```
let (dx, dy) = delta(D)
let target = c.cell
loop:
    let next = target + (dx, dy)
    if next is still inside the grid:
        if cell `next` is occupied (by an already-settled critter):
            break            // blocked by another critter → stop at `target`
        else:
            target = next    // free cell → keep sliding
            continue
    else:
        // `next` is off the board → we are at the wall on edge D, slot = perpendicular index of `target`
        let gate = GateAt(edge(D), slot(target))
        if gate != null AND gate.color == c.color:
            c.state = Exiting     // matching gate → critter exits
            mark c as leaving (frees its cell)
            target = OFF_BOARD
        // else: no gate, or non-matching gate → solid wall, stop at `target`
        break
if target == OFF_BOARD:
    c.state -> Exited (after exit animation)
else:
    c.cell = target           // settle in final cell
```

### 4.3 Why order-from-destination-wall works
Processing the critter nearest the destination wall **first** means it either exits (freeing its lane) or settles against the wall. The next critter inward then sees the correct settled state and packs against it. This produces the intuitive "everything slides and stacks against that wall, matching ones fall out the gate" behavior with no two-pass needed.

### 4.4 No-op detection
If, after resolution, **no critter changed cell and none exited**, the tilt was a no-op (every critter was already flush against the wall/blockers with no matching gate). The UI still plays the small board-lean but no slide; this is not a fail, just a wasted lean.

---

## 5. Slide Animation Spec

- **Simultaneous:** all moving critters animate at the same time (they were resolved together in §4).
- **Trigger:** on registered tilt, after board-lean begins (§6), critters start sliding ~0.05 s later (lean leads slightly to sell causality).
- **Easing:** `ease-out` (decelerate into final cell) — `OutCubic`. A critter that travels farther covers more distance in the same time window (constant duration) OR moves at constant speed (constant velocity); MVP uses **constant speed** so longer slides feel weightier.
- **Speed:** **9 cells / second** (≈ 0.11 s per cell). A full 6-cell slide ≈ 0.66 s — within the 0.3–0.8 s auto-resolve budget (MVP P3).
- **Settle:** tiny **elastic overshoot** (`OutBack`, ~4% overshoot, 0.08 s) on the final cell stop / on collision contact. Soft thud SFX on collision; no thud if the critter exits.
- **Exit animation:** matching critter continues through the gate opening, scales down + fades + sparkle, 0.25 s, cozy chime. Then `Exited`.
- **Input lock:** input is locked from tilt registration until the longest slide + settle completes (max ~0.8 s), respecting the 0.35 s minimum cooldown from the GDD.

---

## 6. Board Tilt Animation Spec

- On registered tilt, the **board model** (not the camera) leans toward the tilt direction.
- **Lean angle:** **15°** about the axis perpendicular to the tilt direction.
- **Lean in:** 0.12 s, `OutCubic`.
- **Hold:** during the slide resolution.
- **Return:** board eases back to flat over 0.18 s, `InOutCubic`, starting once all slides have settled.
- The camera stays fixed at the 45° view throughout (per GDD §4) — only the board tilts. This sells "I tipped the tray" without disorienting the player.

---

## 7. Win Detection

After every tilt resolution + animation completes:
```
if (all critters have state == Exited):
    fire LevelComplete
```
Check runs once per resolved tilt. No per-frame polling needed.

---

## 8. Fail Detection (optional for MVP)

Per GDD §5, MVP has **no hard fail**. Two options, both shipped:
- **Undo:** one-step undo of the last tilt (snapshot the pre-tilt board state stack). Unlimited.
- **Retry:** reset to the level's initial state.

**Optional soft hint (deferred / stub):** a "gridlock detector" could test all 4 directions from the current state and, if none changes the board AND not all exited, surface a gentle "Undo?" nudge. Implement only if cheap; not required for MVP. No fail screen, no move limit.

---

## 9. Level Data Schema

Levels are pure data (JSON or ScriptableObject). Example schema:

```json
{
  "id": 1,
  "cols": 5,
  "rows": 5,
  "critters": [
    { "id": 0, "color": "Red",  "cell": "C3" }
  ],
  "gates": [
    { "edge": "Right", "slot": 2, "color": "Red" }
  ]
}
```

**Field rules:**
- `cell` is the `<Col><Row>` address (parser converts to (x,y)).
- Gate `slot`: for Top/Bottom = column index (0-based, A=0); for Left/Right = row index (0-based, row1=0).
- Every critter color **must** have at least one reachable matching gate (validator should assert solvability — at minimum, one matching-color gate exists per color present).
- No two critters share a starting cell; no two gates share an (edge, slot).

**Validator (build-time):** load each level, run a BFS over tilt-states (≤4 branches/state) from the initial state; assert a win state is reachable within a sane depth (e.g. ≤12 tilts). This guarantees every shipped level is solvable. All 25 levels in `level-design-spec.md` have hand- or sequence-verified solutions to seed this.
