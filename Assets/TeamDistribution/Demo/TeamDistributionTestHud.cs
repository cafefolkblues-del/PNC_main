using TeamDistribution.Demo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamDistribution
{
    /// <summary>테스트 전용 패널. 기본은 숨김 — 토글 키로만 연다(작업 방해 최소화). TMP 없이 uGUI Text 사용.</summary>
    public sealed class TeamDistributionTestHud : MonoBehaviour
    {
        /// <summary>에디터 Play 시 자동 생성 여부 (EditorPrefs). 메뉴와 동일 키.</summary>
        public const string EditorAutoSpawnPrefKey = "TeamDistribution.AutoSpawnTestHud";

        [SerializeField] KeyCode _toggleKey = KeyCode.F3;

        [Tooltip("켜면 Play 직후 패널이 보입니다. 끄면 토글 키로만 엽니다(기본 권장).")]
        [SerializeField] bool _startVisible;

        [Tooltip("체크 시 빌드 플레이어에서도 HUD가 살아 있습니다. 배포 전 해제 권장.")]
        [SerializeField] bool _includeInPlayerBuild;

        [SerializeField] string _narrativeTestLine = "[테스트 HUD] NarrativeTextBox 호출이 정상입니다.";
        [SerializeField] string _loadSceneName = "Verification_End";
        [SerializeField] string _testFlagKey = "test.hud.flag";
        [SerializeField] string _testItemId = "test.hud.item";

        CanvasGroup _rootGroup;
        Text _statusText;

        static Font s_builtinFont;

        static Font BuiltinFont()
        {
            if (s_builtinFont != null) return s_builtinFont;
            s_builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (s_builtinFont == null) s_builtinFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return s_builtinFont;
        }

        void Awake()
        {
#if !UNITY_EDITOR
            if (!_includeInPlayerBuild)
            {
                enabled = false;
                return;
            }
#endif
            try
            {
                BuildUi();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[TestHud] UI 생성 실패: {e.Message}\n{e.StackTrace}");
            }

            ApplyStartVisibility();
        }

        void ApplyStartVisibility()
        {
            if (_rootGroup == null) return;
            if (_startVisible) return;
            _rootGroup.alpha = 0f;
            _rootGroup.blocksRaycasts = false;
            _rootGroup.interactable = false;
        }

        void Start()
        {
            if (!enabled) return;
            RefreshStatus();
        }

        void Update()
        {
            if (!enabled) return;
            if (Input.GetKeyDown(_toggleKey)) ToggleVisible();
        }

        void ToggleVisible()
        {
            if (_rootGroup == null) return;
            bool show = _rootGroup.alpha < 0.5f;
            _rootGroup.alpha = show ? 1f : 0f;
            _rootGroup.blocksRaycasts = show;
            _rootGroup.interactable = show;
            if (show) RefreshStatus();
        }

        void BuildUi()
        {
            var canvasGo = new GameObject("TestHudCanvas");
            canvasGo.transform.SetParent(transform, false);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 15000;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            _rootGroup = canvasGo.AddComponent<CanvasGroup>();

            var panel = new GameObject("Panel");
            panel.transform.SetParent(canvasGo.transform, false);
            var panelRt = panel.AddComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0.08f, 0.12f);
            panelRt.anchorMax = new Vector2(0.92f, 0.88f);
            panelRt.offsetMin = Vector2.zero;
            panelRt.offsetMax = Vector2.zero;

            var panelImg = panel.AddComponent<Image>();
            panelImg.color = new Color(0.05f, 0.06f, 0.1f, 0.96f);

            var vlg = panel.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(20, 20, 16, 16);
            vlg.spacing = 10f;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlHeight = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;

            AddLabel(panel.transform, "TeamDistribution 테스트", 26, FontStyle.Bold, Color.white);
            AddLabel(panel.transform, $"「{_toggleKey}」 패널 숨기기 · 다시 열기", 15, FontStyle.Normal, new Color(0.78f, 0.82f, 0.9f));

            _statusText = AddLabel(panel.transform, "상태: —", 14, FontStyle.Normal, new Color(0.7f, 0.75f, 0.85f));
            _statusText.horizontalOverflow = HorizontalWrapMode.Wrap;
            _statusText.verticalOverflow = VerticalWrapMode.Overflow;

            AddButton(panel.transform, "대사 테스트 (NarrativeTextBox)", () =>
            {
                NarrativeTextBox.Instance?.ShowLine(_narrativeTestLine);
                RefreshStatus();
            });

            AddButton(panel.transform, $"씬 로드: {_loadSceneName}", () =>
            {
                if (SceneTransition.Instance != null)
                    SceneTransition.Instance.LoadScene(_loadSceneName);
                else
                    SceneManager.LoadScene(_loadSceneName);
                RefreshStatus();
            });

            AddButton(panel.transform, "큐브와 동일 (1·2단계)", () =>
            {
                var flow = FindObjectOfType<VerificationMiniFlow>();
                if (flow != null) flow.OnDemoClicked();
                else Debug.LogWarning("[TestHud] VerificationMiniFlow 없음 — 큐브 씬에서 사용하세요.");
                RefreshStatus();
            });

            AddButton(panel.transform, "플래그 +1 (test)", () =>
            {
                if (FlagStore.Instance != null)
                    FlagStore.Instance.Add(_testFlagKey, 1);
                RefreshStatus();
            });

            AddButton(panel.transform, "테스트 아이템 지급", () =>
            {
                InventorySystem.Instance?.Add(_testItemId);
                RefreshStatus();
            });

            AddButton(panel.transform, "세이브 (JSON)", () =>
            {
                var p = GetOrCreatePersistence();
                p.SaveToDisk();
                Debug.Log($"[TestHud] 저장됨: {Application.persistentDataPath}");
                RefreshStatus();
            });

            AddButton(panel.transform, "로드 (JSON)", () =>
            {
                var p = FindObjectOfType<GameStatePersistence>();
                if (p == null)
                {
                    Debug.LogWarning("[TestHud] GameStatePersistence 없음 — 세이브 버튼으로 생성 후 다시 시도.");
                    RefreshStatus();
                    return;
                }

                bool ok = p.LoadFromDisk();
                Debug.Log(ok ? "[TestHud] 로드 성공" : "[TestHud] 저장 파일 없음");
                RefreshStatus();
            });

            AddButton(panel.transform, "상태 새로고침", RefreshStatus);

            if (Object.FindObjectOfType<EventSystem>(true) == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }
        }

        static GameStatePersistence GetOrCreatePersistence()
        {
            var p = FindObjectOfType<GameStatePersistence>();
            if (p != null) return p;
            var go = new GameObject("GameStatePersistence");
            return go.AddComponent<GameStatePersistence>();
        }

        Text AddLabel(Transform parent, string text, int size, FontStyle style, Color color)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = size + 14;
            le.preferredWidth = 800f;

            var t = go.AddComponent<Text>();
            t.font = BuiltinFont();
            t.text = text;
            t.fontSize = size;
            t.fontStyle = style;
            t.color = color;
            t.alignment = TextAnchor.MiddleLeft;
            t.raycastTarget = false;
            if (t.font == null)
                Debug.LogWarning("[TestHud] 내장 폰트를 찾지 못했습니다. 글자가 안 보일 수 있습니다.");
            return t;
        }

        void AddButton(Transform parent, string label, UnityAction onClick)
        {
            var go = new GameObject("Btn_" + label.GetHashCode());
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 44f;
            le.preferredHeight = 44f;

            var img = go.AddComponent<Image>();
            img.color = new Color(0.22f, 0.24f, 0.32f, 1f);

            var btn = go.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.32f, 0.35f, 0.45f);
            btn.colors = colors;
            btn.onClick.AddListener(onClick);

            var labelGo = new GameObject("Text");
            labelGo.transform.SetParent(go.transform, false);
            var labelRt = labelGo.AddComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;

            var t = labelGo.AddComponent<Text>();
            t.font = BuiltinFont();
            t.text = label;
            t.fontSize = 17;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.raycastTarget = false;
        }

        void RefreshStatus()
        {
            if (_statusText == null) return;

            string scene = SceneManager.GetActiveScene().name;
            int flag = FlagStore.Instance != null ? FlagStore.Instance.Get(_testFlagKey) : -1;
            bool inv = InventorySystem.Instance != null && InventorySystem.Instance.Has(_testItemId);
            bool ntb = NarrativeTextBox.Instance != null;
            bool st = SceneTransition.Instance != null;

            _statusText.text =
                $"씬: {scene}\n" +
                $"NarrativeTextBox: {(ntb ? "OK" : "없음")}  |  SceneTransition: {(st ? "OK" : "없음")}\n" +
                $"플래그 {_testFlagKey} = {flag}  |  아이템 {_testItemId}: {(inv ? "보유" : "없음")}";
        }
    }

#if UNITY_EDITOR
    static class TeamDistributionPlayHudAutoSpawn
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void SpawnIfNeeded()
        {
            if (!UnityEditor.EditorPrefs.GetBool(TeamDistributionTestHud.EditorAutoSpawnPrefKey, false)) return;
            if (Object.FindObjectOfType<TeamDistributionTestHud>(true) != null) return;

            if (Object.FindObjectOfType<EventSystem>(true) == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            var go = new GameObject("TeamDistributionTestHud");
            go.AddComponent<TeamDistributionTestHud>();
        }
    }
#endif
}
