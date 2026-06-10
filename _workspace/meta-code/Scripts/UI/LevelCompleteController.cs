using UnityEngine;
using UnityEngine.UI;

/// <summary>Level-complete overlay: records completion and offers Next/Replay/Level Select.</summary>
public class LevelCompleteController : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Text titleLabel;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Text nextButtonLabel;

    [SerializeField] private LevelSelectController levelSelect;

    private void Awake()
    {
        if (nextButton != null) nextButton.onClick.AddListener(OnNextTapped);
        if (replayButton != null) replayButton.onClick.AddListener(OnReplayTapped);
        if (levelSelectButton != null) levelSelectButton.onClick.AddListener(OnLevelSelectTapped);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChanged;
        }
        HandleStateChanged(GameManager.Instance != null ? GameManager.Instance.State : GameState.Boot);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }
    }

    private void HandleStateChanged(GameState state)
    {
        bool show = state == GameState.LevelComplete;
        if (root != null) root.SetActive(show);
        if (show) Show();
    }

    /// <summary>Marks the current level complete, updates labels, and configures buttons.</summary>
    private void Show()
    {
        int current = CurrentIndex();

        // Record completion (dedup + unlock next) on show.
        LevelProgressManager.Instance.MarkCompleted(current);

        if (titleLabel != null)
        {
            titleLabel.text = "Level " + (current + 1) + " Complete!";
        }

        bool isLast = current >= LevelProgressManager.TotalLevels - 1;
        if (nextButtonLabel != null)
        {
            nextButtonLabel.text = isLast ? "All Done!" : "Next Level";
        }
    }

    private void OnNextTapped()
    {
        int current = CurrentIndex();
        int next = current + 1;
        if (next >= LevelProgressManager.TotalLevels)
        {
            // Last level — return to level select instead.
            OnLevelSelectTapped();
            return;
        }

        ILevelManager lm = GameManager.Instance != null ? GameManager.Instance.LevelManager : null;
        if (lm != null) lm.LoadLevel(next);
    }

    private void OnReplayTapped()
    {
        ILevelManager lm = GameManager.Instance != null ? GameManager.Instance.LevelManager : null;
        if (lm != null) lm.ReloadCurrentLevel();
    }

    private void OnLevelSelectTapped()
    {
        if (root != null) root.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.LevelSelect);
        if (levelSelect != null)
        {
            levelSelect.gameObject.SetActive(true);
            levelSelect.Refresh();
        }
    }

    private static int CurrentIndex()
    {
        ILevelManager lm = GameManager.Instance != null ? GameManager.Instance.LevelManager : null;
        return lm != null ? lm.CurrentLevelIndex : 0;
    }
}
