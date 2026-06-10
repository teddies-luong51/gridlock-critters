using UnityEngine;

/// <summary>Shared enums and data structs used across the Gridlock Critters core loop.</summary>
public enum TiltDirection { Left, Right, Up, Down }

/// <summary>Color identity shared by critters and gates; a critter exits only through a matching color.</summary>
public enum CritterColor { Red, Blue, Green, Yellow, Purple, Orange }

/// <summary>Logical contents of a single board cell, used by the slide resolver.</summary>
public enum CellState { Empty, Critter, StaticBlocker, ArrowRedirector, Gate }

/// <summary>Which wall a gate lives on.</summary>
public enum WallSide { Top, Bottom, Left, Right }

/// <summary>Static helper mapping tilt directions to grid coordinate deltas. Top-left = (0,0), +y goes down.</summary>
public static class TiltMath
{
    /// <summary>Returns the (dx, dy) grid delta for a tilt direction. +x = right, +y = down (row 1 = y0).</summary>
    public static Vector2Int Delta(TiltDirection dir)
    {
        switch (dir)
        {
            case TiltDirection.Left:  return new Vector2Int(-1, 0);
            case TiltDirection.Right: return new Vector2Int(1, 0);
            case TiltDirection.Up:    return new Vector2Int(0, -1);
            case TiltDirection.Down:  return new Vector2Int(0, 1);
            default:                  return Vector2Int.zero;
        }
    }

    /// <summary>The wall a critter would exit through when sliding in the given direction.</summary>
    public static WallSide DestinationWall(TiltDirection dir)
    {
        switch (dir)
        {
            case TiltDirection.Left:  return WallSide.Left;
            case TiltDirection.Right: return WallSide.Right;
            case TiltDirection.Up:    return WallSide.Top;
            case TiltDirection.Down:  return WallSide.Bottom;
            default:                  return WallSide.Top;
        }
    }
}
