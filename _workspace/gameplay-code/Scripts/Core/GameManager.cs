using System;
using UnityEngine;

/// <summary>Top-level game state machine; broadcasts state transitions to interested systems.</summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /// <summary>High-level application/game states.</summary>
    public enum GameState { MainMenu, Playing, Paused, LevelComplete, GameOver }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    /// <summary>Fired whenever the state changes, with the new state.</summary>
    public event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Application.targetFrameRate = 60;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Transitions to a new state and notifies listeners (idempotent re-fires allowed).</summary>
    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
