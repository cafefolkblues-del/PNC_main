using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SmallCubeCreator
{
    [MenuItem("GameObject/Small Cube")]
    private static void CreateSmallCube()
    {
        var activeScene = SceneManager.GetActiveScene();

        // 기존 오브젝트가 있으면 새로 만들지 않도록 간단히 방지
        var existing = GameObject.Find("SmallCube");
        if (existing != null)
        {
            Selection.activeGameObject = existing;
            return;
        }

        // 기본 큐브 프리미티브 생성
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "SmallCube";
        cube.transform.position = Vector3.zero;
        cube.transform.localScale = Vector3.one * 0.2f; // "작은" 정도로 아주 작게

        Undo.RegisterCreatedObjectUndo(cube, "Create SmallCube");
        Selection.activeGameObject = cube;

        // 씬 저장 여부 표시
        EditorSceneManager.MarkSceneDirty(activeScene);
    }
}
