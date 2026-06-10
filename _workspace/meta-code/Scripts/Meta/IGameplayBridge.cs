/// <summary>Interface that meta-code uses to talk to gameplay-code. gameplay-dev must implement these on their classes.</summary>
public interface ILevelManager
{
    void LoadLevel(int levelIndex);
    void ReloadCurrentLevel();
    int CurrentLevelIndex { get; }
    event System.Action OnLevelComplete;
}

/// <summary>Board-level actions the HUD invokes; gameplay-dev implements this on BoardController.</summary>
public interface IBoardController
{
    void UndoLastMove();
}
