using System;
using UnityEngine;

/// <summary>Per-level lock/unlock/current/completed state derived from SaveManager data.</summary>
public class LevelProgressManager : MonoBehaviour
{
    public static LevelProgressManager Instance { get; private set; }

    /// <summary>Total number of levels in the game.</summary>
    public const int TotalLevels = 30;

    /// <summary>Fires after progress changes (e.g. a level is marked complete).</summary>
    public event Action OnProgressChanged;

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

    /// <summary>Level 0 is always unlocked; otherwise level n unlocks when n-1 is completed.</summary>
    public bool IsLevelUnlocked(int n)
    {
        if (n <= 0)
        {
            return true;
        }
        return SaveManager.Instance.HasCompleted(n - 1);
    }

    /// <summary>Returns the display/interaction state for a given level.</summary>
    public LevelState GetLevelState(int n)
    {
        if (!IsLevelUnlocked(n))
        {
            return LevelState.Locked;
        }

        if (SaveManager.Instance.HasCompleted(n))
        {
            return LevelState.Completed;
        }

        // Unlocked and not completed: it is "Current" if it is the resume target.
        if (n == SaveManager.Instance.GetResumeLevel())
        {
            return LevelState.Current;
        }

        return LevelState.Unlocked;
    }

    /// <summary>Marks a level completed via SaveManager and broadcasts the change.</summary>
    public void MarkCompleted(int n)
    {
        SaveManager.Instance.MarkCompleted(n);
        OnProgressChanged?.Invoke();
    }

    /// <summary>Broadcasts that progression changed (e.g. when completion was recorded elsewhere).</summary>
    public void NotifyProgressChanged()
    {
        OnProgressChanged?.Invoke();
    }
}

/// <summary>Visual/interaction state of a level button.</summary>
public enum LevelState
{
    Locked,
    Unlocked,
    Current,
    Completed
}
