using System;
using UnityEngine;

/// <summary>Listens for board win signals and drives GameManager into the LevelComplete state.</summary>
public class WinChecker : MonoBehaviour
{
    private static WinChecker _instance;

    /// <summary>Fired when all critters have exited and the level is won.</summary>
    public event Action OnWin;

    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        if (BoardController.Instance != null)
            BoardController.Instance.OnTiltResolved += HandleTiltResolved;
    }

    private void Start()
    {
        if (BoardController.Instance != null)
        {
            BoardController.Instance.OnTiltResolved -= HandleTiltResolved;
            BoardController.Instance.OnTiltResolved += HandleTiltResolved;
        }
    }

    private void OnDisable()
    {
        if (BoardController.Instance != null)
            BoardController.Instance.OnTiltResolved -= HandleTiltResolved;
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    private void HandleTiltResolved(bool won)
    {
        if (won) Win();
    }

    /// <summary>Static bridge so BoardController can notify even if event wiring order varies.</summary>
    public static void NotifyWin()
    {
        if (_instance != null) _instance.Win();
    }

    private void Win()
    {
        OnWin?.Invoke();
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameState.LevelComplete);
    }
}
