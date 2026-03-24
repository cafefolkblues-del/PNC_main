using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamDistribution
{
    /// <summary>Interactable.onInteract 등에서 씬 전환 호출.</summary>
    public sealed class InteractableLoadScene : MonoBehaviour
    {
        [SerializeField] string _sceneName;

        [Tooltip("로드 전 GameStatePersistence.SaveToDisk 호출")]
        [SerializeField] bool _saveBeforeLoad;

        public void Load()
        {
            if (_saveBeforeLoad)
            {
                var p = FindObjectOfType<GameStatePersistence>();
                if (p != null) p.SaveToDisk();
            }

            if (SceneTransition.Instance != null) SceneTransition.Instance.LoadScene(_sceneName);
            else SceneManager.LoadScene(_sceneName);
        }
    }
}
