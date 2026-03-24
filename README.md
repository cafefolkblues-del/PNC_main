# PnC (인형의 집 프로토타입)

## Cursor로 이 폴더를 **처음** 연 경우

이 파일이 자동으로 열리도록 워크스페이스 설정이 되어 있습니다(탭 복원만 켜 두었다면 안 뜰 수 있음 — 그때는 사이드바에서 `README.md`를 연다).

1. [BEGINNER_CURSOR_KR.md](Assets/TeamDistribution/Docs/BEGINNER_CURSOR_KR.md) — Unity·Cursor 처음일 때  
2. [TEAM_BUILD.md](Assets/TeamDistribution/Docs/TEAM_BUILD.md) — 코어·메뉴·빌드 전체  

AI에게 같은 순서를 강하게 넣으려면 새 채팅에서 `@read-docs-first` 를 쓰거나 [`AGENTS.md`](AGENTS.md)를 컨텍스트에 넣으면 됩니다.

### Cursor / AI용 요약 (복붙·@첨부용)

- **Unity**: `ProjectSettings/ProjectVersion.txt` 기준 에디터 버전 맞출 것.  
- **팀 코어**: `Assets/TeamDistribution/` — `Scripts/` 런타임, `Demo/` 검증·테스트 HUD, `Editor/` 메뉴만, `Docs/` 문서.  
- **네임스페이스**: `TeamDistribution`. 싱글톤·진입점: `TeamDistributionRoot`, `FlagStore`, `InventorySystem`, `SceneTransition`.  
- **상호작용**: `Interactable` + Collider, 파생 컴포넌트(`InteractableLoadScene` 등). 플래그·아이템 ID는 문자열 규약 — `TEAM_BUILD.md` 표 참고.  
- **금지**: `Library/`·`Temp/`·`Logs/` 커밋, 생성된 `*.csproj`/`*.sln` 커밋(`.gitignore` 처리됨).  
- **Git**: 작업 전 `git pull`, Unity는 가능하면 **저장 후** 풀/머지. 상세는 아래 **GitHub (비공개 팀)** 절.

---

Unity **2022.3 LTS** 기준 포인트앤클릭 팀 공용 코어와 검증 씬이 포함된 프로젝트입니다.

## 코드 구조·유지보수 (요약)

| 판단 | 설명 |
|------|------|
| **양호** | 런타임(`Scripts`)·데모(`Demo`)·에디터(`Editor`)·문서(`Docs`) 분리. 단일 네임스페이스로 검색·리팩터가 쉬움. |
| **양호** | `TeamDistributionRoot`가 공용 싱글톤·EventSystem·씬 전환 생성을 한곳에서 맡음 — 씬마다 중복 배치 최소화. |
| **양호** | `Interactable` + 작은 전용 컴포넌트 조합 — 방 담당자가 Inspector만으로 퍼즐을 붙이기 쉬운 편. |
| **주의** | 플래그·아이템 ID가 **문자열**이라 오타·중복 정의가 생기기 쉬움 → 노션/이슈에 **키 목록**을 두고 `TEAM_BUILD.md`와 맞출 것. |
| **주의** | `.unity` / 프리팹은 **동시 편집 시 머지 충돌**이 잦음 → 방·기능 단위 브랜치, 큰 씬 작업은 한 명씩 또는 짧은 PR 권장. |

기능이 더 늘면 `Scripts/` 아래에 `Interaction/`, `UI/` 같은 하위 폴더만 나누어도 됨(지금 규모에서는 평면 구조도 무방).

## 빠른 시작

1. Unity Hub에서 이 폴더를 프로젝트로 엽니다.  
2. **Unity가 낯설면** 먼저 [`Assets/TeamDistribution/Docs/BEGINNER_CURSOR_KR.md`](Assets/TeamDistribution/Docs/BEGINNER_CURSOR_KR.md) 를 읽습니다.  
3. 기능·메뉴 전체는 [`Assets/TeamDistribution/Docs/TEAM_BUILD.md`](Assets/TeamDistribution/Docs/TEAM_BUILD.md) 를 참고합니다.  
4. TextMesh Pro / DOTween 설정은 `TEAM_BUILD.md` 해당 절을 따릅니다.

## GitHub (비공개 팀) — 받기 / 올리기

1. **최초 받기**  
   - GitHub에서 **Private** 저장소 생성 후, 오너가 이 프로젝트 폴더를 푸시.  
   - 팀원: `git clone <저장소 URL>` 후 **Unity Hub**에서 클론한 폴더를 프로젝트로 연다. 첫 열 때 `Library/`는 로컬에서 다시 생성된다.

2. **작업 전**  
   - `git pull` (또는 Cursor/VS Code 소스 제어에서 Pull).  
   - **Unity 에디터가 같은 프로젝트를 열고 있으면**, 풀/머지 전에 씬·프리팹 저장 후 닫는 것이 충돌·덮어쓰기를 줄이는 데 유리하다.

3. **작업 후**  
   - `git status`로 **`Library/`**, `Temp/`, `Logs/`가 올라가지 않는지 확인(`.gitignore`에 있음).  
   - 변경이 의도한 에셋·스크립트·`ProjectSettings`만인지 본 뒤 커밋.  
   - `git push` (브랜치가 `main`이 아니면 원하는 브랜치로 푸시 후 PR).

4. **브랜치 권장**  
   - `main`: 항상 열리는 상태 유지 목표.  
   - 개인/방 단위: `feature/kitchen-scene` 등으로 나누고 머지하면 씬 충돌을 줄이기 쉽다.

5. **Cursor**  
   - 이 `README.md`가 워크스페이스 열 때 뜨므로, 팀원은 **추가 설정 없이** 동일한 풀·푸시 흐름을 볼 수 있다.  
   - C# 인텔리센스는 Cursor가 **추천 확장 설치** 안내를 띄울 수 있다 — `.vscode/extensions.json`에 C# 확장이 권장으로 적혀 있다.

### 최초 푸시 체크리스트 (저장소 오너)

로컬에서 Git을 처음 연결해 올릴 때 아래를 순서대로 확인하면 된다.

| 단계 | 할 일 |
|------|--------|
| 1 | **`.gitignore`**가 있는지 확인한다. 이 프로젝트는 `Library/`, `Temp/`, `Logs/`, **`UserSettings/`**, `Builds/` 등이 제외되도록 되어 있다. |
| 2 | `git status`로 **추적 대상(Staged/커밋될 파일)**에 `Library/`, `UserSettings/`, `Temp/`가 **섞이지 않았는지** 본다. 섞여 있으면 `.gitignore`를 고치고, 이미 추적 중이면 `git rm -r --cached <경로>`로 캐시만 제거한 뒤 다시 커밋한다. |
| 3 | **반드시 올릴 것**: `Assets/`(에셋·스크립트·`.meta`), `Packages/`(`manifest.json` 등), `ProjectSettings/`, 루트 `README.md`·`AGENTS.md`·`.gitignore`·`.cursor/`·`.vscode/`(팀이 공유하기로 한 것만). |
| 4 | **대용량 파일**: 수백 MB 영상, PSD 원본, 빌드 산출물 등을 자주 올릴 계획이면 팀에서 **Git LFS** 사용 여부를 정한다. LFS 없이 큰 바이너리를 계속 커밋하면 저장소·클론이 무거워진다. (지금 규모만이면 생략 가능.) |
| 5 | GitHub에서 **Private** 저장소 생성 → `git init`(아직 없다면) → `git add` / `git commit` → `git remote add origin <URL>` → `git branch -M main` → `git push -u origin main`. |
| 6 | 팀원에게 **저장소 URL**과 Unity 버전(`ProjectSettings/ProjectVersion.txt`와 동일하게 Hub에서 설치)을 공유한다. |

### 푸시·풀 “자동화” — 어디까지 가능한가

| 방식 | 가능 여부 | 비고 |
|------|-----------|------|
| **폴더 열 때마다 자동 `git pull`** | 기술적으로는 가능 (`tasks` + `runOn: folderOpen`) | Unity가 에셋을 잡고 있는 상태에서 원격 변경이 들어오면 **충돌·손상 위험**이 커서 **비권장**. |
| **주기적 자동 push/pull** | 스크립트·봇으로 가능 | 같은 이유로 Unity 협업에서는 **거의 쓰지 않음**. |
| **원클릭 Pull / Push** | ✅ 권장 | 아래 **Tasks** — 사람이 “Unity 저장·닫기 → Pull” 타이밍을 정함. |
| **GitHub Actions (CI)** | ✅ 별개 축 | **원격에 push된 뒤** 빌드·테스트만 자동. 로컬 풀을 대신해 주지는 않음. |

**Cursor / VS Code에서 원클릭** — 명령 팔레트(`Ctrl+Shift+P` / `Cmd+Shift+P`) → **Tasks: Run Task** → 아래 중 선택:

- `Git: Pull (현재 브랜치)`
- `Git: Push (현재 브랜치)`
- `Git: 상태 (status)`

정의는 `.vscode/tasks.json`에 있으며, 팀원 모두 동일하게 쓸 수 있다. **커밋은 자동화하지 않는다** — 메시지·스테이징은 소스 제어 패널에서 하는 편이 안전하다.

## 서드파티 (재배포 시 확인)

| 구성요소 | 비고 |
|----------|------|
| **DOTween** (`Assets/Plugins/Demigiant/DOTween`) | 저작권 Demigiant — [라이선스](http://dotween.demigiant.com/license.php), `readme.txt` 유지 |
| **TextMesh Pro** (패키지 + `Assets/TextMesh Pro`) | Unity / LiberationSans OFL, EmojiOne 샘플은 `EmojiOne Attribution.txt` 준수 |

## 라이선스

**비공개 팀 저장소**면 팀 내부 규칙으로 충분하고, 루트 `LICENSE`는 필수는 아님. 나중에 **공개**할 때만 `LICENSE`를 추가하면 된다.
