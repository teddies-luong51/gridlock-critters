using UnityEngine;

/// <summary>
/// Registers gameplay components (LevelManager, BoardController) with the meta systems on scene
/// load, so meta UI can drive gameplay through the ILevelManager / IBoardController interfaces.
/// Attach to the [GameManager] GameObject in the Game scene.
/// </summary>
public class GameplayBridge : MonoBehaviour
{
    public static GameplayBridge Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // By Start both LevelManager and BoardController singletons have run Awake,
        // so meta UI can resolve them via the static accessors below.
        Register();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Re-resolves and exposes the live gameplay singletons via their interfaces.</summary>
    public void Register()
    {
        if (LevelManager.Instance == null)
            Debug.LogWarning("[GameplayBridge] LevelManager.Instance is null at registration.");
        if (BoardController.Instance == null)
            Debug.LogWarning("[GameplayBridge] BoardController.Instance is null at registration.");
    }

    /// <summary>The gameplay level manager, as the meta-facing ILevelManager.</summary>
    public static ILevelManager LevelManager => global::LevelManager.Instance;

    /// <summary>The gameplay board controller, as the meta-facing IBoardController.</summary>
    public static IBoardController BoardController => global::BoardController.Instance;
}
