using System;
using UnityEngine;

/// <summary>High-level game states used by both gameplay and meta/UI systems.</summary>
public enum GameState
{
    Boot,
    MainMenu,
    LevelSelect,
    Playing,
    Paused,
    LevelComplete,
    GameOver
}

/// <summary>
/// Single authoritative game state machine. Broadcasts state transitions to interested
/// systems, applies pause time-scale, and holds the live gameplay bridge references
/// (ILevelManager / IBoardController) registered by gameplay components on Awake.
/// (Merged from the former gameplay GameManager.)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /// <summary>Current high-level game state.</summary>
    public GameState CurrentState { get; private set; } = GameState.Boot;

    /// <summary>Fired whenever the state changes, with the new state.</summary>
    public event Action<GameState> OnStateChanged;

    /// <summary>The live gameplay level manager; set by LevelManager.Awake. Null outside gameplay scenes.</summary>
    public ILevelManager LevelManager { get; set; }

    /// <summary>The live board controller; set by BoardController.Awake. Null outside gameplay scenes.</summary>
    public IBoardController BoardController { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Transitions to a new state, applying pause time-scale and notifying listeners.</summary>
    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Time.timeScale = newState == GameState.Paused ? 0f : 1f;
        OnStateChanged?.Invoke(newState);
    }
}
