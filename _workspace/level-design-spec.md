# Gridlock Critters — Level Design Spec (MVP · 25 Levels)
_Mechanics: Tilt board → critters slide → exit through matching-color gates_
_New Input: Physical accelerometer tilt (Left/Right/Up/Down)_
_View: 3D perspective, 45° angled camera_

## Mechanic Progression
| Levels | New Mechanic |
|--------|-------------|
| 1–5    | Core: slide + matching gate exit |
| 6–10   | Blocking: critter order matters |
| 11–15  | Multi-step chains |
| 16–20  | Gate scarcity + sequencing |
| 21–25  | Mastery: all mechanics |

---

## Conventions used in this spec
- Columns `A–E` (5-wide) or `A–F` (6-wide), left→right. Rows `1–N`, top→bottom (row 1 = far wall in the 45° view).
- `[R]` = a critter of that color; `[ ]` = empty cell.
- Gates listed as `Edge-Slot=Color`. Slot = column letter (Top/Bottom gates) or row number (Left/Right gates). Inline arrows (`→R`, `↓B`, `↑G`, `←Y`) mark a colored gate opening on that wall next to its row/column.
- Tilt resolution per core-loop-spec §4: all critters slide simultaneously; processing runs from the destination wall inward (lead settles/exits first, followers pack behind). A non-matching gate is a solid wall.
- "Tilt Right" = player physically tips the tray so every critter slides right, etc.

---

## LEVELS 1–5 — TUTORIAL (core mechanic only, 5×5)

### Level 1 — "First Slide" — INTRODUCE — Easy
**Grid:** 5×5
**Est. time:** 5–10 sec
**New element:** Core loop — tilt to slide, exit through a matching-color gate.

```
     A    B    C    D    E
1  [ ]  [ ]  [ ]  [ ]  [ ]
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [R]  [ ]  [ ]  [ ]  [ ]   →R  (Right gate, row 3)
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [B]  [ ]  [ ]  [ ]  [ ]   →B  (Right gate, row 5)
```
Gates: Right-3=Red, Right-5=Blue
Critters: R at A3, B at A5

**Solution:** Tilt Right.

**Simulation:**
- Start: R=A3, B=A5.
- Tilt Right (rows independent): R slides A3→E3→off right edge row 3; Right-3=Red matches → R exits. B slides A5→E5→off right edge row 5; Right-5=Blue matches → B exits.
- Board empty → WIN.

**Key decision:** None — pure teaching tilt. Both critters are dead-aligned with their gates; one Right tilt clears the board.
**P1–P5 check:**
- P1: Two critters on the left, two same-color glowing gates directly across — direction to tilt is obvious.
- P2: Cluttered left wall → empty board; sparkle exits = clear payoff.
- P3: Single tilt; both slides auto-resolve simultaneously.
- P4: Introduces exactly one thing (slide + match-exit).
- P5: Trivial by design (level-1 success-rate gate); real decisions start L2+.

---

### Level 2 — "Drop Down" — INTRODUCE — Easy
**Grid:** 5×5
**Est. time:** 5–12 sec
**New element:** Vertical axis — gates on top/bottom walls, tilt Up/Down.

```
     A    B    C    D    E
1  [R]  [ ]  [ ]  [ ]  [B]
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [ ]  [ ]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]
   ↓R                  ↓B   (Bottom gates: col A=Red, col E=Blue)
```
Gates: Bottom-A=Red, Bottom-E=Blue
Critters: R at A1, B at E1

**Solution:** Tilt Down.

**Simulation:**
- Start: R=A1, B=E1.
- Tilt Down (columns independent): R slides A1→A5→off bottom edge col A; Bottom-A=Red → R exits. B slides E1→E5→off bottom edge col E; Bottom-E=Blue → B exits.
- Board empty → WIN.

**Key decision:** Read that the gates are on the bottom wall, so the tilt is Down, not Right. Teaches the second axis.
**P1–P5 check:**
- P1: Critters at top, matching gates glowing at the bottom in the same columns — implies Down.
- P2: Top row clears, board empties.
- P3: One tilt; both auto-resolve.
- P4: One new idea: the Up/Down axis.
- P5: Minor — read gate placement to pick the axis.

---

### Level 3 — "Tag Along" — REINFORCE — Easy
**Grid:** 5×5
**Est. time:** 8–15 sec
**New element:** none (third critter; all three share a destination wall on different rows).

```
     A    B    C    D    E
1  [ ]  [ ]  [ ]  [ ]  [ ]
2  [R]  [ ]  [ ]  [ ]  [ ]   →R  (Right gate, row 2)
3  [G]  [ ]  [ ]  [ ]  [ ]   →G  (Right gate, row 3)
4  [B]  [ ]  [ ]  [ ]  [ ]   →B  (Right gate, row 4)
5  [ ]  [ ]  [ ]  [ ]  [ ]
```
Gates: Right-2=Red, Right-3=Green, Right-4=Blue
Critters: R at A2, G at A3, B at A4

**Solution:** Tilt Right.

**Simulation:**
- Start: R=A2, G=A3, B=A4 (each on its own row, each row carries its matching gate).
- Tilt Right (rows independent): R: A2→E2→exit Red gate row 2. G: A3→E3→exit Green gate row 3. B: A4→E4→exit Blue gate row 4.
- Board empty → WIN.

**Key decision:** Three critters, but one tilt sends each to its own aligned gate. Reinforces that critters move simultaneously and independently across separate rows.
**P1–P5 check:**
- P1: Three critters stacked left, three matching gates directly across — Right is obvious.
- P2: Full left column clears at once — bigger payoff than L1.
- P3: One tilt; three simultaneous slides.
- P4: No new mechanic; just a third critter.
- P5: Light — confirm all three are independently aligned before tilting.

---

### Level 4 — "Two Directions" — REINFORCE — Easy-Medium
**Grid:** 5×5
**Est. time:** 12–20 sec
**New element:** none (first puzzle needing tilts on two different walls, in sequence).

```
     A    B    C    D    E
1  [ ]  [ ]  [ ]  [ ]  [B]   →B  (Right gate, row 1)
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [R]  [ ]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]
   ↑R                       (Top gate, col A=Red)
```
Gates: Top-A=Red, Right-1=Blue
Critters: R at A3, B at E1

**Solution:** Tilt Up, then Tilt Right.

**Simulation:**
- Start: R=A3, B=E1.
- Tilt Up (columns independent): R: A3→A1→off top edge col A; Top-A=Red → R exits. B: E1 is already on row 1; moving up takes it off the top edge col E — no gate there → B stays at E1.
  - State: B=E1.
- Tilt Right: B: E1→off right edge row 1; Right-1=Blue → B exits.
- Board empty → WIN.

**Key decision:** Two critters want two different walls. Clear R out the top, then send B out the right. (Right-first also works since they never interfere; the lesson is "different walls = separate tilts.")
**P1–P5 check:**
- P1: R sits under its top gate, B beside its right gate — two exit directions cued.
- P2: Board clears over two satisfying tilts.
- P3: Each tilt auto-resolves.
- P4: No new mechanic; first two-direction puzzle.
- P5: A real (if forgiving) choice of which tilt first.

---

### Level 5 — "Three Steps" — REINFORCE — Medium
**Grid:** 5×5
**Est. time:** 18–30 sec
**New element:** none (ordering — a wrong first tilt strands a critter).

```
     A    B    C    D    E
1  [ ]  [ ]  [ ]  [ ]  [ ]   ←R Left-1=Red ; →B Right-1=Blue
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [R]  [ ]  [G]  [ ]  [B]
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]
              ↑G            Top-C=Green
```
Gates: Top-C=Green, Left-1=Red, Right-1=Blue
Critters: R at A3, G at C3, B at E3

**Solution:** Tilt Up → Tilt Left → Tilt Right.

**Simulation:**
- Start: R=A3, G=C3, B=E3 (all on row 3).
- **Tilt Up** (columns independent): R: A3→A1 (no top gate col A) → stops A1. G: C3→C1→off top col C; Top-C=Green → G exits. B: E3→E1 (no top gate col E) → stops E1.
  - State: R=A1, B=E1.
- **Tilt Left** (ascending x → R first): R: A1→off left edge row 1; Left-1=Red → R exits. B: E1→B1→A1 (lane clear) →off left edge row 1; Left-1 is Red, B is Blue → non-match wall → B stops at A1.
  - State: B=A1.
- **Tilt Right:** B: A1→E1→off right edge row 1; Right-1=Blue → B exits.
- Board empty → WIN.

**Key decision:** Fire Green out the TOP first. If you tilt Left or Right first, Green gets dragged out of column C and can never reach its Top-C gate. First level where wrong ordering strands a critter — sets up the blocking levels. (Recoverable via Undo.)
**P1–P5 check:**
- P1: Three critters on a line, three gates on three different walls — exits cued all around.
- P2: Each tilt pops one critter; board empties in three beats.
- P3: Every tilt auto-resolves.
- P4: No new mechanic; reinforces sequencing.
- P5: Genuine order dependency — wrong first tilt strands Green.

---

## LEVELS 6–10 — BLOCKING (critters block each other)

### Level 6 — "After You" — INTRODUCE — Medium
**Grid:** 5×5
**Est. time:** 15–25 sec
**New element:** Blocking — two critters stacked in one column; one must exit before the other can reach its gate.

```
     A    B    C    D    E
1  [ ]  [ ]  [B]  [ ]  [ ]   ↑B Top-C=Blue
2  [ ]  [ ]  [R]  [ ]  [ ]
3  [ ]  [ ]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]
              ↓R           Bottom-C=Red
```
Gates: Top-C=Blue, Bottom-C=Red
Critters: B at C1, R at C2 (stacked in column C)

**Solution:** Tilt Up (B exits top; R packs to C1), then Tilt Down (R exits bottom).

**Verification of key moves:**
- Tilt Up (ascending y → B at C1 first): B: C1→off top col C; Top-C=Blue → B exits. R: C2→C1→off top col C? Top-C is Blue, R is Red → non-match wall → R stops at C1.
  - State: R=C1.
- Tilt Down: R: C1→C5→off bottom col C; Bottom-C=Red → R exits. WIN.

Note: Down-first also solves (R exits bottom; B walls at C5; then Up sends B out the top). The two-critter lane reverses cleanly here — the teaching point is simply that stacked critters take turns.
**Key decision:** Two critters share a column and want opposite walls. Fire one out its gate; the lane opens for the next.
**P1–P5 check:**
- P1: Two stacked critters, a blue top gate and red bottom gate on their column — both pulls visible.
- P2: Column empties in two pops.
- P3: Auto-resolves; the follower packs behind automatically.
- P4: One new idea: same-lane critters interact.
- P5: Player must see both can't exit on one tilt and sequence them.

---

### Level 7 — "Step Aside" — REINFORCE — Medium
**Grid:** 5×5
**Est. time:** 20–30 sec
**New element:** none (a blocker beside your critter must be removed without trapping you).

```
     A    B    C    D    E
1  [ ]  [ ]  [ ]  [ ]  [ ]   →R Right-1=Red
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [R]  [G]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]
        ↑G                 Top-B=Green
```
Gates: Top-B=Green, Right-1=Red
Critters: R at A3, G at B3

**Solution:** Tilt Up → Tilt Right.

**Verification:**
- Tilt Up (columns independent): R: A3→A1 (no top gate col A) → stops A1. G: B3→B1→off top col B; Top-B=Green → G exits.
  - State: R=A1.
- Tilt Right: R: A1→E1→off right edge row 1; Right-1=Red → R exits. WIN.

Wrong move — Right first: R: A3→B3? blocked by G → R stays A3 (no-op). G: B3→E3 (no right gate row 3) → stuck mid-board. Critters tangle.
**Key decision:** One tilt (Up) simultaneously removes the green blocker (out its top gate) AND stages R against the top wall, lined up for the finishing Right tilt. Tilting Right first jams R into G.
**P1–P5 check:**
- P1: R, the green blocker beside it, and two gates on two walls visible at load.
- P2: Two-tilt clear.
- P3: Tilts auto-resolve; G exits and R packs in the same motion.
- P4: No new mechanic; reinforces removing a blocker.
- P5: Decision: which tilt clears the blocker without trapping your own critter.

---

### Level 8 — "Right Order" — COMBINE — Medium
**Grid:** 5×5
**Est. time:** 25–35 sec
**New element:** none (combines blocking + ordering; reversing the order parks a critter against the wrong gate).

```
     A    B    C    D    E
1  [ ]  [ ]  [R]  [ ]  [ ]   ↑R Top-C=Red
2  [ ]  [ ]  [B]  [ ]  [ ]
3  [ ]  [ ]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]
              ↓B          Bottom-C=Blue
```
Gates: Top-C=Red, Bottom-C=Blue
Critters: R at C1, B at C2 (stacked in column C, R above B)

**Solution:** Tilt Up (R exits top; B packs to C1), then Tilt Down (B exits bottom).

**Verification (correct path):**
- Tilt Up (ascending y → R at C1 first): R: C1→off top col C; Top-C=Red → R exits. B: C2→C1→off top col C? Top-C is Red, B is Blue → non-match wall → B stops at C1.
  - State: B=C1.
- Tilt Down: B: C1→C5→off bottom col C; Bottom-C=Blue → B exits. WIN.

**The trap (wrong order — Down first):**
- Tilt Down (descending y → B at C2 first): B: C2→C5→off bottom col C; Bottom-C=Blue → B exits. R: C1→C5→off bottom col C? Bottom-C is Blue, R is Red → non-match wall → R stops at C5.
  - State: R parked against the wrong-colored bottom gate at C5. The player must Undo (or Tilt Up to bring R back to its red top gate).

**Key decision:** Both critters share column C and want opposite walls. Exit the critter nearest its OWN gate first (R → top). Reversing the order parks R against the blue gate it cannot pass. Recoverable via Undo, but teaches "right order = no backtracking."
**P1–P5 check:**
- P1: Two stacked critters; red top gate and blue bottom gate on their column — both pulls visible.
- P2: Column clears top-then-bottom in two pops.
- P3: Auto-resolves; the follower packs behind the leader.
- P4: No new mechanic; combines blocking (L6) with order-consequence (L5).
- P5: Real ordering decision with a visible-on-reflection wrong path.

---

### Level 9 — "Tangle" — SPIKE — Hard
**Grid:** 5×5
**Est. time:** 35–60 sec
**New element:** none (four critters, two co-lane pairs that block each other).

```
     A    B    C    D    E
1  [R]  [ ]  [ ]  [ ]  [G]   ↑R Top-A=Red ; ↑G Top-E=Green
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [ ]  [ ]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [B]  [ ]  [ ]  [ ]  [Y]   ↓B Bottom-A=Blue ; ↓Y Bottom-E=Yellow
```
Gates: Top-A=Red, Top-E=Green, Bottom-A=Blue, Bottom-E=Yellow
Critters: R at A1, G at E1, B at A5, Y at E5

**Solution:** Tilt Up → Tilt Down.

**Verification:**
- Tilt Up (ascending y → top critters first):
  - R: A1→off top col A; Top-A=Red → R exits. G: E1→off top col E; Top-E=Green → G exits.
  - B: A5→A1→off top col A? Top-A is Red, B is Blue → non-match wall → B stops at A1.
  - Y: E5→E1→off top col E? Top-E is Green, Y is Yellow → non-match wall → Y stops at E1.
  - State: B=A1, Y=E1.
- Tilt Down: B: A1→A5→off bottom col A; Bottom-A=Blue → B exits. Y: E1→E5→off bottom col E; Bottom-E=Yellow → Y exits.
  - Board empty → WIN.

**Why it's a spike:** Looks like four independent critters, but they are two stacked pairs (column A: R/B; column E: G/Y), each pair fighting for the same lane with opposite gates. One Up clears both top critters AND flips both bottom critters up; one Down clears those. Any horizontal tilt scrambles columns A and E into a single tangled lane.
**Key decision:** See past "4 separate critters" to "2 co-lane pairs." Solve purely on the vertical axis: Up then Down. Avoid horizontal tilts.
**P1–P5 check:**
- P1: Four corner critters, four colored gates at top/bottom of columns A and E — symmetric, readable.
- P2: Two big tilts empty the whole board; satisfying symmetric clear.
- P3: All four auto-resolve per tilt; followers pack and flip.
- P4: No new mechanic; SPIKE via doubled blocking.
- P5: Strong — recognize the pair structure and avoid the tangling horizontal tilts.

---

### Level 10 — "Easy Does It" — BREATHER — Easy-Medium
**Grid:** 5×5
**Est. time:** 12–20 sec
**New element:** none (looks busy, but one obvious tilt solves it — pacing relief after L9).

```
     A    B    C    D    E
1  [R]  [G]  [B]  [Y]  [ ]
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [ ]  [ ]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]
   ↓R   ↓G   ↓B   ↓Y       Bottom-A=Red, Bottom-B=Green, Bottom-C=Blue, Bottom-D=Yellow
```
Gates: Bottom-A=Red, Bottom-B=Green, Bottom-C=Blue, Bottom-D=Yellow
Critters: R at A1, G at B1, B at C1, Y at D1 (each directly above its own matching bottom gate)

**Solution:** Tilt Down.

**Verification:**
- Tilt Down (columns independent): R: A1→A5 → Bottom-A=Red → exits. G: B1→B5 → Bottom-B=Green → exits. B: C1→C5 → Bottom-C=Blue → exits. Y: D1→D5 → Bottom-D=Yellow → exits.
  - Board empty → WIN.

**Key decision:** Four critters, but each is column-aligned with its matching gate below. A single Down tilt rains them all out — restores flow after the spike.
**P1–P5 check:**
- P1: A colorful top row, each gate directly below its critter — Down is unmistakable.
- P2: The whole row drops out at once — biggest single-tilt payoff so far.
- P3: One tilt, four simultaneous exits.
- P4: No new mechanic; deliberate breather.
- P5: Minimal by design — a breather may relax P5 for pacing.

---

## LEVELS 11–13 — MULTI-STEP CHAINS

### Level 11 — "Pass Through" — INTRODUCE — Medium
**Grid:** 5×5
**Est. time:** 25–40 sec
**New element:** Chain — a critter reaches its gate only after riding to an intermediate position on a prior tilt.

```
     A    B    C    D    E
1  [ ]  [ ]  [ ]  [ ]  [ ]
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [B]  [R]  [ ]  [ ]  [ ]   ←B Left-3=Blue ; →R Right-3=Red
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]
```
Gates: Left-3=Blue, Right-3=Red
Critters: B at A3, R at B3 (R sits between B and B's left gate is fine; the two share the row, each wanting the opposite wall)

**Solution:** Tilt Right (R exits Right-3; B rides to the far right wall — its intermediate position), then Tilt Left (B slides back across and exits Left-3).

**Verification:**
- Tilt Right (descending x → R first): R: B3→E3→off right edge row 3; Right-3=Red → R exits. B: A3→E3 (lane now clear) →off right edge row 3? Right-3 is Red, B is Blue → non-match wall → B stops at E3 (intermediate position).
  - State: B=E3.
- Tilt Left: B: E3→A3→off left edge row 3; Left-3=Blue → B exits.
  - Board empty → WIN.

Note: Left-first also solves (B exits immediately; R walls at A3; then Right sends R out). Either order works in two tilts. The introduced idea is that the trailing critter is carried wall-to-wall as an intermediate step before its own exit.
**Key decision:** Two critters share row 3, each wanting the opposite side wall. Fire one out; the other is carried to the far wall (intermediate), then a reverse tilt sends it home. Introduces "a critter passes through an intermediate cell on the way to its gate."
**P1–P5 check:**
- P1: Two critters mid-row, blue gate left and red gate right — both directions cued.
- P2: Two clean exits; board empties.
- P3: Each tilt auto-resolves; the carried critter visibly rides wall-to-wall.
- P4: One new idea: gate reached only after an intermediate repositioning tilt.
- P5: Decision of which to fire first; the player sees the "ride across then come back" pattern.

---

### Level 12 — "One Then Two" — REINFORCE — Medium-Hard
**Grid:** 5×5
**Est. time:** 35–55 sec
**New element:** none (two-step chain: B's exit sets up R's exit; then a column-A pair finishes).

```
     A    B    C    D    E
1  [G]  [ ]  [ ]  [ ]  [ ]   ↑G Top-A=Green
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [R]  [B]  [ ]  [ ]  [ ]   ←R Left-3=Red ; →B Right-3=Blue
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [Y]  [ ]  [ ]  [ ]  [ ]   ↓Y Bottom-A=Yellow
```
Gates: Top-A=Green, Bottom-A=Yellow, Left-3=Red, Right-3=Blue
Critters: G at A1, R at A3, B at B3, Y at A5

**Solution:** Tilt Right → Tilt Left → Tilt Up → Tilt Down.

**Verification:**
- **Tilt Right** (rows independent). Row 1: G: A1→E1 (no right gate row 1) → stops E1. Row 3 (descending x → B first): B: B3→E3→off right edge row 3; Right-3=Blue → B exits. R: A3→E3 (lane clear) →off right edge row 3? Right-3 is Blue, R is Red → wall → R stops at E3 (intermediate). Row 5: Y: A5→E5 (no right gate row 5) → stops E5.
  - State: G=E1, R=E3, Y=E5.
- **Tilt Left** (rows independent). Row 1: G: E1→A1 (no left gate row 1) → stops A1. Row 3: R: E3→A3→off left edge row 3; Left-3=Red → R exits. Row 5: Y: E5→A5 (no left gate row 5) → stops A5.
  - State: G=A1, Y=A5.
- **Tilt Up** (column A; ascending y → G at A1 first): G: A1→off top col A; Top-A=Green → G exits. Y: A5→A1→off top col A? Top-A is Green, Y is Yellow → wall → Y stops at A1.
  - State: Y=A1.
- **Tilt Down:** Y: A1→A5→off bottom col A; Bottom-A=Yellow → Y exits.
  - Board empty → WIN.

**Two-step chain:** R cannot reach its Left-3 gate until (1) B clears the right side and (2) R has ridden to the far-right wall — a strict two-step dependency on B's exit. Steps 3–4 are an L9-style vertical pair clean-up in column A.
**Key decision:** B blocks R from the right; firing B right also carries R to the far wall as setup, then R comes home left. Plan the horizontal chain (Right→Left) before the vertical column-A cleanup (Up→Down). Vertical-first scrambles G/R/Y in column A.
**P1–P5 check:**
- P1: Four critters, four gates on all four walls — busy but readable.
- P2: Board clears in four beats, each a satisfying pop.
- P3: Every tilt auto-resolves; the ride-across is clearly animated.
- P4: No new mechanic; reinforces the chain via a two-step horizontal dependency.
- P5: Real planning — identify R's dependency on B and sequence horizontal-then-vertical.

---

### Level 13 — "Hidden Link" — SPIKE — Hard
**Grid:** 6×6
**Est. time:** 50–90 sec
**New element:** none (three-step chain with a hidden dependency revealed only mid-solve).

```
     A    B    C    D    E    F
1  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]   ↑R Top-A=Red
2  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
3  [B]  [ ]  [ ]  [ ]  [ ]  [ ]   →B Right-3=Blue
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [R]  [G]  [ ]  [ ]  [ ]  [ ]   →G Right-6=Green
```
Gates: Top-A=Red, Right-3=Blue, Right-6=Green
Critters: B at A3, R at A6, G at B6

**Solution:** Tilt Right → Tilt Left → Tilt Up.

**Simulation:**
- Start: B=A3, R=A6, G=B6.
- **Tilt Right** (rows independent). Row 3: B: A3→F3→off right edge row 3; Right-3=Blue → B exits. Row 6 (descending x → G first): G: B6→F6→off right edge row 6; Right-6=Green → G exits. R: A6→F6 (lane clear) →off right edge row 6? Right-6 is Green, R is Red → non-match wall → R stops at F6.
  - State: R=F6. (B and G exited.)
  - **Hidden dependency revealed:** the Right tilt that freed B and G also flung R all the way to F6 — far from its Top-A gate, which lives in column A. The player only discovers this after committing the tilt.
- **Tilt Left:** R: F6→A6→off left edge row 6 (no left gate row 6) → stops A6.
  - State: R=A6.
- **Tilt Up:** R: A6→A1→off top edge col A; Top-A=Red → R exits.
  - Board empty → WIN.

**Hidden link:** R needs Top-A, but B sits above it in column A — so B must leave first. The only way to clear B is Tilt Right, which (unexpectedly) also carries R to the far wall, forcing a corrective Tilt Left before the finishing Tilt Up. Trying Up first jams R behind B (R: A6→A4, blocked by B at A3) and exits nothing useful — teaching that column A must be cleared first.
**Key decision:** Recognize R's Top-A exit is blocked by B in the same column, so B must go first (Right). The mid-solve twist: that same tilt strands R in column F, requiring a Left correction before the final Up. Three coupled steps with a surprise.
**P1–P5 check:**
- P1: Four critters, three gates on three walls — the column-A stack (B over R) reads as a relationship.
- P2: A large board clears over three deliberate tilts; the final R exit is the payoff.
- P3: Each tilt auto-resolves; R's long ride to F6 and back is clearly animated.
- P4: No new mechanic; SPIKE — extends the chain to three steps with a hidden dependency.
- P5: Strong — the hidden link only appears mid-solve and forces an adaptive corrective tilt; failure is clearly mis-sequencing, recoverable via Undo.

---

## LEVELS 14–15 — MULTI-STEP CHAINS (completing the set)

### Level 14 — "Round Trip" — REINFORCE — Medium-Hard
**Grid:** 5×5
**Est. time:** 50–80 sec

```
     A    B    C    D    E
1  [ ]  [ ]  [ ]  [ ]  [ ]   ↑R Top-A=Red
2  [ ]  [ ]  [ ]  [ ]  [ ]
3  [B]  [R]  [ ]  [ ]  [G]   ←B Left-3=Blue ; →G Right-3=Green
4  [ ]  [ ]  [ ]  [ ]  [ ]
5  [Y]  [ ]  [ ]  [ ]  [ ]   ↓Y Bottom-A=Yellow
```
Gates: Top-A=Red, Bottom-A=Yellow, Left-3=Blue, Right-3=Green
Critters: B at A3, R at B3, G at E3, Y at A5

**Solution:** Tilt Right → Tilt Left → Tilt Up → Tilt Down.

**Simulation:**
- Start: B=A3, R=B3, G=E3, Y=A5.
- **Tilt Right** (rows independent). Row 3 (descending x → lead settles first): G: E3→off right edge row 3; Right-3=Green → G exits. R: B3→E3 (lane clear behind G) →off right edge? Right-3 is Green, R is Red → wall → R stops at E3. B: A3→D3 (blocked by R at E3) → B stops at D3 (first ferry leg — B is carried rightward). Row 5: Y: A5→E5 (no right gate row 5) → stops E5.
  - State: B=D3, R=E3, Y=E5.
- **Tilt Left** (rows independent). Row 3 (ascending x → lead settles first): B: D3→A3→off left edge row 3; Left-3=Blue → B exits. R: E3→B3 (packs behind where B was) → actually lane to left edge: Left-3 is Blue, R is Red → wall, R packs to A3. Row 5: Y: E5→A5 (no left gate row 5) → stops A5.
  - State: R=A3, Y=A5.
- **Tilt Up** (column A; ascending y → lead first): R: A3→A1→off top col A; Top-A=Red → R exits. Y: A5→A4 (blocked? R already exited) → Y: A5→A1→off top col A? Top-A is Red, Y is Yellow → wall → Y stops at A1.
  - State: Y=A1.
- **Tilt Down:** Y: A1→A5→off bottom col A; Bottom-A=Yellow → Y exits.
  - Board empty → WIN.

**Round-trip ferry:** B starts at A3 but is dragged RIGHT (to D3) when the board tilts to clear G — that is B's first ferry leg, away from its own Left-3 gate. The corrective Tilt Left then ferries B all the way back across to exit Left. B crosses the board twice before exiting. R and Y resolve as a column-A vertical pair afterward.
**Key decision:** You cannot reach G's right gate without also flinging B rightward; accept the round trip and bring B home on the reverse tilt before cleaning up column A. Doing vertical tilts first scrambles B/R/Y in column A.
**P1–P5 check:**
- P1: Four critters, four gates on all four walls — relationships readable.
- P2: Board clears in four pops; B's long return ride is the visual highlight.
- P3: Every tilt auto-resolves; followers pack behind leads.
- P4: No new mechanic; reinforces the chain via a two-leg ferry.
- P5: Real planning — recognize B's forced round trip and order horizontal-before-vertical.

---

### Level 15 — "Grand Chain" — SPIKE — Hard
**Grid:** 6×6
**Est. time:** 70–110 sec

```
     A    B    C    D    E    F
1  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
2  [B]  [R]  [ ]  [ ]  [ ]  [ ]   ←B Left-2=Blue ; →R Right-2=Red
3  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
4  [G]  [Y]  [P]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]   ↓G Bottom-A=Green ; ↓Y Bottom-B=Yellow ; ↓P Bottom-C=Purple
```
Gates: Left-2=Blue, Right-2=Red, Bottom-A=Green, Bottom-B=Yellow, Bottom-C=Purple
Critters: B at A2, R at B2, G at A4, Y at B4, P at C4

**Solution:** Tilt Right → Tilt Left → Tilt Down.

**Simulation:**
- Start: B=A2, R=B2, G=A4, Y=B4, P=C4.
- **Tilt Right** (rows independent). Row 2 (lead first): R: B2→F2 → Right-2=Red → R exits. B: A2→F2 (lane clear) → Right-2 is Red, B is Blue → wall → B stops at F2 (ferried fully across). Row 4 (lead first): P: C4→F4 (no right gate row 4) → stops F4. Y: B4→E4 (packs behind P) → stops E4. G: A4→D4 (packs behind Y) → stops D4.
  - State: B=F2, G=D4, Y=E4, P=F4.
- **Tilt Left** (rows independent). Row 2: B: F2→A2→off left edge row 2; Left-2=Blue → B exits (return leg). Row 4 (lead first): G: D4→A4 (no left gate row 4) → stops A4. Y: E4→B4 (packs behind G) → stops B4. P: F4→C4 (packs behind Y) → stops C4.
  - State: G=A4, Y=B4, P=C4 (row 4 restored to start — they only needed to be cleared of B/R interference, which never touched row 4 columns).
- **Tilt Down** (columns independent). Col A: G: A4→A6→off bottom col A; Bottom-A=Green → G exits. Col B: Y: B4→B6→off bottom col B; Bottom-B=Yellow → Y exits. Col C: P: C4→C6→off bottom col C; Bottom-C=Purple → P exits.
  - Board empty → WIN.

**Grand chain:** The row-2 pair (B,R) demands a two-leg ferry: R fires right, B is carried wall-to-wall to F2, then the reverse Left tilt brings B home to its Left-2 gate. Both horizontal tilts also slosh the row-4 trio left-and-right, but because row 4 has no side gates, the trio returns to its original columns — so a final clean Tilt Down rains all three out their matching bottom gates. The mastery insight: the horizontal ferry for B/R is harmless to the vertical trio as long as you finish the ferry before tilting Down.
**Key decision:** Sequence the horizontal ferry (Right→Left) fully BEFORE the vertical exit (Down). A premature Down strands the trio against the bottom while B/R are still mid-ferry, and any stray vertical tilt drags B/R out of row 2 where their only gates live. Recognize the two independent sub-systems and order them.
**P1–P5 check:**
- P1: Five critters in two tidy clusters (row-2 pair, row-4 trio), gates on three walls — structure reads at a glance.
- P2: Board clears across three tilts; the trio's simultaneous bottom-exit is a big finish.
- P3: Every tilt auto-resolves; the trio visibly sloshes and re-packs.
- P4: No new mechanic; SPIKE — the most complex chain, layering a ferry over an untouched vertical trio.
- P5: Strong — identify the two sub-systems and the strict horizontal-before-vertical ordering.

---

## LEVELS 16–20 — GATE SCARCITY + ALL MECHANICS COMBINED (6×6)

### Level 16 — "Shared Lane" — INTRODUCE — Medium
**Grid:** 6×6
**Est. time:** 30–50 sec
**New element:** Gate scarcity — fewer gates than critters. Two same-color critters share ONE exit gate, so one must clear the lane before the other arrives.

```
     A    B    C    D    E    F
1  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]   →G Right-1=Green
2  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
3  [R]  [ ]  [ ]  [ ]  [ ]  [ ]   →R Right-3=Red
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [G]  [ ]  [ ]  [ ]  [ ]  [G]   (both Green — share the single Green gate)
6  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
```
Gates: Right-1=Green, Right-3=Red — only 2 gates for 3 critters (two Greens share Right-1=Green)
Critters: R at A3, G at A5, G at F5 (two green critters, one green gate)

**Solution:** Tilt Right → Tilt Up → Tilt Right.

**Simulation:**
- Start: R=A3, G1=A5, G2=F5.
- **Tilt Right** (rows independent). Row 3: R: A3→F3→off right edge; Right-3=Red → R exits. Row 5 (lead first): G2: F5 at right edge → off right edge row 5 (no right gate row 5) → stays F5. G1: A5→E5 (packs behind G2) → stops E5.
  - State: G1=E5, G2=F5. (Both greens stuck on row 5 — no green gate on row 5. The shared green gate is at Right-1.)
- **Tilt Up** (columns independent). Col E: G1: E5→E1 (no top gate col E) → stops E1. Col F: G2: F5→F1 (no top gate col F) → stops F1.
  - State: G1=E1, G2=F1 (both now on row 1, where the lone green gate lives at Right-1).
- **Tilt Right** (row 1, lead first): G2: F1→off right edge row 1; Right-1=Green → G2 exits. G1: E1→F1→off right edge row 1; Right-1=Green → G1 exits too (follows through the now-open shared lane).
  - Board empty → WIN.

**Gate scarcity introduced:** Two green critters but only one green gate (Right-1). They must line up in the SAME row (row 1) and exit single-file through the shared lane — the lead exits, the follower packs in behind and exits on the same tilt. R, on its own dedicated gate, leaves first to stay out of the way.
**Key decision:** Realize row 5 has no green gate — the two greens must be routed UP to row 1 to share the one green gate. Both greens funnel through a single exit.
**P1–P5 check:**
- P1: Two identical green critters + one green gate signals "they share this exit"; R has its own gate.
- P2: Board clears in three tilts; the two greens exiting the same gate back-to-back is a satisfying funnel.
- P3: Auto-resolves; the follower packs and exits behind the leader in one motion.
- P4: One new idea: more critters than gates → shared exit lane.
- P5: Decision — route both greens to the single shared gate's row.

---

### Level 17 — "Take a Number" — COMBINE — Medium-Hard
**Grid:** 6×6
**Est. time:** 40–65 sec

```
     A    B    C    D    E    F
1  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
2  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
3  [B]  [R]  [R]  [ ]  [ ]  [ ]   ←B Left-3=Blue ; →R Right-3=Red (shared by both Reds)
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
```
Gates: Left-3=Blue, Right-3=Red (shared by both Reds) — 2 gates, 3 critters
Critters: B at A3, R1 at B3, R2 at C3 (B is to the LEFT of the reds, next to its own Left gate)

**Solution:** Tilt Right → Tilt Left.

**Simulation:**
- Start: B=A3, R1=B3, R2=C3.
- **Tilt Right** (row 3, lead first): R2: C3→F3→off right edge row 3; Right-3=Red → R2 exits. R1: B3→F3 (lane clear) → Right-3=Red → R1 exits too (single-file through the shared red gate). B: A3→F3 (lane now fully clear) → Right-3 is Red, B is Blue → wall → B stops at F3 (ferried to the far wall).
  - State: B=F3. (Both reds gone through the one shared red gate; B carried across.)
- **Tilt Left** (row 3): B: F3→A3→off left edge row 3; Left-3=Blue → B exits.
  - Board empty → WIN.

**Combine — blocking + scarcity:** The two reds share the single Right-3 red gate and must exit single-file (scarcity). B sits behind them relative to the right gate, so a Tilt Right clears BOTH reds through their shared lane while ferrying B across to the far wall; the reverse Tilt Left sends B out its own Left-3 gate. If B were on the gate side it would jam the shared lane — placement is the puzzle.
**Key decision:** See that both reds funnel through one gate, and that the single Tilt Right both empties the shared red lane and conveniently ferries B to the opposite wall for a clean Left exit.
**P1–P5 check:**
- P1: Two identical reds + one red gate (shared lane) and a lone blue with its own left gate — readable.
- P2: Three critters out in two tilts; the reds' back-to-back same-gate exit is the payoff.
- P3: Auto-resolves; reds pack and exit single-file, B rides across.
- P4: No new mechanic; COMBINES blocking (L6–8) with scarcity (L16).
- P5: Decision — recognize the shared-lane single-file exit and B's free ferry.

---

### Level 18 — "Funnel & Ferry" — COMBINE — Hard
**Grid:** 6×6
**Est. time:** 55–85 sec

```
     A    B    C    D    E    F
1  [ ]  [ ]  [Y]  [ ]  [ ]  [ ]   ←Y Left-1=Yellow
2  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
3  [ ]  [ ]  [ ]  [ ]  [B]  [R]   ←B Left-3=Blue ; →R Right-3=Red
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [G]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [G]  [ ]  [ ]  [ ]  [ ]  [ ]   ↓G Bottom-A=Green (shared by both Greens)
```
Gates: Left-1=Yellow, Left-3=Blue, Right-3=Red, Bottom-A=Green (shared by both Greens) — 4 gates, 5 critters
Critters: Y at C1, B at E3, R at F3, G1 at A5, G2 at A6

**Solution:** Tilt Right → Tilt Left → Tilt Down.

**Simulation:**
- Start: Y=C1, B=E3, R=F3, G1=A5, G2=A6.
- **Tilt Right** (rows independent). Row 1: Y: C1→F1 (no right gate row 1) → stops F1. Row 3 (lead first): R: F3→off right edge; Right-3=Red → R exits. B: E3→F3 → Right-3 is Red, B is Blue → wall → B stops at F3 (ferried right). Row 5: G1: A5→F5 (no right gate row 5) → stops F5. Row 6: G2: A6→F6 (no right gate row 6) → stops F6.
  - State: Y=F1, B=F3, G1=F5, G2=F6.
- **Tilt Left** (rows independent). Row 1: Y: F1→A1→off left edge row 1; Left-1=Yellow → Y exits. Row 3: B: F3→A3→off left edge row 3; Left-3=Blue → B exits (return leg). Row 5: G1: F5→A5 (no left gate row 5) → stops A5. Row 6: G2: F6→A6 (no left gate row 6) → stops A6.
  - State: G1=A5, G2=A6 (greens, each alone in their row, ride right then back and return to column A).
- **Tilt Down** (column A, lead first): G2: A6→off bottom col A; Bottom-A=Green → G2 exits. G1: A5→A6→off bottom col A; Bottom-A=Green → G1 exits (single-file through the shared green gate).
  - Board empty → WIN.

**Combine — chain + scarcity:** The row-3 ferry pair (B,R) resolves as in L11/L17 — R exits right, B is carried across then comes home left. The two greens SHARE one Bottom-A gate (scarcity) and exit single-file. The mastery beat: the horizontal ferry tilts also sweep the greens right and back, but because each green is alone in its row they return to column A unharmed — and the same Tilt Left that brings B home also drops Y out its Left-1 gate. One round trip resolves four critters; the final Down funnels the green pair.
**Key decision:** Trust that the greens survive the horizontal round trip (they return to column A), and bundle Y's Left-1 exit into the same Left tilt. Do not Tilt Down before the ferry — it strands B/R off row 3.
**P1–P5 check:**
- P1: A ferry pair, a stacked green pair on one shared gate, and a lone yellow — three readable sub-structures.
- P2: Five critters clear in three tilts; the green funnel is the finisher.
- P3: Auto-resolves; greens slosh and re-pack, ferry rides across.
- P4: No new mechanic; COMBINES chain (L11–15) + scarcity (L16–17).
- P5: Strong — sequence horizontal-before-vertical and recognize the greens' safe round trip.

---

### Level 19 — "Gridlock" — SPIKE — Hard
**Grid:** 6×6
**Est. time:** 70–110 sec

```
     A    B    C    D    E    F
1  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
2  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
3  [B]  [R]  [R]  [ ]  [ ]  [ ]   ←B Left-3=Blue ; →R Right-3=Red (shared by both Reds)
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [P]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [P]  [ ]  [ ]  [ ]  [ ]  [ ]   ↓P Bottom-A=Purple (shared by both Purples)
```
Gates: Left-3=Blue, Right-3=Red (shared by both Reds), Bottom-A=Purple (shared by both Purples) — 3 gates, 5 critters
Critters: B at A3, R1 at B3, R2 at C3, P1 at A5, P2 at A6

**Solution:** Tilt Right → Tilt Left → Tilt Down.

**Simulation:**
- Start: B=A3, R1=B3, R2=C3, P1=A5, P2=A6.
- **Tilt Right** (rows independent). Row 3 (lead first): R2: C3→F3→off right edge; Right-3=Red → R2 exits. R1: B3→F3 → Right-3=Red → R1 exits too (single-file through the shared red gate). B: A3→F3 → Right-3 is Red, B is Blue → wall → B stops at F3 (ferried right). Row 5: P1: A5→F5 (no right gate row 5) → stops F5. Row 6: P2: A6→F6 (no right gate row 6) → stops F6.
  - State: B=F3, P1=F5, P2=F6.
- **Tilt Left** (rows independent). Row 3: B: F3→A3→off left edge row 3; Left-3=Blue → B exits. Row 5: P1: F5→A5 (no left gate row 5) → stops A5. Row 6: P2: F6→A6 (no left gate row 6) → stops A6.
  - State: P1=A5, P2=A6 (purples ride out and back, returning to column A).
- **Tilt Down** (column A, lead first): P2: A6→off bottom col A; Bottom-A=Purple → P2 exits. P1: A5→A6→off bottom col A; Bottom-A=Purple → P1 exits (single-file).
  - Board empty → WIN.

**Why it's a spike — double scarcity:** TWO shared lanes at once. Two reds funnel through Right-3, two purples funnel through Bottom-A, and B ferries through the middle. Five critters, only three gates. The single Tilt Right does triple duty: empties the red shared lane, sweeps the purples toward F (a harmless round trip), and ferries B across. The trap is any vertical tilt before the horizontals — it scrambles the reds off row 3 and jams B behind them.
**Key decision:** Read the board as two funnels plus a ferry, all resolved on the horizontal axis first, with a single Down to finish the purple funnel. Hardest level so far because two scarcity systems overlap in column A.
**P1–P5 check:**
- P1: Two reds on a shared gate, two purples on a shared gate, one ferry blue — symmetric and readable.
- P2: Five critters out in three tilts; two funnels firing is a big payoff.
- P3: Auto-resolves; both funnels single-file, ferry rides across.
- P4: No new mechanic; SPIKE via doubled scarcity.
- P5: Strong — recognize the two overlapping funnels and the strict horizontal-first order.

---

### Level 20 — "Breathing Room" — BREATHER — Easy-Medium
**Grid:** 6×6
**Est. time:** 15–25 sec

```
     A    B    C    D    E    F
1  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
2  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
3  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [R]  [G]  [B]  [Y]  [ ]  [ ]   ↓R ↓G ↓B ↓Y (Bottom-A=Red, B=Green, C=Blue, D=Yellow)
```
Gates: Bottom-A=Red, Bottom-B=Green, Bottom-C=Blue, Bottom-D=Yellow — 4 gates, 4 critters (one-to-one)
Critters: R at A6, G at B6, B at C6, Y at D6 (each directly above its own matching bottom gate)

**Solution:** Tilt Down.

**Simulation:**
- Start: R=A6, G=B6, B=C6, Y=D6 (already on the bottom row, each in its gate's column).
- **Tilt Down** (columns independent): R: A6→off bottom col A; Bottom-A=Red → exits. G: B6→Bottom-B=Green → exits. B: C6→Bottom-C=Blue → exits. Y: D6→Bottom-D=Yellow → exits.
  - Board empty → WIN.

**Breather:** After the double-scarcity spike, a one-to-one gate level that solves in a single tilt. Four critters each sit directly on their own matching gate — no scarcity, no ferry, no blocking. Pure pacing relief and a confidence reset before the mastery section.
**Key decision:** Minimal — confirm every critter is column-aligned with its matching gate, then tilt Down once. (A breather may relax P5.)
**P1–P5 check:**
- P1: Four critters on the bottom row, each over its matching gate — Down is unmistakable.
- P2: The whole row drops out at once — clean single-tilt payoff.
- P3: One tilt, four simultaneous exits.
- P4: No new mechanic; deliberate breather.
- P5: Minimal by design (breather pacing).

---

## LEVELS 21–25 — MASTERY (all mechanics, 6×6)

### Level 21 — "Long Way Home" — MASTERY — Hard
**Grid:** 6×6
**Est. time:** 75–115 sec

```
     A    B    C    D    E    F
1  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
2  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
3  [B]  [R]  [R]  [ ]  [ ]  [ ]   ←B Left-3=Blue ; →R Right-3=Red (shared by both Reds)
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [G]  [G]   →G Right-5=Green (shared by both Greens)
6  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
```
Gates: Left-3=Blue, Right-3=Red (shared by both Reds), Right-5=Green (shared by both Greens) — 3 gates, 5 critters
Critters: B at A3, R1 at B3, R2 at C3, G1 at E5, G2 at F5

**Solution:** Tilt Right → Tilt Left.

**Simulation (verified):**
- Start: B=A3, R1=B3, R2=C3, G1=E5, G2=F5.
- **Tilt Right** (rows independent). Row 3 (lead first): R2: C3→F3→Right-3=Red → R2 exits. R1: B3→F3→Right-3=Red → R1 exits (single-file). B: A3→F3 → Right-3 is Red, B is Blue → wall → B stops at F3 (ferried right). Row 5 (lead first): G2: F5→off right edge; Right-5=Green → G2 exits. G1: E5→F5→Right-5=Green → G1 exits (single-file).
  - State: B=F3 (all four reds/greens funneled out their two shared gates; B ferried across).
- **Tilt Left** (row 3): B: F3→A3→off left edge row 3; Left-3=Blue → B exits.
  - Board empty → WIN.

**Mastery — chain + double scarcity, one axis:** Two reds funnel through Right-3, two greens funnel through Right-5, and B ferries across to be brought home by the reverse tilt. The whole five-critter board collapses on the horizontal axis: one Right empties both shared lanes and ferries B; one Left finishes B. The insight is seeing that both funnels point the same direction, so a single Right tilt clears four critters at once.
**Key decision:** Recognize that both shared lanes (reds and greens) exit to the RIGHT, so one Tilt Right fires all four through their funnels while ferrying B; then bring B home Left. Any vertical tilt scrambles the rows and breaks both funnels.
**P1–P5 check:**
- P1: Two same-color pairs each on a shared right gate, plus a lone ferry blue — readable funnels.
- P2: Four funnel exits on one tilt, then B home — big payoff.
- P3: Auto-resolves; both funnels single-file simultaneously.
- P4: No new mechanic; MASTERY — chain + double scarcity sequencing.
- P5: Strong — see the shared exit direction and the ferry's round trip.

---

### Level 22 — "Four Moves Ahead" — MASTERY — Hard
**Grid:** 6×6
**Est. time:** 90–130 sec

```
     A    B    C    D    E    F
1  [Y]  [ ]  [ ]  [ ]  [ ]  [ ]   ←Y Left-1=Yellow
2  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
3  [B]  [R]  [R]  [ ]  [ ]  [ ]   ←B Left-3=Blue ; →R Right-3=Red (shared by both Reds)
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [P]  [ ]  [ ]  [ ]  [ ]  [ ]   ↓P Bottom-A=Purple
```
Gates: Left-1=Yellow, Left-3=Blue, Right-3=Red (shared by both Reds), Bottom-A=Purple — 4 gates, 5 critters
Critters: Y at A1, B at A3, R1 at B3, R2 at C3, P at A6

**Solution:** Tilt Right → Tilt Left → Tilt Down.

**Simulation:**
- Start: Y=A1, B=A3, R1=B3, R2=C3, P=A6.
- **Tilt Right** (rows independent). Row 1: Y: A1→F1 (no right gate row 1) → stops F1. Row 3 (lead first): R2: C3→F3→Right-3=Red → R2 exits. R1: B3→F3→Right-3=Red → R1 exits (single-file). B: A3→F3 → wall (Red gate, B is Blue) → B stops at F3. Row 6: P: A6→F6 (no right gate row 6) → stops F6.
  - State: Y=F1, B=F3, P=F6.
- **Tilt Left** (rows independent). Row 1: Y: F1→A1→off left edge row 1; Left-1=Yellow → Y exits. Row 3: B: F3→A3→off left edge row 3; Left-3=Blue → B exits. Row 6: P: F6→A6 (no left gate row 6) → stops A6.
  - State: P=A6 (Y and B both exit their left gates on the same tilt; P rides back to column A).
- **Tilt Down** (column A): P: A6→off bottom col A; Bottom-A=Purple → P exits.
  - Board empty → WIN.

**Mastery — blocking + chain + scarcity, plan 4+ ahead:** The two reds share Right-3 (scarcity); B must ferry right-then-left (chain); Y exits left on the return tilt; P rides the round trip back to column A and drops out the bottom. Five critters, but the player must foresee that one Right + one Left resolves four of them (two reds funneled, B and Y out their left gates) before the finishing Down. Plan the full Right→Left→Down arc before committing.
**Key decision:** See that the Tilt Left does triple duty — Y out Left-1, B out Left-3, P repositioned to column A — then a single Down finishes P. Mis-ordering (Down first) strands B/R off row 3 and parks P against the bottom early.
**P1–P5 check:**
- P1: A column-A stack (Y/B/P) plus the red funnel — relationships visible.
- P2: Five critters out in three tilts; the Left tilt's double exit is satisfying.
- P3: Auto-resolves; funnel, ferries, and repack all in motion.
- P4: No new mechanic; MASTERY — blocking + chain + scarcity together.
- P5: Strong — requires planning the whole arc before the first tilt.

---

### Level 23 — "Total Gridlock" — MASTERY — Very Hard
**Grid:** 6×6
**Est. time:** 110–160 sec

```
     A    B    C    D    E    F
1  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
2  [Y]  [ ]  [ ]  [ ]  [ ]  [G]   ←Y Left-2=Yellow ; →G Right-2=Green
3  [B]  [R]  [R]  [ ]  [ ]  [ ]   ←B Left-3=Blue ; →R Right-3=Red (shared by both Reds)
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [P]  [ ]  [ ]  [ ]  [ ]  [ ]   ↓P Bottom-A=Purple
```
Gates: Left-2=Yellow, Right-2=Green, Left-3=Blue, Right-3=Red (shared by both Reds), Bottom-A=Purple — 5 gates, 6 critters (two reds share Right-3)
Critters: Y at A2, G at F2, B at A3, R1 at B3, R2 at C3, P at A6

**Solution:** Tilt Right → Tilt Left → Tilt Down.

**Simulation (verified):**
- Start: Y=A2, G=F2, B=A3, R1=B3, R2=C3, P=A6.
- **Tilt Right** (rows independent). Row 2 (lead first): G: F2→off right edge; Right-2=Green → G exits. Y: A2→F2 → Right-2 is Green, Y is Yellow → wall → Y stops at F2 (ferried right). Row 3 (lead first): R2: C3→F3→Right-3=Red → R2 exits. R1: B3→F3→Right-3=Red → R1 exits (single-file). B: A3→F3 → wall (Red gate) → B stops at F3 (ferried right). Row 6: P: A6→F6 (no right gate row 6) → stops F6.
  - State: Y=F2, B=F3, P=F6.
- **Tilt Left** (rows independent). Row 2: Y: F2→A2→off left edge row 2; Left-2=Yellow → Y exits. Row 3: B: F3→A3→off left edge row 3; Left-3=Blue → B exits. Row 6: P: F6→A6 (no left gate row 6) → stops A6.
  - State: P=A6.
- **Tilt Down** (column A): P: A6→off bottom col A; Bottom-A=Purple → P exits.
  - Board empty → WIN.

**Mastery — maximum complexity:** Six critters, five gates. Two ferry pairs (Y/G on row 2, B/R-funnel on row 3) plus a lone purple. The single Tilt Right fires G and both reds out their right gates while ferrying Y and B across; the Tilt Left brings Y and B home out their left gates; the Down finishes P. The board looks overwhelming but collapses on the horizontal axis in two tilts plus a cleanup.
**Key decision:** Spot the symmetry — two independent ferry rows resolving with the same Right→Left pair, plus P's free round trip to column A. Any premature vertical tilt scrambles both ferry rows.
**P1–P5 check:**
- P1: Two ferry rows + a lone purple; six critters but grouped and readable.
- P2: The most critters cleared yet — four exit on the first tilt.
- P3: Auto-resolves; two ferry rows and the red funnel all in motion.
- P4: No new mechanic; MASTERY — peak complexity combining every element.
- P5: Very strong — read the whole board as two ferries + a funnel + a cleanup.

---

### Level 24 — "The Elegant Knot" — MASTERY — Hard
**Grid:** 6×6
**Est. time:** 60–95 sec (looks much harder than it is)

```
     A    B    C    D    E    F
1  [R]  [ ]  [ ]  [ ]  [ ]  [B]   ↑R Top-A=Red ; ↑B Top-F=Blue
2  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
3  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
5  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [G]  [Y]  [ ]  [ ]  [ ]  [P]   ↓G ↓Y ↓P (Bottom-A=Green, Bottom-B=Yellow, Bottom-F=Purple)
```
Gates: Top-A=Red, Top-F=Blue, Bottom-A=Green, Bottom-B=Yellow, Bottom-F=Purple — 5 gates, 5 critters (one-to-one)
Critters: R at A1, B at F1, G at A6, Y at B6, P at F6

**Solution:** Tilt Up → Tilt Down.

**Simulation:**
- Start: R=A1, B=F1, G=A6, Y=B6, P=F6.
- **Tilt Up** (columns independent). Col A (lead first): R: A1→off top col A; Top-A=Red → R exits. G: A6→A1 → Top-A is Red, G is Green → wall → G stops at A1. Col B: Y: B6→B1 (no top gate col B) → stops B1. Col F (lead first): B: F1→off top col F; Top-F=Blue → B exits. P: F6→F1 → Top-F is Blue, P is Purple → wall → P stops at F1.
  - State: G=A1, Y=B1, P=F1.
- **Tilt Down** (columns independent). Col A: G: A1→A6→off bottom col A; Bottom-A=Green → G exits. Col B: Y: B1→B6→off bottom col B; Bottom-B=Yellow → Y exits. Col F: P: F1→F6→off bottom col F; Bottom-F=Purple → P exits.
  - Board empty → WIN.

**Mastery — elegant two-tilt solution:** Five critters, five gates on opposite walls — looks like a tangle, but the whole board solves in just Up then Down. Each column is a self-contained vertical pair (or single) with a top gate and a bottom gate: Up fires the top-gate critters and flips the bottom ones to the top wall; Down sends those out the bottom. Column B's lone Yellow simply rides up and back to its bottom gate. The insight: ignore the horizontal axis entirely.
**Key decision:** Resist over-thinking. Recognize every critter's gate lies on the vertical axis of its own column, so two tilts (Up, Down) clear everything. Any horizontal tilt tangles the columns and ruins the symmetry.
**P1–P5 check:**
- P1: Critters hug the top and bottom walls with matching gates above/below — vertical solution cued.
- P2: Whole board clears in two big symmetric tilts.
- P3: Auto-resolves; tops exit, bottoms flip and exit.
- P4: No new mechanic; MASTERY — rewards the elegant-insight read.
- P5: Strong-but-clean — the decision is recognizing the trap-free vertical solution.

---

### Level 25 — "Final Gridlock" — MASTERY — Very Hard
**Grid:** 6×6
**Est. time:** 120–180 sec (the final boss)

```
     A    B    C    D    E    F
1  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
2  [B]  [R]  [R]  [ ]  [ ]  [ ]   ←B Left-2=Blue ; →R Right-2=Red (shared by both Reds)
3  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
4  [ ]  [ ]  [ ]  [ ]  [ ]  [Y]   →Y Right-4=Yellow
5  [ ]  [ ]  [ ]  [ ]  [ ]  [ ]
6  [P]  [ ]  [ ]  [ ]  [ ]  [ ]   ↓P Bottom-A=Purple
```
Gates: Left-2=Blue, Right-2=Red (shared by both Reds), Right-4=Yellow, Bottom-A=Purple — 4 gates, 5 critters (two reds share Right-2)
Critters: B at A2, R1 at B2, R2 at C2, Y at F4, P at A6

**Solution:** Tilt Right → Tilt Left → Tilt Down.

**Simulation (verified):**
- Start: B=A2, R1=B2, R2=C2, Y=F4, P=A6.
- **Tilt Right** (rows independent). Row 2 (lead first): R2: C2→F2→Right-2=Red → R2 exits. R1: B2→F2→Right-2=Red → R1 exits (single-file through the shared red gate). B: A2→F2 → Right-2 is Red, B is Blue → wall → B stops at F2 (ferried right). Row 4: Y: F4 at right edge → off right edge row 4; Right-4=Yellow → Y exits. Row 6: P: A6→F6 (no right gate row 6) → stops F6.
  - State: B=F2, P=F6 (both reds out the shared red gate, Y out its own right gate, B ferried, P swept right).
- **Tilt Left** (rows independent). Row 2: B: F2→A2→off left edge row 2; Left-2=Blue → B exits. Row 6: P: F6→A6 (no left gate row 6) → stops A6.
  - State: P=A6.
- **Tilt Down** (column A): P: A6→off bottom col A; Bottom-A=Purple → P exits.
  - Board empty → WIN.

**Final boss — grand synthesis:** Every mechanic in one board. Scarcity: two reds funnel single-file through one shared Right-2 gate. Blocking: B sits behind the reds and cannot pass the red gate. Chain/ferry: B is carried to the far wall by the Right tilt, then home by the Left tilt. Dedicated exit: Y leaves on its own right gate in the same first tilt. Round-trip survivor: P is swept right and back to column A, then dropped out the bottom. The whole five-critter, four-gate tangle collapses to Right → Left → Down once the player sees that the first Right tilt resolves three critters at once.
**Key decision:** The grand read — one Tilt Right fires both reds (shared lane) AND Y (own gate) AND ferries B AND sweeps P, all simultaneously. Then Left brings B home; Down finishes P. Every wrong instinct (vertical first, or trying to move reds left) breaks a funnel or strands the ferry. The culmination of all 24 prior lessons.
**P1–P5 check:**
- P1: Red funnel, lone blue ferry, lone yellow, lone purple — busy but each relationship is legible.
- P2: Three critters exit on the very first tilt — the biggest single-tilt payoff in the set, then a clean finish.
- P3: Auto-resolves; funnel, dedicated exit, ferry, and sweep all animate in one tilt.
- P4: No new mechanic; MASTERY — the grand synthesis of scarcity + blocking + chain + dedicated exits.
- P5: Maximum — the player must read four interacting sub-systems and commit to the full Right→Left→Down arc.

---
## Level Summary

| # | Name | Tag | Difficulty | Mechanic |
|---|------|-----|-----------|---------|
| 1 | First Slide | INTRODUCE | Easy | Core: slide + match-exit |
| 2 | Drop Down | INTRODUCE | Easy | Vertical axis (Up/Down) |
| 3 | Tag Along | REINFORCE | Easy | Multiple critters, parallel rows |
| 4 | Two Directions | REINFORCE | Easy-Medium | Two-wall sequencing |
| 5 | Three Steps | REINFORCE | Medium | Order dependency |
| 6 | After You | INTRODUCE | Medium | Blocking (shared lane, opposite gates) |
| 7 | Step Aside | REINFORCE | Medium | Remove a blocker |
| 8 | Right Order | COMBINE | Medium | Blocking + ordering |
| 9 | Tangle | SPIKE | Hard | Doubled blocking (co-lane pairs) |
| 10 | Easy Does It | BREATHER | Easy-Medium | One-tilt column clear |
| 11 | Pass Through | INTRODUCE | Medium | Chain (intermediate ferry) |
| 12 | One Then Two | REINFORCE | Medium-Hard | Two-step chain + vertical cleanup |
| 13 | Hidden Link | SPIKE | Hard | Three-step chain, hidden dependency |
| 14 | Round Trip | REINFORCE | Medium-Hard | Two-leg ferry |
| 15 | Grand Chain | SPIKE | Hard | Ferry over an untouched vertical trio |
| 16 | Shared Lane | INTRODUCE | Medium | Gate scarcity (shared exit) |
| 17 | Take a Number | COMBINE | Medium-Hard | Blocking + scarcity |
| 18 | Funnel & Ferry | COMBINE | Hard | Chain + scarcity |
| 19 | Gridlock | SPIKE | Hard | Double scarcity (two funnels) |
| 20 | Breathing Room | BREATHER | Easy-Medium | One-to-one single-tilt clear |
| 21 | Long Way Home | MASTERY | Hard | Chain + double scarcity, one axis |
| 22 | Four Moves Ahead | MASTERY | Hard | Blocking + chain + scarcity |
| 23 | Total Gridlock | MASTERY | Very Hard | Two ferries + funnel + cleanup |
| 24 | The Elegant Knot | MASTERY | Hard | Elegant two-tilt vertical insight |
| 25 | Final Gridlock | MASTERY | Very Hard | Grand synthesis of all mechanics |

## Principle Coverage
| Principle | All levels | Notes |
|-----------|-----------|-------|
| P1: Visual clarity | ✅ all 25 | Every level reads its goal at load — critters, matching-color gates, and structural clusters (pairs, funnels, ferries) are visually distinct. Shared-lane gates use identical-color critter pairs to signal "share this exit." |
| P2: Payoff | ✅ all 25 | Every level ends in an empty board; cluttered before-state → clean after-state. Payoff scales with progression (single exit → row clears → multi-critter funnels firing on one tilt at L19/L23/L25). |
| P3: Smooth resolve | ✅ all 25 | All motion is simultaneous-slide with auto-packing; no level needs timing or precision. Followers pack behind leaders and funnel single-file automatically. Frame budget per tilt 0.3–0.8s. |
| P4: Progressive complexity | ✅ all 25 | Strict ladder: core (1–5) → blocking (6–10) → chains (11–15) → scarcity (16–20) → mastery (21–25). One new element per INTRODUCE; SPIKEs (9,13,15,19) always followed by easier pacing; BREATHERs at 10 and 20. No level introduces two mechanics at once. |
| P5: Decision-making | ✅ all 25 | Every non-breather level has a genuine ordering/routing decision with a visible-on-reflection wrong path (recoverable via Undo). Hidden dependencies (L13), forced round trips (L14/L15), and overlapping funnels (L19/L23/L25) force planning ahead. Breathers (10, 20) intentionally relax P5 for pacing. |

