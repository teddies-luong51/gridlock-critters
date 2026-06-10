using UnityEngine;
using UnityEngine.UI;

/// <summary>Pause overlay: Resume, Restart, Level Select, Main Menu.</summary>
public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button mainMenuButton;

    [SerializeField] private LevelSelectController levelSelect;

    private void Awake()
    {
        if (resumeButton != null) resumeButton.onClick.AddListener(OnResumeTapped);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartTapped);
        if (levelSelectButton != null) levelSelectButton.onClick.AddListener(OnLevelSelectTapped);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenuTapped);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChanged;
        }
        HandleStateChanged(GameManager.Instance != null ? GameManager.Instance.CurrentState : GameState.Boot);
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
        if (root != null) root.SetActive(state == GameState.Paused);
    }

    private void OnResumeTapped()
    {
        if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.Playing);
    }

    private void OnRestartTapped()
    {
        if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.Playing);
        ILevelManager lm = GameManager.Instance != null ? GameManager.Instance.LevelManager : null;
        if (lm != null) lm.ReloadCurrentLevel();
    }

    private void OnLevelSelectTapped()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.Save();
        if (root != null) root.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.LevelSelect);
        if (levelSelect != null)
        {
            levelSelect.gameObject.SetActive(true);
            levelSelect.Refresh();
        }
    }

    private void OnMainMenuTapped()
    {
        // No title screen in MVP — Main Menu routes to Level Select.
        OnLevelSelectTapped();
    }
}
