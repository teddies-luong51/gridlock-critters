---
name: unity-implementation
description: Implements Unity (C#) code for hybrid casual mobile games. Use this skill for all Unity code, scene setup, input handling, physics, performance optimization, and mobile build configuration. Always triggers for: "Unity", "C# script", "MonoBehaviour", "scene design", "implement the game", "core loop implementation", "accelerometer", "tilt input", "3D board", "perspective camera", "physics tilt".
---

## Project Structure

```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Core/        # GameManager, LevelManager, BoardController
│   │   ├── Input/       # TiltInputHandler, EditorInputHandler
│   │   ├── Board/       # BoardTiltAnimator, CritterMover, GateController
│   │   ├── Meta/        # LevelProgressManager, SaveManager
│   │   ├── UI/          # HUD, LevelSelect, LevelComplete
│   │   ├── Data/        # ScriptableObjects (LevelData, CritterData)
│   │   └── Utils/       # ObjectPool, Easing, Extensions
│   ├── Prefabs/
│   │   ├── Board/       # BoardRoot, Cell, Critter, Gate
│   │   └── UI/
│   ├── Scenes/
│   └── Art/
└── Plugins/             # Third-party SDKs
```

---

## Gridlock Critters — Core Architecture

### 1. Camera Setup (3D Perspective — 45° Angled View)

Position the camera to match the chess-board-style perspective: elevated, angled down at ~45°, slightly offset to give depth.

```csharp
// Camera setup values — apply in Inspector or Bootstrap
// Position:  (0, 12, -10)
// Rotation:  (45, 0, 0)
// FOV:       50
// Near clip: 0.3   Far clip: 100

public class CameraSetup : MonoBehaviour
{
    [Header("3D Perspective — Gridlock Critters")]
    public Vector3 position  = new Vector3(0f, 12f, -10f);
    public Vector3 rotation  = new Vector3(45f, 0f, 0f);
    public float   fieldOfView = 50f;

    void Awake()
    {
        transform.position    = position;
        transform.eulerAngles = rotation;
        Camera.main.fieldOfView = fieldOfView;
    }
}
```

The board sits at world origin (0,0,0). From this angle, the player sees the full board in perspective — critters slide toward/away with visible depth, and board tilt animations look physically real.

---

### 2. Physical Tilt Input (Accelerometer)

**No special permissions needed on iOS or Android.** `Input.acceleration` is always available.

```csharp
public class TiltInputHandler : MonoBehaviour
{
    public static TiltInputHandler Instance { get; private set; }

    public event Action<TiltDirection> OnTilt;

    [Header("Sensitivity")]
    [SerializeField] float tiltThreshold = 0.35f;   // 0.25–0.45 recommended
    [SerializeField] float smoothing     = 0.1f;     // low-pass filter strength
    [SerializeField] float cooldown      = 0.6f;     // seconds between tilts

    private Vector3  _smoothedAccel;
    private float    _lastTiltTime = -999f;
    private bool     _tilting;

    public enum TiltDirection { Left, Right, Up, Down }

    void Awake() => Instance = this;

    void Update()
    {
        // Low-pass filter — smooths out hand shake
        _smoothedAccel = Vector3.Lerp(_smoothedAccel, Input.acceleration, smoothing);

        if (Time.time - _lastTiltTime < cooldown) return;

        TiltDirection? dir = DetectTilt(_smoothedAccel);
        if (dir.HasValue)
        {
            _lastTiltTime = Time.time;
            OnTilt?.Invoke(dir.Value);
        }
    }

    TiltDirection? DetectTilt(Vector3 accel)
    {
        // accel.x: negative = tilt left, positive = tilt right
        // accel.y: negative = tilt down (toward player), positive = tilt up (away)
        float absX = Mathf.Abs(accel.x);
        float absY = Mathf.Abs(accel.y);

        if (absX < tiltThreshold && absY < tiltThreshold) return null;

        // Dominant axis wins
        if (absX >= absY)
            return accel.x < 0 ? TiltDirection.Left : TiltDirection.Right;
        else
            return accel.y < 0 ? TiltDirection.Down : TiltDirection.Up;
    }
}
```

**Editor fallback (keyboard arrow keys for testing in Unity Editor):**

```csharp
public class EditorTiltInput : MonoBehaviour
{
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))  BoardController.Instance.ExecuteTilt(TiltInputHandler.TiltDirection.Left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) BoardController.Instance.ExecuteTilt(TiltInputHandler.TiltDirection.Right);
        if (Input.GetKeyDown(KeyCode.UpArrow))    BoardController.Instance.ExecuteTilt(TiltInputHandler.TiltDirection.Up);
        if (Input.GetKeyDown(KeyCode.DownArrow))  BoardController.Instance.ExecuteTilt(TiltInputHandler.TiltDirection.Down);
    }
#endif
}
```

---

### 3. Board Tilt Animation (3D Visual)

When a tilt fires, the board mesh physically rotates before critters slide — this is what makes the physical tilt feel real on screen.

```csharp
public class BoardTiltAnimator : MonoBehaviour
{
    [SerializeField] float tiltAngle    = 15f;   // degrees of visible lean
    [SerializeField] float tiltDuration = 0.15f; // seconds to lean
    [SerializeField] float holdDuration = 0.05f; // hold at peak
    [SerializeField] float returnDuration = 0.2f;

    // Call this before critters start moving
    public IEnumerator PlayTilt(TiltInputHandler.TiltDirection dir)
    {
        Vector3 targetRotation = dir switch
        {
            TiltInputHandler.TiltDirection.Left  => new Vector3(0, 0,  tiltAngle),
            TiltInputHandler.TiltDirection.Right => new Vector3(0, 0, -tiltAngle),
            TiltInputHandler.TiltDirection.Up    => new Vector3(-tiltAngle, 0, 0),
            TiltInputHandler.TiltDirection.Down  => new Vector3( tiltAngle, 0, 0),
            _ => Vector3.zero
        };

        // Lean
        yield return LerpRotation(Vector3.zero, targetRotation, tiltDuration);
        // Hold
        yield return new WaitForSeconds(holdDuration);
        // Return
        yield return LerpRotation(targetRotation, Vector3.zero, returnDuration);
    }

    IEnumerator LerpRotation(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            transform.localEulerAngles = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.localEulerAngles = to;
    }
}
```

---

### 4. BoardController — Tilt → Slide Logic

```csharp
public class BoardController : MonoBehaviour
{
    public static BoardController Instance { get; private set; }

    [SerializeField] BoardTiltAnimator tiltAnimator;
    [SerializeField] float critterSlideSpeed = 8f;

    private CritterPiece[,] _grid;   // [col, row]
    private int _cols, _rows;
    private bool _isAnimating;

    void Awake() => Instance = this;

    void Start()
    {
        TiltInputHandler.Instance.OnTilt += ExecuteTilt;
    }

    public async void ExecuteTilt(TiltInputHandler.TiltDirection dir)
    {
        if (_isAnimating) return;
        _isAnimating = true;

        // 1. Visual board lean
        yield return StartCoroutine(tiltAnimator.PlayTilt(dir));

        // 2. Resolve critter slides (logic, not animation yet)
        List<SlideMove> moves = ResolveMoves(dir);

        // 3. Animate all critters sliding simultaneously
        yield return AnimateSlides(moves);

        // 4. Check win condition
        CheckLevelComplete();

        _isAnimating = false;
    }

    List<SlideMove> ResolveMoves(TiltInputHandler.TiltDirection dir)
    {
        // Process from destination wall outward — lead pieces settle first
        // Returns list of (critter, fromCell, toCell, exits:bool)
        // See references/gridlock-slide-logic.md for full BFS resolver
        var moves = new List<SlideMove>();
        // ... resolver implementation
        return moves;
    }

    IEnumerator AnimateSlides(List<SlideMove> moves)
    {
        // All critters slide simultaneously
        var tweens = moves.Select(m => m.Critter.SlideTo(m.TargetWorldPos, critterSlideSpeed));
        yield return CoroutineUtils.WaitForAll(this, tweens);
    }
}
```

---

### 5. GameManager — State Machine

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, LevelComplete, GameOver }
    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnStateChanged;

    void Awake() => Instance = this;

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
```

---

### 6. Scene Structure

```
Bootstrap   — app init, save load, scene manager
MainMenu    — level select, meta UI
Game        — board + critters + tilt input + HUD
Loading     — transition screen
```

---

## Performance Checklist (Mobile 60fps)

- [ ] No `GetComponent` calls in `Update()` — cache in `Awake()`
- [ ] Critter GameObjects use object pooling (reuse between levels)
- [ ] Board mesh: single draw call with combined mesh or GPU instancing
- [ ] `Application.targetFrameRate = 60` set in Bootstrap
- [ ] Physics layers matrix configured — disable unused collision pairs
- [ ] Texture atlases applied to reduce draw calls
- [ ] `QualitySettings` mobile preset active
- [ ] Tilt cooldown (0.6s) prevents rapid-fire accidental tilts from hand shake

## Tilt Sensitivity Tuning Guide

| `tiltThreshold` | Feel |
|-----------------|------|
| 0.25 | Very sensitive — triggers on small movements |
| 0.35 | **Recommended default** — deliberate tilts only |
| 0.45 | Requires strong tilt — good for seated play |

Expose `tiltThreshold` as a ScriptableObject value so it can be tuned without recompile.

## References

Detailed slide logic (BFS resolver), critter exit animation, gate VFX, and SDK integration in `references/`:
- `references/gridlock-slide-logic.md` — full BFS slide resolver for N×M grid
- `references/critter-animations.md` — slide, exit pop, win sequence
- `references/sdk-integration.md` — AdMob, Unity IAP, Firebase
