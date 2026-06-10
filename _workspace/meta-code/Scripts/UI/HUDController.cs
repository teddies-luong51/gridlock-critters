using UnityEngine;
using UnityEngine.UI;

/// <summary>In-level heads-up display: current level label, undo button, pause button.</summary>
public class HUDController : MonoBehaviour
{
    [SerializeField] private Text levelLabel;
    [SerializeField] private Button undoButton;
    [SerializeField] private Button pauseButton;

    private void Awake()
    {
        if (undoButton != null) undoButton.onClick.AddListener(OnUndoTapped);
        if (pauseButton != null) pauseButton.onClick.AddListener(OnPauseTapped);
    }

    private void OnEnable()
    {
        RefreshLevelLabel();
    }

    /// <summary>Updates the "Level N" label from the current gameplay level (1-based display).</summary>
    public void RefreshLevelLabel()
    {
        if (levelLabel == null) return;

        ILevelManager lm = GameManager.Instance != null ? GameManager.Instance.LevelManager : null;
        int display = lm != null ? lm.CurrentLevelIndex + 1 : 1;
        levelLabel.text = "Level " + display;
    }

    private void OnUndoTapped()
    {
        IBoardController board = GameManager.Instance != null ? GameManager.Instance.BoardController : null;
        if (board == null)
        {
            Debug.LogWarning("HUDController: no IBoardController registered; cannot undo.");
            return;
        }
        board.UndoLastMove();
    }

    private void OnPauseTapped()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.Paused);
        }
    }
}
