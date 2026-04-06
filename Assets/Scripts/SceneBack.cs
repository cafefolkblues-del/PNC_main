using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBack : MonoBehaviour
{
    public static string previousScene;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC 눌림");
            Debug.Log("이전 씬: " + previousScene);

            if (!string.IsNullOrEmpty(previousScene))
            {
                SceneManager.LoadScene(previousScene);
            }
            else
            {
                Debug.Log("이전 씬 정보가 없습니다.");
            }
        }
    }
}