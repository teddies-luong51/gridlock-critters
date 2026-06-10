using UnityEngine;

/// <summary>Marks a cell as permanently impassable; carries no behaviour beyond identifying its cell type.</summary>
public class StaticBlocker : MonoBehaviour
{
    [SerializeField] private Vector2Int cell;

    /// <summary>The grid cell this blocker occupies.</summary>
    public Vector2Int Cell => cell;

    /// <summary>The cell type this object represents for the resolver.</summary>
    public CellState CellType => CellState.StaticBlocker;

    /// <summary>Assigns the grid cell (called by LevelManager during spawn).</summary>
    public void SetCell(Vector2Int c) => cell = c;
}
