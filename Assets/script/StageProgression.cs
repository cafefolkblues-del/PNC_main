using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 조건이 만족되면 다음 씬으로 전환. 인스펙터에 다음 씬 이름을 넣고,
/// 다른 스크립트·버튼·이벤트에서 <see cref="LoadNextStage"/>를 호출하면 됩니다.
/// </summary>
public class StageProgression : MonoBehaviour
{
    [SerializeField] string nextSceneName = "Stage2Scene";

    [Tooltip("체크 시 빌드 설정에서 \"현재 인덱스 + 1\" 씬을 로드 (nextSceneName 대신 사용)")]
    [SerializeField] bool useNextBuildIndex;

    /// <summary>조건을 만족했을 때 호출 (UI 버튼, 대사 종료, 점수 달성 등에서 연결)</summary>
    public void LoadNextStage()
    {
        if (useNextBuildIndex)
        {
            int next = SceneManager.GetActiveScene().buildIndex + 1;
            if (next >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogWarning("StageProgression: No next scene in Build Settings.");
                return;
            }

            SceneManager.LoadScene(next);
            return;
        }

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogWarning("StageProgression: nextSceneName is empty.");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
