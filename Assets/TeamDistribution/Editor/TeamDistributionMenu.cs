#if UNITY_EDITOR
using TeamDistribution;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamDistribution.Editor
{
    public static class TeamDistributionMenu
    {
        const string RootMenu = "GameObject/Team Distribution/";
        [MenuItem("Team Distribution/Play 시 테스트 HUD 자동 생성", false, 40)]
        static void ToggleAutoSpawnTestHud()
        {
            bool v = EditorPrefs.GetBool(TeamDistributionTestHud.EditorAutoSpawnPrefKey, false);
            EditorPrefs.SetBool(TeamDistributionTestHud.EditorAutoSpawnPrefKey, !v);
        }

        [MenuItem("Team Distribution/Play 시 테스트 HUD 자동 생성", true)]
        static bool ToggleAutoSpawnTestHudValidate()
        {
            Menu.SetChecked("Team Distribution/Play 시 테스트 HUD 자동 생성", EditorPrefs.GetBool(TeamDistributionTestHud.EditorAutoSpawnPrefKey, false));
            return true;
        }

        /// <summary>현재 씬에 내러티브 UI를 만들고 루트 오브젝트를 반환합니다.</summary>
        public static GameObject CreateNarrativeCanvas(string undoLabel = "Create Narrative Text UI")
        {
            EnsureEventSystemForEditor();

            var canvasGo = new GameObject("NarrativeCanvas");
            Undo.RegisterCreatedObjectUndo(canvasGo, undoLabel);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("Panel");
            Undo.RegisterCreatedObjectUndo(panel, undoLabel);
            panel.transform.SetParent(canvasGo.transform, false);
            var panelRt = panel.AddComponent<RectTransform>();
            StretchFull(panelRt);
            var panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.55f);

            var panelGroup = panel.AddComponent<CanvasGroup>();
            panelGroup.alpha = 0f;
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;

            var bodyGo = new GameObject("Body");
            Undo.RegisterCreatedObjectUndo(bodyGo, undoLabel);
            bodyGo.transform.SetParent(panel.transform, false);
            var bodyRt = bodyGo.AddComponent<RectTransform>();
            bodyRt.anchorMin = new Vector2(0.08f, 0.25f);
            bodyRt.anchorMax = new Vector2(0.92f, 0.55f);
            bodyRt.offsetMin = Vector2.zero;
            bodyRt.offsetMax = Vector2.zero;

            var body = bodyGo.AddComponent<TextMeshProUGUI>();
            body.text = string.Empty;
            body.fontSize = 32;
            body.alignment = TextAlignmentOptions.TopLeft;
            body.color = Color.white;
            body.raycastTarget = false;

            var btnGo = new GameObject("AdvanceButton");
            Undo.RegisterCreatedObjectUndo(btnGo, undoLabel);
            btnGo.transform.SetParent(panel.transform, false);
            var btnRt = btnGo.AddComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(0.75f, 0.08f);
            btnRt.anchorMax = new Vector2(0.95f, 0.18f);
            btnRt.offsetMin = Vector2.zero;
            btnRt.offsetMax = Vector2.zero;

            var btnImage = btnGo.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.2f, 0.25f, 1f);

            var btn = btnGo.AddComponent<Button>();
            var btnColors = btn.colors;
            btnColors.highlightedColor = new Color(0.35f, 0.35f, 0.42f);
            btn.colors = btnColors;

            var btnLabelGo = new GameObject("Label");
            btnLabelGo.transform.SetParent(btnGo.transform, false);
            var labelRt = btnLabelGo.AddComponent<RectTransform>();
            StretchFull(labelRt);
            var btnLabel = btnLabelGo.AddComponent<TextMeshProUGUI>();
            btnLabel.text = "다음";
            btnLabel.fontSize = 26;
            btnLabel.alignment = TextAlignmentOptions.Center;
            btnLabel.color = Color.white;
            btnLabel.raycastTarget = false;

            var ntb = canvasGo.AddComponent<NarrativeTextBox>();
            var so = new SerializedObject(ntb);
            so.FindProperty("_canvasGroup").objectReferenceValue = panelGroup;
            so.FindProperty("_body").objectReferenceValue = body;
            so.FindProperty("_advanceButton").objectReferenceValue = btn;
            so.ApplyModifiedPropertiesWithoutUndo();

            return canvasGo;
        }

        [MenuItem(RootMenu + "Narrative Text UI", false, 10)]
        static void CreateNarrativeUi()
        {
            var go = CreateNarrativeCanvas();
            Selection.activeGameObject = go;
        }

        [MenuItem(RootMenu + "Team Distribution Root", false, 20)]
        static void CreateRoot()
        {
            var go = new GameObject("TeamDistributionRoot");
            Undo.RegisterCreatedObjectUndo(go, "Create Team Distribution Root");
            go.AddComponent<TeamDistributionRoot>();
            Selection.activeGameObject = go;
        }

        [MenuItem(RootMenu + "Test HUD (인게임 F3)", false, 15)]
        static void CreateTestHud()
        {
            var go = new GameObject("TeamDistributionTestHud");
            Undo.RegisterCreatedObjectUndo(go, "Create Test HUD");
            var hud = go.AddComponent<TeamDistributionTestHud>();
            var so = new SerializedObject(hud);
            so.FindProperty("_startVisible").boolValue = false;
            so.ApplyModifiedPropertiesWithoutUndo();
            Selection.activeGameObject = go;
        }

        [MenuItem(RootMenu + "Scene Transition (DDOL)", false, 21)]
        static void CreateSceneTransition()
        {
            if (Object.FindObjectOfType<SceneTransition>() != null)
            {
                EditorUtility.DisplayDialog("Team Distribution", "씬에 SceneTransition이 이미 있습니다.", "확인");
                return;
            }

            var go = new GameObject("SceneTransition");
            Undo.RegisterCreatedObjectUndo(go, "Create Scene Transition");
            go.AddComponent<SceneTransition>();
            Selection.activeGameObject = go;
        }

        static void EnsureEventSystemForEditor()
        {
            if (Object.FindObjectOfType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
#endif
