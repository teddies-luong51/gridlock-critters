---
name: qa-game
description: 하이브리드 캐주얼 게임의 QA 테스트를 수행하는 스킬. 버그 리포트 작성, 테스트 케이스 정의, 코어 루프/메타 시스템 정합성 검증, 경제 밸런스 검증, 성능 테스트를 요청받으면 반드시 이 스킬을 사용할 것. "QA", "버그 리포트", "테스트", "정합성 검증", "버그 찾기", "품질 검증" 키워드가 포함되면 트리거한다.
---

## QA 전략 — 점진적 검증

전체 완성 후 한 번에 테스트하지 않는다. 각 모듈 완성 직후 즉시 해당 모듈을 검증한다.

```
[코어 루프 완성] → 코어 루프 QA
[메타 시스템 완성] → 메타 QA + 경계면 교차 검증
[수익화 연동 완성] → 수익화 QA
[전체 통합] → 통합 QA + 성능 테스트
```

## 핵심 검증 영역

### 1. 코어 루프 검증

```
[ ] 게임 시작 → 플레이 → 종료 전체 사이클 3회 이상 반복 가능
[ ] 레벨 클리어 조건이 정확히 판정됨
[ ] 레벨 실패 조건이 정확히 판정됨
[ ] 재시작 시 상태가 완전히 초기화됨
[ ] 30초 이내에 한 사이클 완성 가능
[ ] 레벨 1을 튜토리얼 없이 3회 시도 내에 클리어 가능
```

### 2. 메타 시스템 경계면 검증

경계면 교차 비교가 핵심이다 — 코드 존재 여부가 아닌 실제 데이터 흐름을 추적한다.

```
[ ] 레벨 클리어 보상 → EconomyManager.Earn() 호출 확인
[ ] EconomyManager 잔액 → UI 통화 표시 일치 확인
[ ] 업그레이드 구매 → EconomyManager.TrySpend() 실패 시 구매 불가 확인
[ ] SaveManager.Save() 호출 후 앱 재시작 → 데이터 복원 확인
[ ] 통화 음수 불가 (TrySpend 반환값 검증)
```

### 3. 경제 밸런스 검증

```
[ ] 레벨 10까지 소프트 통화 수입/지출 흐름 추적
[ ] 업그레이드 없이 레벨 N까지 클리어 가능한지 확인 (페이월 체크)
[ ] 하드 통화 0 상태에서 게임 진행 불가 시나리오 없는지 확인
[ ] 보상형 광고 미시청 시에도 정상 진행 가능한지 확인
```

### 4. 수익화 연동 검증

```
[ ] 보상형 광고 시청 완료 → 보상 지급 확인
[ ] 보상형 광고 시청 중단 → 보상 미지급 확인
[ ] IAP 구매 완료 → 하드 통화 지급 확인
[ ] IAP 구매 취소 → 통화 미지급, 결제 없음 확인
[ ] 광고 SDK 초기화 실패 시 → 게임 크래시 없음 확인
```

## 버그 리포트 템플릿

```markdown
## [BUG-{번호}] {제목}

**심각도:** Critical / High / Medium / Low
**모듈:** Core Loop / Meta System / Economy / UI / Monetization
**발견자:** QA Engineer
**발견 일시:** {날짜}

### 재현 단계 (Steps to Reproduce)
1.
2.
3.

### 예상 결과
{어떻게 동작해야 하는가}

### 실제 결과
{실제로 어떻게 동작했는가}

### 환경
- Unity 버전:
- 기기/에뮬레이터:
- OS 버전:

### 첨부
{스크린샷, 로그}
```

**심각도 기준:**
- **Critical**: 크래시, 데이터 손실, 진행 불가
- **High**: 주요 기능 오작동 (보상 미지급, 저장 실패)
- **Medium**: 기능 오작동이나 우회 가능
- **Low**: 시각적 오류, 텍스트 오탈자

## 성능 테스트 기준

```
목표 기기: 중급 Android (2019년 이후 출시, RAM 3GB)
목표 FPS: 플레이 중 60fps 유지 (스파이크 55fps 이하 금지)
메모리: 플레이 중 총 메모리 350MB 이하
로딩 시간: 씬 전환 3초 이하
배터리: 30분 플레이 후 발열 없음 (일반 환경)
```

## Unity Test Runner 활용

**EditMode 테스트 (로직 단위 테스트):**
```csharp
[Test]
public void EconomyManager_TrySpend_ReturnsFalse_WhenInsufficientFunds()
{
    var economy = new EconomyManager();
    economy.SetSoftCurrency(50);
    Assert.IsFalse(economy.TrySpend(softAmount: 100));
    Assert.AreEqual(50, economy.SoftCurrency);
}
```

**PlayMode 테스트 (씬 통합 테스트):**
```csharp
[UnityTest]
public IEnumerator LevelComplete_GrantsReward()
{
    GameManager.Instance.ChangeState(GameState.Playing);
    yield return null;
    LevelManager.Instance.CompleteLevel();
    yield return new WaitForSeconds(0.5f);
    Assert.Greater(EconomyManager.Instance.SoftCurrency, 0);
}
```
