---
name: meta-progression
description: 하이브리드 캐주얼 게임의 메타 진행 시스템을 설계하고 구현하는 스킬. 업그레이드 트리, 통화 경제, 인벤토리, 저장 시스템, 진행 시스템 설계를 요청받으면 반드시 이 스킬을 사용할 것. "메타 시스템", "업그레이드", "통화 설계", "진행 구조", "경제 밸런스", "저장 시스템", "리텐션" 키워드가 포함되면 트리거한다.
---

## 메타 시스템 구조

하이브리드 캐주얼 메타는 3개 레이어로 구성된다:

```
[단기 목표] 레벨 클리어, 데일리 미션 → 소프트 통화
[중기 목표] 챕터 완료, 캐릭터 수집 → 프리미엄 통화
[장기 목표] 최대 업그레이드, 컬렉션 완성 → 희귀 아이템
```

## 통화 시스템 설계

**2통화 구조 (표준):**
- **소프트 통화 (코인):** 코어 루프에서 획득, 기본 업그레이드에 사용
- **하드 통화 (젬):** IAP 또는 광고 시청으로 획득, 프리미엄 아이템/스킵에 사용

**통화 밸런스 원칙:**
- 소프트 통화는 플레이어가 '풍족하다'고 느끼되 고급 업그레이드는 빡빡하게 설계한다
- 하드 통화는 희소성을 유지한다 (광고 1회 = 50~100 젬, 레벨 클리어 = 10~30 젬)
- 소프트→하드 통화 환전은 일방향 (하드→소프트만 가능)

**밸런스 시트 템플릿:**
```
[획득 경로]               [소비 경로]
레벨 클리어: X 코인/레벨  기본 업그레이드: Y~Z 코인
광고 시청: X 코인         빌드 슬롯: X 코인
데일리 미션: X 코인       부활: X 코인
IAP: X 젬                프리미엄 언락: X 젬
```

## 진행 시스템 패턴

**선형 진행 (초보 친화적):**
레벨 1 → 2 → 3 → ... → N, 새 컨텐츠는 순서대로 언락

**트리 진행 (선택의 즐거움):**
코어 스킬 → 분기(A경로 또는 B경로) → 최종 스킬

**컬렉션 진행 (FOMO 활용):**
캐릭터/아이템 N개 중 M개 수집 → 리워드, 소셜 공유 유인

## Unity 구현 패턴

### 저장 시스템 (JSON 직렬화)

```csharp
[System.Serializable]
public class PlayerData
{
    public int softCurrency;
    public int hardCurrency;
    public int currentLevel;
    public List<string> unlockedItems;
    public Dictionary<string, int> upgradeLevels;
    public long lastLoginTimestamp;
}

public class SaveManager : MonoBehaviour
{
    private const string SAVE_KEY = "player_data";

    public void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public PlayerData Load()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");
        return string.IsNullOrEmpty(json) ? new PlayerData() : JsonUtility.FromJson<PlayerData>(json);
    }
}
```

### 업그레이드 시스템 (ScriptableObject)

```csharp
[CreateAssetMenu(menuName = "Game/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public int maxLevel;
    public int[] costPerLevel;     // 레벨별 비용
    public float[] valuePerLevel;  // 레벨별 효과값
}
```

### 경제 매니저

```csharp
public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    public event Action<int, int> OnCurrencyChanged; // (soft, hard)

    public bool TrySpend(int softAmount = 0, int hardAmount = 0)
    {
        if (playerData.softCurrency < softAmount || playerData.hardCurrency < hardAmount)
            return false;

        playerData.softCurrency -= softAmount;
        playerData.hardCurrency -= hardAmount;
        OnCurrencyChanged?.Invoke(playerData.softCurrency, playerData.hardCurrency);
        saveManager.Save(playerData);
        return true;
    }

    public void Earn(int softAmount = 0, int hardAmount = 0)
    {
        playerData.softCurrency += softAmount;
        playerData.hardCurrency += hardAmount;
        OnCurrencyChanged?.Invoke(playerData.softCurrency, playerData.hardCurrency);
        saveManager.Save(playerData);
    }
}
```

## 리텐션 설계 체크리스트

- [ ] Day 1 훅: 첫 세션에 강력한 보상 루프 완성 (업그레이드 1회 이상 경험)
- [ ] Day 3 훅: 스트릭/데일리 미션 시스템
- [ ] Day 7 훅: 주간 이벤트 또는 챕터 클리어 기념 보상
- [ ] 오프라인 보상: 앱 재방문 유인 (idle 수익)
- [ ] 푸시 알림 트리거 포인트 (에너지 회복, 이벤트 시작)
