using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamDistribution.Demo
{
    /// <summary>검증용: 1회 클릭 → 내러티브, 「다음」으로 닫은 뒤 2회 클릭 → 다음 씬.</summary>
    public sealed class VerificationMiniFlow : MonoBehaviour
    {
        [SerializeField] string _firstLine =
            "팀 배포 코어 검증\n" +
            "이 텍스트가 보이면 NarrativeTextBox 연결이 정상입니다.\n" +
            "「다음」을 누른 뒤, 이 큐브를 다시 클릭하면 페이드 씬 전환으로 다음 씬으로 이동합니다.";

        [SerializeField] string _nextSceneName = "Verification_End";

        int _step;

        public void OnDemoClicked()
        {
            if (_step == 0)
            {
                if (NarrativeTextBox.Instance != null)
                    NarrativeTextBox.Instance.ShowLine(_firstLine);
                _step = 1;
                return;
            }

            if (SceneTransition.Instance != null)
                SceneTransition.Instance.LoadScene(_nextSceneName);
            else
                SceneManager.LoadScene(_nextSceneName);
        }
    }
}
