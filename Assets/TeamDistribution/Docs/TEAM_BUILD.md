# 인형의 집 — 팀 배포용 코어 (Unity)

기획 총정리·스프린트 계획·수업 피드백을 반영한 **공통 시스템**입니다. 각 방 씬은 `TeamDistribution` 스크립트만으로 상호작용·플래그·인벤토리를 맞출 수 있습니다.

> **Unity를 거의 안 써본 팀원 / Cursor 위주로만 개발** → 먼저 **[BEGINNER_CURSOR_KR.md](./BEGINNER_CURSOR_KR.md)** 를 읽고, 그다음 이 문서를 참고하는 것을 권장합니다.

## 폴더

| 경로 | 설명 |
|------|------|
| `Assets/TeamDistribution/Scripts/` | 런타임 스크립트 |
| `Assets/TeamDistribution/Demo/` | 검증 플로우·인게임 테스트 HUD |
| `Assets/TeamDistribution/Editor/` | 에디터 메뉴 (UI 자동 생성, TMP 임포트) |
| `Assets/TeamDistribution/Docs/` | `TEAM_BUILD.md`, **`BEGINNER_CURSOR_KR.md`** (초보·Cursor 온보딩) |

## TextMesh Pro (TMP) Essential Resources

내러티브 UI 등에서 TMP를 쓰려면 **한 번** 임포트가 필요합니다.

- **Team Distribution → TextMesh Pro — Essential Resources 임포트** (프로젝트에 추가한 메뉴, 대화 없이 가져옴)  
- 또는 Unity 기본: **Window → TextMeshPro → Import TMP Essential Resources**

이후 `Assets/TextMesh Pro/Resources/TMP Settings.asset` 이 생깁니다.

## DOTween (연출·UI 트윈)

기획상 **인지 필터 글리치**, **UI 페이드**, **오브젝트 이동/스케일** 등에 쓰기 좋은 트윈 라이브러리입니다. `TeamDistribution` 코어는 DOTween 없이 동작하지만, 방 담당자가 연출할 때 **팀 전체가 같은 버전**을 쓰도록 정리해 두는 것을 권장합니다.

### 설치 (택 1)

1. **공식 UnityPackage (가장 흔함)**  
   - [DOTween 다운로드](http://dotween.demigiant.com/download.php) 에서 **UnityPackage** 받기  
   - 프로젝트에 임포트 (보통 `Assets/Plugins/Demigiant/DOTween` 등이 생김)  
   - 메뉴 **Tools → Demigiant → DOTween Utility Panel** 열기 → **Setup DOTween…** → 모듈에서 **UI** 등 필요한 항목 체크 후 **Apply**  
   - 이후 `Assets/Resources/DOTweenSettings.asset` (또는 설정 에셋)이 생성되는지 확인  

2. **OpenUPM 등 서드파티 레지스트리**  
   - 팀에서 OpenUPM·내부 Verdaccio 등을 쓰는 경우, `manifest.json`에 **합의된 패키지 ID·버전**만 추가하고 README/슬랙에 그대로 적어 두기  

### 사용 시 팀 규칙 (권장)

- `using DG.Tweening;` — 트윈은 **씬 전환·오브젝트 파괴 전** `DOKill()` / `Kill()` 로 정리해 누수·경고 방지  
- **TimeScale**: 일시정지 연출 시 `SetUpdate(true)` (unscaled) 여부 팀에서 통일  
- **Pro 버전** 사용 여부(유료)는 팀 단위로만 맞추기  

### 이 프로젝트 `manifest.json` 참고

현재 저장소의 `Packages/manifest.json`에는 DOTween이 **기본 포함되어 있지 않습니다**. 위 절차로 넣은 뒤 **Git에 Plugins(또는 패키지 항목)까지 커밋**해 팀원이 동일 상태로 열 수 있게 하세요.

## 에디터 메뉴 (빠른 배치)

상단 **GameObject → Team Distribution** 에서:

- **Narrative Text UI** — Canvas + 반투명 패널 + TMP 본문 + 「다음」 버튼 + `NarrativeTextBox`까지 연결
- **Test HUD (인게임 F3)** — `TeamDistributionTestHud` (기본 **시작 시 숨김** — 작업 방해 방지, **F3**으로 열기/닫기. 필요하면 인스펙터 **Start Visible** 켜기)
- **Team Distribution Root** — `TeamDistributionRoot`만 달린 오브젝트
- **Scene Transition (DDOL)** — 페이드 씬 전환 싱글톤 (이미 있으면 생성 안 함)

## 씬에 한 번만 두는 것

1. **`TeamDistributionRoot`** (권장) — `FlagStore`·`InventorySystem`·**EventSystem(DDOL)**·**SceneTransition(DDOL)** 자동 생성. 인스펙터에서 각 항목 끌 수 있음.
2. 메인 카메라 + **`PnCRayInteractor`** — 레이로 클릭·드래그
3. (선택) **`GameStatePersistence`** — 방 클리어 등에서 `SaveToDisk()` 호출해 JSON 저장
4. **Build Settings**에 이동할 **씬 이름**을 등록 (`InteractableLoadScene`·`SceneTransition`에서 이름 문자열과 일치).  
   리포지토리 기본값: `SampleScene`, `Verification_End` 가 `ProjectSettings/EditorBuildSettings.asset` 에 포함되어 있습니다. 새 방 씬을 추가하면 **File → Build Settings**에서 같이 넣어 주세요.

## 담당 씬에서 쓰는 컴포넌트

- **`Interactable`** + Collider — `onInteract`에 퍼즐/대사/씬 전환 연결
- **`InteractableLoadScene`** — `Load()` → 페이드 후 `LoadScene(Single)`. 옵션으로 로드 전 `GameStatePersistence.SaveToDisk()`
- **`InteractableShowLine`** — `Show()` → `NarrativeTextBox`에 한 줄 (유품·일기 클릭 대사 등)
- **`InteractableSetFlag`** — `Apply()` → `FlagStore`에 키/값 기록
- **`InteractableGiveItem`** — `Interactable.onInteract` → `Give()` (유품·쪽지 ID는 팀 규약으로 통일)
- **`InteractableRequireItem`** — 특정 `itemId` 있을 때만 성공 이벤트
- **`WorldDraggable`** + **`WorldDropSlot`** — 기획서의 드래그 앤 드롭 퍼즐
- **`CognitiveFilterRoomState`** — 방 클리어 시 `RevealReality()` → 플래그 `cognitive.real.{roomKey}` (후처리·BGM 교체는 이 플래그 또는 `UnityEvent`로 연결)
- **`NarrativeTextBox`** — 메뉴로 생성하거나 Canvas + TMP + Button 수동 연결 후 `ShowLine` / `ShowSequence` 호출
- **`SceneTransition`** — 코드에서 `SceneTransition.Instance.LoadScene("씬이름")` 또는 `InteractableLoadScene`으로 호출. 내부에서 **`ScreenFadeOverlay`**(검은 페이드) 사용

## 플래그·인벤토리 규약 (예시)

- 인지 필터: `cognitive.real.kitchen`, `cognitive.real.living`, … (`CognitiveFilterRoomState`의 Room Key와 동일)
- 엔딩·분기: `ending.bad`, `puzzle.kitchen.a.done` 등 **문자열은 팀 노션/깃허브 Projects에 목록화** 권장
- 아이템 ID: `note.safe_code`, `item.key_kitchen` … (다른 스테이지에서 쓰는 아이템은 회의록대로 담당자와 합의 후 추가)

## Git / 협업 (3·20 피드백 반영)

클론·pull·push·브랜치 습관은 **루트 [`README.md`](../../../README.md)** 의 **「GitHub (비공개 팀)」** 절을 기준으로 맞춘다. 저장소에 **처음 올릴 때**는 같은 문서의 **「최초 푸시 체크리스트」** 를 따른다.

- **주기적으로 Pull / Push** — `main`은 항상 실행 가능한 상태를 목표
- **GitHub Projects**로 태스크·담당·상태 관리 권장
- **브랜치**: 방별 `scene/kitchen` 등으로 나누고 PR로 머지
- 커서 공용 계정·Pro는 팀 내부 규칙에 따름 (발표 시 AI 사용 여부는 과제 지침 준수)

## JSON 세이브

`Application.persistentDataPath` 아래 `dollhouse_save.json` — 플래그 전체 + 인벤토리 ID 목록. 퍼즐 클리어·중요 분기 후 `GameStatePersistence.SaveToDisk()`를 호출하면 됩니다.

## 검증용 미니 플로우 (팀 확인용)

Unity 에디터 메뉴 **Team Distribution → 검증용 미니 플로우 — 씬에 적용 (1회)** 실행:

1. `SampleScene`에 `TeamDistributionRoot`, 내러티브 UI, 메인 카메라에 `PnCRayInteractor`, 클릭용 큐브 `VerificationDemoCube`, **`TeamDistributionTestHud`** 가 생깁니다.  
2. `Verification_End` 씬이 생성되고 **Build Settings** 맨 앞에 `SampleScene`·`Verification_End` 가 들어갑니다 (이미 있던 다른 씬은 뒤로 밀려 유지).  
3. **Play** → **F3**으로 테스트 패널을 연 뒤 버튼으로 점검하거나, 큐브 클릭 → 「다음」 → 큐브 재클릭으로 `Verification_End` 까지 확인합니다. (패널은 기본 숨김이라 레이·내러티브 작업을 가리지 않습니다.)

인스펙터에서 **Toggle Key**·**씬 이름**·테스트용 플래그/아이템 ID를 바꿀 수 있습니다. **빌드 플레이어**에서 HUD를 쓰려면 `Include In Player Build`를 켜세요 (배포 전에는 끄는 것을 권장).

### 화면에 아무것도 안 나올 때

- **에디터 Play**: `TeamDistributionTestHud`는 **TMP 없이** uGUI `Text`만 씁니다. **기본적으로 씬에 자동 생성하지 않습니다** — 메뉴 **Team Distribution → Play 시 테스트 HUD 자동 생성**을 **켠 경우에만** HUD가 없을 때 스폰됩니다 (`EventSystem`도 같이 생성). 수동으로는 **GameObject → Team Distribution → Test HUD** 를 쓰면 됩니다.  
- **콘솔**에 `[TestHud] UI 생성 실패` 로그가 있으면 내용을 확인하세요.  
- **3D만 안 보임** (하늘만 보임): 카메라 앞에 오브젝트가 있는지, `Verification` 셋업을 돌렸는지 확인하세요. **UI는 카메라와 무관**하게 Overlay로 그려집니다.

> 최초 1회만 실행하면 됩니다. 다시 실행하면 큐브만 갱신됩니다.

## 다음 단계 (기획서 대비)

- 글리치(0.2~0.5초 화면 번쩍임): `CognitiveFilterRoomState` 이벤트에 **Post Process / 애니** 또는 **DOTween** (`DOShakePosition`, `CanvasGroup.DOFade` 등) 연결
- 오토 세이브 타이밍: 방 클리어·씬 로드 직후 등 팀에서 고정
- 크로스 스테이지 아이템: 인벤에 넣은 뒤 다른 씬의 `InteractableRequireItem`에서 동일 ID 사용
