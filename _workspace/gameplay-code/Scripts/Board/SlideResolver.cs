using System.Collections.Generic;
using UnityEngine;

/// <summary>One resolved critter movement produced by the slide resolver.</summary>
public struct SlideMove
{
    public CritterPiece critter;
    public Vector2Int   from;
    public Vector2Int   to;        // final cell (valid only when exits == false)
    public bool         exits;
    public CritterColor exitColor; // color of the gate the critter exited through

    public SlideMove(CritterPiece critter, Vector2Int from, Vector2Int to, bool exits, CritterColor exitColor)
    {
        this.critter   = critter;
        this.from      = from;
        this.to        = to;
        this.exits     = exits;
        this.exitColor = exitColor;
    }
}

/// <summary>
/// Pure deterministic tilt resolver. Processes critters from the destination wall inward so that
/// each critter packs against already-settled state, matching critters exit through gates,
/// blockers/critters/non-matching gates stop slides, and arrow tiles redirect mid-slide.
/// </summary>
public class SlideResolver
{
    private GateController _gates;
    private Dictionary<Vector2Int, ArrowRedirector> _arrows;

    /// <summary>Supplies the gate registry and arrow lookup the resolver needs. Call once per level.</summary>
    public void Configure(GateController gates, Dictionary<Vector2Int, ArrowRedirector> arrows)
    {
        _gates = gates;
        _arrows = arrows ?? new Dictionary<Vector2Int, ArrowRedirector>();
    }

    /// <summary>
    /// Resolves a tilt over the given logical grid and critter lookup.
    /// grid: CellState per cell (Critter cells hold a CritterPiece via critterAt).
    /// Returns the list of resulting moves (only critters that actually move or exit).
    /// </summary>
    public List<SlideMove> ResolveMoves(
        TiltDirection dir,
        CellState[,] grid,
        Dictionary<Vector2Int, CritterPiece> critterAt)
    {
        var moves = new List<SlideMove>();
        if (grid == null || critterAt == null) return moves;

        int cols = grid.GetLength(0);
        int rows = grid.GetLength(1);

        // Work on a mutable copy so we can mark cells freed/occupied as we settle each critter.
        var occupied = new Dictionary<Vector2Int, CritterPiece>(critterAt);

        // Build processing order: critters nearest the destination wall first.
        var order = new List<CritterPiece>(critterAt.Values);
        order.Sort((a, b) => CompareForOrder(a.GridPosition, b.GridPosition, dir));

        foreach (var critter in order)
        {
            Vector2Int from = critter.GridPosition;
            // Skip if this critter was already removed (shouldn't happen, but defensive).
            if (!occupied.ContainsKey(from)) continue;

            bool exits;
            CritterColor exitColor;
            Vector2Int finalCell = Slide(critter, from, dir, cols, rows, occupied, out exits, out exitColor);

            if (exits)
            {
                occupied.Remove(from);
                moves.Add(new SlideMove(critter, from, finalCell, true, exitColor));
            }
            else if (finalCell != from)
            {
                occupied.Remove(from);
                occupied[finalCell] = critter;
                moves.Add(new SlideMove(critter, from, finalCell, false, default));
            }
            // else: no movement — emit nothing.
        }

        return moves;
    }

    /// <summary>
    /// Slides one critter from `from` in direction `dir` until it stops or exits.
    /// Honors arrow redirects (with a loop guard) and matching-gate exits.
    /// </summary>
    private Vector2Int Slide(
        CritterPiece critter,
        Vector2Int from,
        TiltDirection dir,
        int cols,
        int rows,
        Dictionary<Vector2Int, CritterPiece> occupied,
        out bool exits,
        out CritterColor exitColor)
    {
        exits = false;
        exitColor = default;

        Vector2Int current = from;
        TiltDirection moveDir = dir;
        Vector2Int delta = TiltMath.Delta(moveDir);

        // Guard against arrow ping-pong: a cell may only redirect this critter once per resolve.
        var redirectedCells = new HashSet<Vector2Int>();

        // Safety bound on iterations.
        int maxSteps = (cols + rows) * 4;
        int steps = 0;

        while (steps++ < maxSteps)
        {
            Vector2Int next = current + delta;

            bool insideX = next.x >= 0 && next.x < cols;
            bool insideY = next.y >= 0 && next.y < rows;

            if (insideX && insideY)
            {
                // Blocked by an already-settled critter (other than itself).
                if (occupied.TryGetValue(next, out var other) && other != critter)
                    break; // stop at current

                // Move into the free cell.
                current = next;

                // If this cell is an arrow tile, try to redirect.
                if (_arrows != null && _arrows.TryGetValue(current, out var arrow))
                {
                    TiltDirection newDir = arrow.RedirectDirection;
                    Vector2Int newDelta = TiltMath.Delta(newDir);

                    // Loop guard: if the redirect would push the critter back onto this same arrow
                    // tile (or we've already used this tile), stop on the tile instead.
                    if (redirectedCells.Contains(current))
                        break;

                    redirectedCells.Add(current);
                    moveDir = newDir;
                    delta = newDelta;
                }

                continue;
            }
            else
            {
                // `next` is off-board: we're at the wall on the current move direction.
                WallSide wall = TiltMath.DestinationWall(moveDir);
                if (_gates != null && _gates.CanExit(current, wall, critter.Color))
                {
                    exits = true;
                    exitColor = critter.Color;
                }
                // else: solid wall or non-matching gate → stop at current cell.
                break;
            }
        }

        return current;
    }

    /// <summary>Orders two cells so the one nearer the destination wall sorts first.</summary>
    private static int CompareForOrder(Vector2Int a, Vector2Int b, TiltDirection dir)
    {
        switch (dir)
        {
            case TiltDirection.Right: return b.x.CompareTo(a.x); // descending x
            case TiltDirection.Left:  return a.x.CompareTo(b.x); // ascending x
            case TiltDirection.Down:  return b.y.CompareTo(a.y); // descending y
            case TiltDirection.Up:    return a.y.CompareTo(b.y); // ascending y
            default:                  return 0;
        }
    }
}
