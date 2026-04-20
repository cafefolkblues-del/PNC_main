using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 사례분석 폴더의 원본 이미지를 에디터 창 크기에 맞게(비율 유지) 표시합니다.
/// </summary>
public sealed class CaseStudyOriginalWindow : EditorWindow
{
    private const string FolderPath = "Assets/Editor/사례분석";

    private readonly List<Texture2D> _textures = new List<Texture2D>();
    private Vector2 _scroll;

    [MenuItem("Window/사례분석/원본 사진")]
    private static void Open()
    {
        var w = GetWindow<CaseStudyOriginalWindow>();
        w.titleContent = new GUIContent("사례분석 · 원본");
        w.minSize = new Vector2(320, 240);
    }

    private void OnEnable()
    {
        ReloadTextures();
    }

    private void ReloadTextures()
    {
        _textures.Clear();
        if (!AssetDatabase.IsValidFolder(FolderPath))
            return;

        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { FolderPath });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var fileName = Path.GetFileNameWithoutExtension(path);
            if (fileName != "원본" && !fileName.StartsWith("원본"))
                continue;

            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex != null)
                _textures.Add(tex);
        }

        _textures.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("새로고침", EditorStyles.toolbarButton))
        {
            ReloadTextures();
            Repaint();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (_textures.Count == 0)
        {
            EditorGUILayout.HelpBox(
                $"\"{FolderPath}\" 에서 이름이 \"원본\" 으로 시작하는 PNG를 찾지 못했습니다.",
                MessageType.Info);
            return;
        }

        float toolbarH = EditorGUIUtility.singleLineHeight;
        var viewRect = new Rect(0f, toolbarH, position.width, position.height - toolbarH);

        if (_textures.Count == 1)
        {
            // 한 장이면 창 전체에 비율 유지하며 맞춤(여백은 ScaleToFit이 처리)
            GUI.DrawTexture(viewRect, _textures[0], ScaleMode.ScaleToFit, true);
            return;
        }

        float innerWidth = position.width - 24f;

        EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        foreach (var tex in _textures)
            DrawTextureFitWidth(tex, innerWidth);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 스크롤 영역 안에서 가로를 맞추고 세로는 비율대로 표시합니다.
    /// </summary>
    private static void DrawTextureFitWidth(Texture2D tex, float maxWidth)
    {
        if (tex.width <= 0 || tex.height <= 0)
            return;

        float w = Mathf.Min(maxWidth, tex.width);
        float h = w * (tex.height / (float)tex.width);
        Rect r = GUILayoutUtility.GetRect(w, h);
        GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit, true);
        GUILayout.Space(6f);
    }
}
