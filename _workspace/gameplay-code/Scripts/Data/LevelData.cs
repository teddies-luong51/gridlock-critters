using System;
using UnityEngine;

/// <summary>ScriptableObject describing a single level: grid size plus critter/gate/blocker/arrow placements.</summary>
[CreateAssetMenu(fileName = "Level_", menuName = "Gridlock Critters/Level Data", order = 0)]
public class LevelData : ScriptableObject
{
    [Header("Identity")]
    public int levelIndex;

    [Header("Grid")]
    [Min(1)] public int gridWidth  = 5;
    [Min(1)] public int gridHeight = 5;

    [Header("Contents")]
    public CritterSpawn[] critters = Array.Empty<CritterSpawn>();
    public GateData[]     gates    = Array.Empty<GateData>();
    public BlockerData[]  blockers = Array.Empty<BlockerData>();
    public ArrowData[]    arrows   = Array.Empty<ArrowData>();

    /// <summary>A critter to spawn at a given cell with a given color.</summary>
    [Serializable]
    public struct CritterSpawn
    {
        public CritterColor color;
        public Vector2Int   cell;
    }

    /// <summary>A colored gate opening on a wall, addressed by cell and wall side.</summary>
    [Serializable]
    public struct GateData
    {
        public CritterColor color;
        public Vector2Int   cell;   // the on-board cell adjacent to this gate
        public WallSide     wall;
    }

    /// <summary>An impassable static blocker occupying a cell.</summary>
    [Serializable]
    public struct BlockerData
    {
        public Vector2Int cell;
    }

    /// <summary>An arrow redirector tile that changes a critter's slide direction when it lands there.</summary>
    [Serializable]
    public struct ArrowData
    {
        public Vector2Int   cell;
        public TiltDirection direction;
    }
}
