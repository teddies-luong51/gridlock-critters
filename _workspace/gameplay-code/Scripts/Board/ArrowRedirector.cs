using UnityEngine;

/// <summary>An arrow tile that redirects a critter's slide direction when the critter passes onto its cell.</summary>
public class ArrowRedirector : MonoBehaviour
{
    [SerializeField] private Vector2Int cell;
    [SerializeField] private TiltDirection redirectDirection;

    /// <summary>The grid cell this arrow occupies.</summary>
    public Vector2Int Cell => cell;

    /// <summary>The new direction a critter takes after landing on this tile.</summary>
    public TiltDirection RedirectDirection => redirectDirection;

    /// <summary>The cell type this object represents for the resolver.</summary>
    public CellState CellType => CellState.ArrowRedirector;

    /// <summary>Assigns the grid cell and direction (called by LevelManager during spawn).</summary>
    public void Configure(Vector2Int c, TiltDirection dir)
    {
        cell = c;
        redirectDirection = dir;
    }
}
