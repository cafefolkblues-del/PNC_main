using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public string sceneName;

    private void OnMouseDown()
    {
        Debug.Log("Å¬¸¯µÊ");
        SceneManager.LoadScene(sceneName);
    }
}