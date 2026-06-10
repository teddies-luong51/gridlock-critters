using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Loads LevelData ScriptableObjects and instantiates the board, critters, gates, blockers and arrows.</summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Catalog")]
    [SerializeField] private LevelData[] levels;

    [Header("Prefabs")]
    [SerializeField] private CritterPiece     critterPrefab;
    [SerializeField] private StaticBlocker    blockerPrefab;
    [SerializeField] private ArrowRedirector  arrowPrefab;
    [SerializeField] private GameObject       gateVisualPrefab; // optional cosmetic gate

    [Header("Refs")]
    [SerializeField] private BoardController board;
    [SerializeField] private GateController  gateController;
    [SerializeField] private Transform       spawnRoot;

    [Header("Color Tints (optional, indexed by CritterColor)")]
    [SerializeField] private Color[] colorTints;

    /// <summary>Fired when the active level reports all critters exited.</summary>
    public event Action OnLevelComplete;

    public int CurrentLevelIndex { get; private set; } = -1;

    private readonly List<GameObject> _spawned = new List<GameObject>();

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
        if (board != null) board.OnTiltResolved += HandleTiltResolved;
    }

    private void OnDisable()
    {
        if (board != null) board.OnTiltResolved -= HandleTiltResolved;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void HandleTiltResolved(bool won)
    {
        if (won) OnLevelComplete?.Invoke();
    }

    /// <summary>Tears down any current level and instantiates the level at the given catalog index.</summary>
    public void LoadLevel(int levelIndex)
    {
        if (levels == null || levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError($"[LevelManager] Invalid level index {levelIndex}.");
            return;
        }

        ClearLevel();
        CurrentLevelIndex = levelIndex;
        LevelData data = levels[levelIndex];

        if (gateController != null) gateController.Clear();

        var critters = new List<CritterPiece>();
        var arrows   = new List<ArrowRedirector>();
        var blockers = new List<StaticBlocker>();

        // Blockers
        if (data.blockers != null)
        {
            foreach (var b in data.blockers)
            {
                if (blockerPrefab == null) break;
                StaticBlocker inst = Instantiate(blockerPrefab, spawnRoot);
                inst.SetCell(b.cell);
                inst.transform.position = WorldOf(b.cell);
                blockers.Add(inst);
                _spawned.Add(inst.gameObject);
            }
        }

        // Arrows
        if (data.arrows != null)
        {
            foreach (var a in data.arrows)
            {
                if (arrowPrefab == null) break;
                ArrowRedirector inst = Instantiate(arrowPrefab, spawnRoot);
                inst.Configure(a.cell, a.direction);
                inst.transform.position = WorldOf(a.cell);
                arrows.Add(inst);
                _spawned.Add(inst.gameObject);
            }
        }

        // Gates (logical registration + optional visual)
        if (data.gates != null && gateController != null)
        {
            foreach (var g in data.gates)
            {
                gateController.RegisterGate(g.cell, g.wall, g.color, true);
                if (gateVisualPrefab != null)
                {
                    GameObject vis = Instantiate(gateVisualPrefab, spawnRoot);
                    vis.transform.position = WorldOf(g.cell);
                    TintRenderer(vis, g.color);
                    _spawned.Add(vis);
                }
            }
        }

        // Critters
        if (data.critters != null)
        {
            int id = 0;
            foreach (var c in data.critters)
            {
                if (critterPrefab == null) break;
                CritterPiece inst = Instantiate(critterPrefab, spawnRoot);
                inst.Init(id++, c.color, c.cell);
                inst.ResetVisual();
                inst.transform.position = WorldOf(c.cell);
                TintRenderer(inst.gameObject, c.color);
                critters.Add(inst);
                _spawned.Add(inst.gameObject);
            }
        }

        // Hand the assembled board to the controller.
        if (board != null)
            board.BuildBoard(data.gridWidth, data.gridHeight, critters, arrows, blockers, gateController);

        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }

    /// <summary>Reloads the current level from scratch (Retry).</summary>
    public void RetryCurrent()
    {
        if (CurrentLevelIndex >= 0) LoadLevel(CurrentLevelIndex);
    }

    /// <summary>Loads the next level if one exists.</summary>
    public void LoadNext()
    {
        if (levels != null && CurrentLevelIndex + 1 < levels.Length)
            LoadLevel(CurrentLevelIndex + 1);
    }

    private void ClearLevel()
    {
        foreach (var go in _spawned)
            if (go != null) Destroy(go);
        _spawned.Clear();
    }

    private Vector3 WorldOf(Vector2Int cell)
    {
        return board != null ? board.CellToWorld(cell) : new Vector3(cell.x, 0f, -cell.y);
    }

    private void TintRenderer(GameObject go, CritterColor color)
    {
        if (colorTints == null || colorTints.Length == 0) return;
        int idx = (int)color;
        if (idx < 0 || idx >= colorTints.Length) return;

        Renderer r = go.GetComponentInChildren<Renderer>();
        if (r != null) r.material.color = colorTints[idx];
    }
}
