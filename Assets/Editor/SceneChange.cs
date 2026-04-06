using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public string sceneName;

    private void OnMouseDown()
    {
        // 현재 씬 이름 저장
        SceneBack.previousScene = SceneManager.GetActiveScene().name;

        // 클릭 확인용
        Debug.Log("클릭됨");
        Debug.Log("현재 씬: " + SceneManager.GetActiveScene().name);
        Debug.Log("이동할 씬: " + sceneName);

        // 다음 씬으로 이동
        SceneManager.LoadScene(sceneName);
    }
}