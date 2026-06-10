using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Owns the logical board and runs the tilt pipeline: board lean -> resolve moves -> animate slides
/// simultaneously -> exits -> win check. Blocks input while animating.
/// </summary>
public class BoardController : MonoBehaviour
{
    public static BoardController Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private BoardTiltAnimator tiltAnimator;
    [SerializeField] private GateController     gateController;
    [SerializeField] private Transform          boardOrigin;     // world anchor for cell (0,0)

    [Header("Layout")]
    [SerializeField] private float cellSize = 1f;

    [Header("Slide")]
    [Tooltip("Cells per second (spec: 9). World speed = this * cellSize.")]
    [SerializeField] private float critterSlideCellsPerSecond = 9f;
    [SerializeField] private float slideLeadDelay = 0.05f; // lean leads, then critters move

    /// <summary>Fired after every fully-resolved tilt (animations complete), passing whether the level is won.</summary>
    public event Action<bool> OnTiltResolved;

    private int _cols, _rows;
    private CellState[,] _grid;
    private readonly Dictionary<Vector2Int, CritterPiece>    _critterAt = new Dictionary<Vector2Int, CritterPiece>();
    private readonly Dictionary<Vector2Int, ArrowRedirector> _arrowAt   = new Dictionary<Vector2Int, ArrowRedirector>();
    private readonly Dictionary<Vector2Int, StaticBlocker>   _blockerAt = new Dictionary<Vector2Int, StaticBlocker>();

    private readonly SlideResolver _resolver = new SlideResolver();
    private readonly List<CritterPiece> _activeCritters = new List<CritterPiece>();

    private bool _isAnimating;

    public bool IsAnimating => _isAnimating;
    public int Cols => _cols;
    public int Rows => _rows;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (TiltInputHandler.Instance != null)
            TiltInputHandler.Instance.OnTilt += ExecuteTilt;
    }

    private void Start()
    {
        // Bind here too in case the input handler awoke after this controller's OnEnable.
        if (TiltInputHandler.Instance != null)
        {
            TiltInputHandler.Instance.OnTilt -= ExecuteTilt;
            TiltInputHandler.Instance.OnTilt += ExecuteTilt;
        }
    }

    private void OnDisable()
    {
        if (TiltInputHandler.Instance != null)
            TiltInputHandler.Instance.OnTilt -= ExecuteTilt;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// (Re)builds the logical board from spawned objects. Called by LevelManager after instantiation.
    /// </summary>
    public void BuildBoard(
        int cols, int rows,
        IEnumerable<CritterPiece> critters,
        IEnumerable<ArrowRedirector> arrows,
        IEnumerable<StaticBlocker> blockers,
        GateController gates)
    {
        _cols = cols;
        _rows = rows;
        _grid = new CellState[cols, rows];

        _critterAt.Clear();
        _arrowAt.Clear();
        _blockerAt.Clear();
        _activeCritters.Clear();

        if (gates != null) gateController = gates;

        if (blockers != null)
            foreach (var b in blockers)
            {
                _blockerAt[b.Cell] = b;
                SetCell(b.Cell, CellState.StaticBlocker);
            }

        if (arrows != null)
            foreach (var a in arrows)
            {
                _arrowAt[a.Cell] = a;
                // Arrow underlays a cell; only mark if not already occupied by a blocker.
                if (GetCell(a.Cell) == CellState.Empty)
                    SetCell(a.Cell, CellState.ArrowRedirector);
            }

        if (critters != null)
            foreach (var c in critters)
            {
                _critterAt[c.GridPosition] = c;
                _activeCritters.Add(c);
                SetCell(c.GridPosition, CellState.Critter);
            }

        _resolver.Configure(gateController, _arrowAt);
        _isAnimating = false;
    }

    /// <summary>Converts a grid cell to its world position using the board origin and cell size.</summary>
    public Vector3 CellToWorld(Vector2Int cell)
    {
        Vector3 origin = boardOrigin != null ? boardOrigin.position : Vector3.zero;
        // +x = right (east), grid +y = down (toward near wall) => world -z.
        float wx = origin.x + cell.x * cellSize;
        float wz = origin.z - cell.y * cellSize;
        float wy = origin.y;
        return new Vector3(wx, wy, wz);
    }

    /// <summary>The off-board world position a critter slides to when exiting in a direction.</summary>
    public Vector3 ExitWorld(Vector2Int fromCell, TiltDirection dir)
    {
        Vector2Int d = TiltMath.Delta(dir);
        Vector2Int beyond = fromCell + d; // one cell past the wall
        return CellToWorld(beyond);
    }

    /// <summary>Entry point for a tilt (from accelerometer or editor input). No-ops while animating.</summary>
    public void ExecuteTilt(TiltDirection dir)
    {
        if (_isAnimating) return;
        if (_grid == null) return;
        StartCoroutine(TiltRoutine(dir));
    }

    private IEnumerator TiltRoutine(TiltDirection dir)
    {
        _isAnimating = true;

        // 1. Board lean (runs alongside the slide; lean leads slightly).
        Coroutine lean = tiltAnimator != null ? StartCoroutine(tiltAnimator.PlayTilt(dir)) : null;

        if (slideLeadDelay > 0f) yield return new WaitForSeconds(slideLeadDelay);

        // 2. Resolve all moves against the current logical grid.
        List<SlideMove> moves = _resolver.ResolveMoves(dir, _grid, _critterAt);

        // 3. Apply logical state + animate slides simultaneously.
        yield return AnimateMoves(moves, dir);

        // Wait for the board lean to finish returning before unlocking.
        if (lean != null) yield return lean;

        // 4. Win check.
        bool won = CheckWin();
        _isAnimating = false;

        OnTiltResolved?.Invoke(won);
    }

    private IEnumerator AnimateMoves(List<SlideMove> moves, TiltDirection dir)
    {
        if (moves == null || moves.Count == 0) yield break;

        float worldSpeed = critterSlideCellsPerSecond * cellSize;
        var running = new List<Coroutine>();

        // First update logical grid so concurrent animations read consistent state.
        foreach (var m in moves)
        {
            SetCell(m.from, CellState.Empty);
            // Restore arrow underlay if an arrow lived on the vacated cell.
            if (_arrowAt.ContainsKey(m.from)) SetCell(m.from, CellState.ArrowRedirector);

            if (m.exits)
            {
                _critterAt.Remove(m.from);
            }
            else
            {
                _critterAt.Remove(m.from);
                _critterAt[m.to] = m.critter;
                m.critter.SetGridPosition(m.to);
                SetCell(m.to, CellState.Critter);
            }
        }

        // Then launch animations together.
        foreach (var m in moves)
        {
            if (m.exits)
                running.Add(StartCoroutine(ExitRoutine(m, dir, worldSpeed)));
            else
                running.Add(StartCoroutine(m.critter.SlideTo(CellToWorld(m.to), worldSpeed)));
        }

        foreach (var c in running)
            yield return c;
    }

    private IEnumerator ExitRoutine(SlideMove m, TiltDirection dir, float worldSpeed)
    {
        // Slide to the wall cell the critter exited from, then through the gate.
        yield return m.critter.SlideTo(CellToWorld(m.from), worldSpeed);
        yield return m.critter.SlideTo(ExitWorld(m.from, dir), worldSpeed);
        yield return m.critter.PlayExitAnimation();
        _activeCritters.Remove(m.critter);
    }

    private bool CheckWin() => _critterAt.Count == 0;

    private void SetCell(Vector2Int cell, CellState state)
    {
        if (InBounds(cell)) _grid[cell.x, cell.y] = state;
    }

    private CellState GetCell(Vector2Int cell) => InBounds(cell) ? _grid[cell.x, cell.y] : CellState.StaticBlocker;

    private bool InBounds(Vector2Int cell) =>
        cell.x >= 0 && cell.x < _cols && cell.y >= 0 && cell.y < _rows;
}
