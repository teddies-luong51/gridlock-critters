using UnityEngine;
using UnityEngine.UI;

/// <summary>Builds the 30-button level-select grid and routes taps into the level manager.</summary>
public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private Transform gridParent;
    [SerializeField] private Button buttonPrefab;

    [Header("State Colors")]
    [SerializeField] private Color lockedColor = Color.grey;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color completedColor = new Color(0.4f, 0.8f, 0.4f);

    [Header("Per-button child references (by name)")]
    [SerializeField] private string labelChildName = "Label";
    [SerializeField] private string lockIconChildName = "LockIcon";
    [SerializeField] private string checkmarkChildName = "Checkmark";
    [SerializeField] private string highlightChildName = "Highlight";

    private readonly Button[] _buttons = new Button[LevelProgressManager.TotalLevels];

    private void Awake()
    {
        BuildGrid();
    }

    private void OnEnable()
    {
        if (LevelProgressManager.Instance != null)
        {
            LevelProgressManager.Instance.OnProgressChanged += Refresh;
        }
        Refresh();
    }

    private void OnDisable()
    {
        if (LevelProgressManager.Instance != null)
        {
            LevelProgressManager.Instance.OnProgressChanged -= Refresh;
        }
    }

    private void BuildGrid()
    {
        for (int i = 0; i < LevelProgressManager.TotalLevels; i++)
        {
            int levelIndex = i; // capture for closure
            Button btn = Instantiate(buttonPrefab, gridParent);
            btn.onClick.AddListener(() => OnLevelButtonTapped(levelIndex));
            _buttons[i] = btn;
        }
    }

    /// <summary>Re-applies visuals to every button based on current progression.</summary>
    public void Refresh()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            ApplyState(_buttons[i], i, LevelProgressManager.Instance.GetLevelState(i));
        }
    }

    private void ApplyState(Button btn, int levelIndex, LevelState state)
    {
        if (btn == null)
        {
            return;
        }

        Image bg = btn.GetComponent<Image>();
        Text label = FindChild<Text>(btn.transform, labelChildName);
        GameObject lockIcon = FindChildObject(btn.transform, lockIconChildName);
        GameObject checkmark = FindChildObject(btn.transform, checkmarkChildName);
        GameObject highlight = FindChildObject(btn.transform, highlightChildName);

        bool showNumber = state != LevelState.Locked;
        if (label != null)
        {
            label.text = showNumber ? (levelIndex + 1).ToString() : string.Empty;
        }
        if (lockIcon != null) lockIcon.SetActive(state == LevelState.Locked);
        if (checkmark != null) checkmark.SetActive(state == LevelState.Completed);
        if (highlight != null) highlight.SetActive(state == LevelState.Current);

        switch (state)
        {
            case LevelState.Locked:
                if (bg != null) bg.color = lockedColor;
                btn.interactable = false;
                break;
            case LevelState.Unlocked:
            case LevelState.Current:
                if (bg != null) bg.color = unlockedColor;
                btn.interactable = true;
                break;
            case LevelState.Completed:
                if (bg != null) bg.color = completedColor;
                btn.interactable = true;
                break;
        }
    }

    /// <summary>Loads the tapped level through the gameplay level manager.</summary>
    public void OnLevelButtonTapped(int levelIndex)
    {
        ILevelManager lm = GameManager.Instance != null ? GameManager.Instance.LevelManager : null;
        if (lm == null)
        {
            Debug.LogWarning("LevelSelectController: no ILevelManager registered on GameManager.");
            return;
        }
        lm.LoadLevel(levelIndex);
    }

    private static T FindChild<T>(Transform root, string childName) where T : Component
    {
        Transform t = root.Find(childName);
        return t != null ? t.GetComponent<T>() : null;
    }

    private static GameObject FindChildObject(Transform root, string childName)
    {
        Transform t = root.Find(childName);
        return t != null ? t.gameObject : null;
    }
}
