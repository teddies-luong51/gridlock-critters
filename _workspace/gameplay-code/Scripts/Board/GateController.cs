using System.Collections.Generic;
using UnityEngine;

/// <summary>Registry of all gates on the board; answers whether a cell+wall has a gate of a given color.</summary>
public class GateController : MonoBehaviour
{
    /// <summary>One gate opening: color plus open state. Non-matching colors act as solid walls.</summary>
    public struct GateInfo
    {
        public bool         Exists;
        public CritterColor Color;
        public bool         IsOpen;
    }

    // Keyed by (cell, wall) so two walls of one corner cell can each carry a distinct gate.
    private readonly Dictionary<(Vector2Int cell, WallSide wall), GateInfo> _gates =
        new Dictionary<(Vector2Int, WallSide), GateInfo>();

    /// <summary>Clears all registered gates (called on level load).</summary>
    public void Clear() => _gates.Clear();

    /// <summary>Registers a gate at the given cell on the given wall.</summary>
    public void RegisterGate(Vector2Int cell, WallSide wall, CritterColor color, bool isOpen = true)
    {
        _gates[(cell, wall)] = new GateInfo { Exists = true, Color = color, IsOpen = isOpen };
    }

    /// <summary>Returns the gate at a cell on a specific wall, if any.</summary>
    public GateInfo GetGateAt(Vector2Int cell, WallSide wall)
    {
        if (_gates.TryGetValue((cell, wall), out var info)) return info;
        return new GateInfo { Exists = false };
    }

    /// <summary>True if a critter of the given color may pass through a gate at cell on wall (matching + open).</summary>
    public bool CanExit(Vector2Int cell, WallSide wall, CritterColor color)
    {
        var info = GetGateAt(cell, wall);
        return info.Exists && info.IsOpen && info.Color == color;
    }
}
