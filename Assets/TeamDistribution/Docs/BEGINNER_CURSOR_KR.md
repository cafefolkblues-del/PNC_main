# Unity 거의 처음 + Cursor로만 개발할 때 — 온보딩

이 프로젝트의 **배포용 코어**는 “중급자가 빠르게 쓰기”엔 괜찮지만, **Unity를 거의 안 만져본 상태에서 Cursor만 믿고 가기**에는 원래 **친절함이 부족**합니다.  
그래서 이 문서를 따로 두었습니다. **이 파일 → 아래 순서대로 한 번만** 밟고, 막히면 `TEAM_BUILD.md`를 펼치면 됩니다.

---

## 1. 꼭 알아야 하는 Unity 단어 (최소)

| 용어 | 한 줄 |
|------|--------|
| **씬(Scene)** | 게임 한 판의 “무대” 파일. 방마다 씬을 나누는 경우가 많음. |
| **게임 오브젝트** | 씬 안의 사물(빈 오브젝트, 큐브, UI 패널 등). |
| **컴포넌트** | 오브젝트에 붙는 기능 조각(스크립트, Collider, Camera 등). |
| **Inspector** | 오른쪽 패널. 선택한 오브젝트의 컴포넌트·값이 보임. |
| **프리팹(Prefab)** | 재사용 가능한 오브젝트 템플릿. (이번 코어는 “메뉴로 비슷한 걸 생성” 위주) |
| **Console** | **Window → General → Console** — 빨간 에러가 나오면 **여기부터** 본다. |
| **Play 버튼** | 실행. 끄면(Play 다시 누름) **씬에 한 편집은 대부분 원래대로** 돌아감에 주의. |

---

## 2. 처음 하루에 할 일 (순서 고정 추천)

1. **Unity Hub**로 이 프로젝트 폴더를 연다. (버전은 팀이 정한 LTS, README 참고)  
2. 첫 열림은 **import가 오래** 걸릴 수 있음. 기다린다.  
3. **Git**: 작업 전 `pull`, 작업 후 `push` 습관. **`Library` 폴더는 커밋하지 않음** (`.gitignore`에 있음).  
4. 메뉴 **Team Distribution → TextMesh Pro — Essential Resources 임포트** (아직 안 했다면).  
5. **검증 씬**으로 동작 확인: `Team Distribution → 검증용 미니 플로우 — 씬에 적용 (1회)` 후 `SampleScene` Play → 큐브·대사·씬 전환이 되는지 본다.  
6. **내 담당 씬**을 연다. 씬이 없으면 **팀장/혁에게 씬 파일 이름**을 물어보고, `Assets/Scenes` 등에 만들 예정인지 확인.  
7. 씬에 **빈 오브젝트** 하나 두고 이름을 `TeamBootstrap` 같이 정한 뒤 **`TeamDistributionRoot`** 컴포넌트를 붙인다 (또는 메뉴 **GameObject → Team Distribution → Team Distribution Root**).  
8. **Main Camera**에 **`PnCRayInteractor`** 가 있는지 확인. 없으면 Add Component.  
9. 대사가 필요하면 **GameObject → Team Distribution → Narrative Text UI**.  
10. 클릭할 물체에는 **Collider** + **`Interactable`**, Inspector에서 **`onInteract`** 에 함수 연결(버튼처럼 눌러서 고름).

이후부터는 **Cursor에게 “이 씬에서 ○○ 하고 싶다”**고 말하면서 `TEAM_BUILD.md`의 컴포넌트 표를 참고하게 하면 됩니다.

---

## 3. Cursor에 복붙해서 쓰기 좋은 말투 (예시)

프로젝트를 열어둔 채로, 대략 이런 식으로 시작하면 안전합니다.

```
이 프로젝트는 Unity 2022 LTS이고, Assets/TeamDistribution/Docs/TEAM_BUILD.md 에 공용 시스템 규칙이 있어.
TeamDistribution 네임스페이스 스크립트는 함부로 삭제하지 말고, 내 담당 씬 이름은 "XXX"야.
Interactable + Collider로 클릭을 받고, FlagStore / InventorySystem 싱글톤을 쓰고 싶어.
```

```
NarrativeTextBox.Instance.ShowLine("...") 를 Interactable의 onInteract에서 호출하고 싶은데,
씬에 NarrativeCanvas가 이미 있어. 필요한 연결만 C#으로 추가해 줘.
```

```
Library 폴더는 건드리지 말고, 새 스크립트는 Assets/ 아래 우리 방 폴더에만 추가해 줘.
```

**주의:** Cursor가 **전체 리팩터**나 **패키지 삭제**를 제안하면, **한 번 멈추고** 팀에 물어본 뒤 진행하는 게 좋습니다.

---

## 4. 자주 터지는 실수 (미리 방지)

| 증상 | 원인 후보 |
|------|-----------|
| 클릭해도 반응 없음 | Collider 없음, **`PnCRayInteractor`** 가 카메라에 없음, UI가 화면을 덮어 **EventSystem**만 먹는 경우 |
| `Instance` 가 null | **`TeamDistributionRoot`** 가 씬에 없거나, Play 전에 싱글톤이 아직 생성 안 됨 |
| 씬 전환 에러 | **File → Build Settings**에 그 씬 이름이 **체크**되어 있는지 |
| 글자가 안 보임 / 분홍색 | TMP Essential 미임포트 → 위 메뉴로 임포트 |
| Git 충돌 지옥 | **같은 씬 파일**을 두 명이 동시에 크게 수정. 가능하면 **방 씬 분리** 또는 **한 명이 씬 머지** |

---

## 5. “친절한가?”에 대한 솔직한 답

- **지금 상태**: 스크립트·메뉴·`TEAM_BUILD.md`는 **“읽을 사람이 Unity 기본 조작을 알고 있다”**는 전제에 가깝습니다.  
- **이 문서 + 검증 플로우 1회 + 위 체크리스트**까지 하면, **Cursor만 적극 쓰는 초보**도 **방 하나 붙이는 수준**은 따라갈 수 있게 맞춰 둔 상태입니다.  
- **그 이상**(애니메이션 파이프라인, 라이팅, 빌드 서명, Addressables 등)은 이 배포본 범위 밖이라, **별도 튜토리얼이나 담당(혁) Q&A**가 필요합니다.

---

## 6. 다음으로 읽을 것

- 기능·API 목록: **`TEAM_BUILD.md`**  
- 저장소 전체: 루트 **`README.md`**

질문은 **콘솔 에러 문장 전체를 캡처**해서 팀 채널에 올리면 답이 빨라집니다.
