#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace TeamDistribution.Editor
{
    /// <summary>Window → TextMeshPro → Import TMP Essential Resources 와 동일. 한 번 실행하면 됩니다.</summary>
    public static class TmpEssentialsImporter
    {
        const string TmpSettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";

        [MenuItem("Team Distribution/TextMesh Pro — Essential Resources 임포트", false, 35)]
        public static void ImportTmpEssentials()
        {
            if (File.Exists(TmpSettingsPath))
            {
                EditorUtility.DisplayDialog(
                    "TextMesh Pro",
                    "TMP Essential Resources가 이미 임포트되어 있습니다.\n\n" + TmpSettingsPath,
                    "확인");
                return;
            }

            try
            {
                RunImport();
                EditorUtility.DisplayDialog(
                    "TextMesh Pro",
                    "TMP Essential Resources 임포트를 요청했습니다.\n콘솔에 오류가 없는지 확인하세요.",
                    "확인");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                EditorUtility.DisplayDialog(
                    "TextMesh Pro",
                    "임포트 실패. Unity 메뉴에서 직접 실행해 보세요:\n" +
                    "Window → TextMeshPro → Import TMP Essential Resources",
                    "확인");
            }
        }

        /// <summary>배치/CI용. 프로젝트가 다른 Unity에 열려 있으면 실패할 수 있음.</summary>
        public static void ImportTmpEssentialsSilent()
        {
            if (File.Exists(TmpSettingsPath)) return;
            try
            {
                RunImport();
            }
            catch (System.Exception e)
            {
                Debug.LogError("[TMP] ImportTmpEssentialsSilent: " + e);
            }
        }

        static void RunImport()
        {
            TMP_PackageResourceImporter.ImportResources(importEssentials: true, importExamples: false, interactive: false);
            AssetDatabase.Refresh();
        }
    }
}
#endif
