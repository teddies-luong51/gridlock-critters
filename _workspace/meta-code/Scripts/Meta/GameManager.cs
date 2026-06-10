using System;
using UnityEngine;

/// <summary>Owns the high-level game state machine and broadcasts state changes to UI.</summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /// <summary>Fires whenever the game state changes (new state passed).</summary>
    public event Action<GameState> OnStateChanged;

    /// <summary>Current high-level game state.</summary>
    public GameState State { get; private set; } = GameState.Boot;

    /// <summary>The gameplay-side level manager, registered by gameplay-dev at startup.</summary>
    public ILevelManager LevelManager { get; set; }

    /// <summary>The gameplay-side board controller, registered by gameplay-dev at level load.</summary>
    public IBoardController BoardController { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Transitions to a new state, applying time-scale for pause and notifying listeners.</summary>
    public void ChangeState(GameState newState)
    {
        State = newState;
        Time.timeScale = newState == GameState.Paused ? 0f : 1f;
        OnStateChanged?.Invoke(newState);
    }
}

/// <summary>High-level game states.</summary>
public enum GameState
{
    Boot,
    LevelSelect,
    Playing,
    Paused,
    LevelComplete
}
