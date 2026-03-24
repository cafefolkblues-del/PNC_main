#if UNITY_EDITOR
using System.Collections.Generic;
using TeamDistribution;
using TeamDistribution.Demo;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamDistribution.Editor
{
    /// <summary>SampleScene에 검증 큐브·UI·카메라 레이를 넣고, Verification_End 씬과 빌드 설정을 맞춥니다.</summary>
    public static class VerificationMiniFlowSetup
    {
        const string SampleScenePath = "Assets/Scenes/SampleScene.unity";
        const string EndScenePath = "Assets/Scenes/Verification_End.unity";
        const string DemoCubeName = "VerificationDemoCube";

        [MenuItem("Team Distribution/검증용 미니 플로우 — 씬에 적용 (1회)", false, 50)]
        public static void ApplyVerificationFlow()
        {
            if (!EditorUtility.DisplayDialog(
                    "검증용 미니 플로우",
                    "SampleScene에 검증 큐브·내러티브 UI·PnCRayInteractor를 추가하고,\n" +
                    "Verification_End 씬을 만든 뒤 Build Settings 맨 앞에\n" +
                    "두 씬을 끼워 넣습니다 (기존에 등록된 다른 씬은 유지).\n\n" +
                    "계속할까요?",
                    "적용",
                    "취소"))
                return;

            if (!System.IO.File.Exists(SampleScenePath))
            {
                EditorUtility.DisplayDialog("오류", $"파일이 없습니다: {SampleScenePath}", "확인");
                return;
            }

            EditorSceneManager.OpenScene(SampleScenePath);

            EnsureTeamDistributionRoot();
            EnsurePnCRayInteractorOnMainCamera();
            EnsureNarrativeUi();
            EnsureDemoCube();
            EnsureTestHud();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();

            CreateOrUpdateEndScene();

            EditorSceneManager.OpenScene(SampleScenePath);

            ApplyBuildSettings();

            EditorUtility.DisplayDialog(
                "완료",
                "Play 후 F3으로 테스트 HUD를 열어 버튼으로도 동일 검증이 가능합니다.\n" +
                "또는 큐브 클릭 → 「다음」 → 큐브 재클릭으로 Verification_End까지 확인하세요.",
                "확인");
        }

        static void EnsureTeamDistributionRoot()
        {
            if (Object.FindObjectOfType<TeamDistributionRoot>() != null) return;

            var go = new GameObject("TeamDistributionRoot");
            Undo.RegisterCreatedObjectUndo(go, "Verification Flow");
            go.AddComponent<TeamDistributionRoot>();
        }

        static void EnsurePnCRayInteractorOnMainCamera()
        {
            var cam = Camera.main;
            if (cam == null) return;
            if (cam.GetComponent<PnCRayInteractor>() != null) return;
            Undo.AddComponent<PnCRayInteractor>(cam.gameObject);
        }

        static void EnsureNarrativeUi()
        {
            if (Object.FindObjectOfType<NarrativeTextBox>() != null) return;
            TeamDistributionMenu.CreateNarrativeCanvas("Verification Flow — Narrative UI");
        }

        static void EnsureDemoCube()
        {
            var old = GameObject.Find(DemoCubeName);
            if (old != null) Undo.DestroyObjectImmediate(old);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = DemoCubeName;
            Undo.RegisterCreatedObjectUndo(cube, "Verification Flow");
            cube.transform.position = new Vector3(0f, 0.5f, 0f);
            cube.transform.localScale = Vector3.one * 1.2f;

            var interactable = cube.AddComponent<Interactable>();
            var flow = cube.AddComponent<VerificationMiniFlow>();
            UnityEventTools.AddPersistentListener(interactable.onInteract, flow.OnDemoClicked);
            EditorUtility.SetDirty(interactable);
        }

        static void EnsureTestHud()
        {
            if (Object.FindObjectOfType<TeamDistributionTestHud>() != null) return;

            var go = new GameObject("TeamDistributionTestHud");
            Undo.RegisterCreatedObjectUndo(go, "Verification Flow");
            var hud = go.AddComponent<TeamDistributionTestHud>();
            var so = new SerializedObject(hud);
            so.FindProperty("_startVisible").boolValue = false;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void CreateOrUpdateEndScene()
        {
            var sc = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            var canvasGo = new GameObject("EndScreenCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("Panel");
            panel.transform.SetParent(canvasGo.transform, false);
            var panelRt = panel.AddComponent<RectTransform>();
            StretchFull(panelRt);
            var img = panel.AddComponent<Image>();
            img.color = new Color(0.08f, 0.1f, 0.14f, 1f);

            var textGo = new GameObject("Message");
            textGo.transform.SetParent(panel.transform, false);
            var textRt = textGo.AddComponent<RectTransform>();
            StretchFull(textRt);
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text =
                "검증 완료\n\n" +
                "Verification_End 씬까지 도착했습니다.\n" +
                "페이드 · 씬 전환 · NarrativeTextBox · PnCRayInteractor · TeamDistributionRoot 가 정상입니다.";
            tmp.fontSize = 34;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            EditorSceneManager.SaveScene(sc, EndScenePath);
        }

        static void ApplyBuildSettings()
        {
            var tail = new List<EditorBuildSettingsScene>();
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (s.path == SampleScenePath || s.path == EndScenePath) continue;
                tail.Add(s);
            }

            var merged = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene(SampleScenePath, true),
                new EditorBuildSettingsScene(EndScenePath, true)
            };
            merged.AddRange(tail);
            EditorBuildSettings.scenes = merged.ToArray();
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
