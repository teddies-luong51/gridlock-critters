---
name: unity-implementation
description: 하이브리드 캐주얼 게임의 Unity(C#) 코드를 구현하는 스킬. Unity 코드 작성, 씬 설계, 물리/입력 처리, 성능 최적화, 모바일 빌드 설정을 요청받으면 반드시 이 스킬을 사용할 것. "Unity", "C# 스크립트", "MonoBehaviour", "씬 설계", "모바일 게임 구현", "코어 루프 구현" 키워드가 포함되면 트리거한다.
---

## 프로젝트 구조 표준

```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Core/          # 코어 루프 (GameManager, LevelManager)
│   │   ├── Meta/          # 메타 시스템 (EconomyManager, UpgradeManager)
│   │   ├── UI/            # UI 컨트롤러
│   │   ├── Data/          # ScriptableObject 데이터
│   │   └── Utils/         # 공통 유틸리티
│   ├── Prefabs/
│   ├── Scenes/
│   ├── ScriptableObjects/
│   └── Art/               # 에셋 (아트팀 산출물)
└── Plugins/               # 서드파티 SDK
```

## 핵심 아키텍처 패턴

### GameManager — 게임 상태 머신

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

### 입력 처리 (모바일 터치 + 에디터 마우스)

```csharp
public class InputHandler : MonoBehaviour
{
    public event Action<Vector2> OnTap;
    public event Action<Vector2, Vector2> OnSwipe;

    private Vector2 touchStart;
    private const float SWIPE_THRESHOLD = 50f;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) touchStart = Input.mousePosition;
        if (Input.GetMouseButtonUp(0)) ProcessRelease(Input.mousePosition);
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) touchStart = touch.position;
            if (touch.phase == TouchPhase.Ended) ProcessRelease(touch.position);
        }
#endif
    }

    void ProcessRelease(Vector2 endPos)
    {
        Vector2 delta = endPos - touchStart;
        if (delta.magnitude < SWIPE_THRESHOLD) OnTap?.Invoke(endPos);
        else OnSwipe?.Invoke(touchStart, endPos);
    }
}
```

## 성능 최적화 (모바일 60fps 목표)

**오브젝트 풀링:**
```csharp
public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> pool = new Queue<T>();
    private T prefab;
    private Transform parent;

    public ObjectPool(T prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        for (int i = 0; i < initialSize; i++) Return(Create());
    }

    public T Get()
    {
        T obj = pool.Count > 0 ? pool.Dequeue() : Create();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

    private T Create() => Object.Instantiate(prefab, parent);
}
```

**체크리스트 (빌드 전):**
- [ ] Update()에서 GetComponent 호출 없음 (캐싱 사용)
- [ ] 자주 생성/삭제되는 오브젝트는 풀링 적용
- [ ] 텍스처 아틀라스 적용 (드로우콜 최소화)
- [ ] 물리 레이어 매트릭스 설정 (불필요한 충돌 연산 제거)
- [ ] Target Frame Rate 설정: `Application.targetFrameRate = 60`
- [ ] Quality Settings: 모바일 전용 프리셋 사용

## 씬 구조

**필수 씬 목록:**
```
Bootstrap (앱 시작, 초기화, 씬 로드 매니저)
MainMenu   (홈 화면, 메타 UI)
Game       (코어 루프 플레이)
Loading    (씬 전환 로딩)
```

**씬 전환 패턴:**
```csharp
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public async void LoadScene(string sceneName)
    {
        // 로딩 화면 표시
        await SceneManager.LoadSceneAsync("Loading");
        await SceneManager.LoadSceneAsync(sceneName);
    }
}
```

## 세부 구현 가이드

장르별 코어 루프 구현 패턴, SDK 연동(광고/IAP), UI 시스템 상세는 `references/` 폴더를 참조한다:
- `references/genre-patterns.md` — Runner, Merge, Puzzle, Idle 장르별 구현
- `references/sdk-integration.md` — AdMob, Unity IAP, Firebase 연동
