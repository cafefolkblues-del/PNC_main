using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UI Button 클릭 시 지정한 씬으로 전환합니다.
/// 같은 오브젝트에 Button이 있으면 자동으로 연결하고, 다른 오브젝트의 Button을 끌어다 넣을 수도 있습니다.
/// </summary>
[DisallowMultipleComponent]
public sealed class LoadSceneOnButtonClick : MonoBehaviour
{
    [Tooltip("비우면 같은 GameObject의 Button을 사용합니다.")]
    [SerializeField] private Button _button;

    [Tooltip("File → Build Settings 에 등록된 씬 이름(확장자 없음).")]
    [SerializeField] private string _sceneName = "NextScene";

    private Button _bound;

    private void Awake()
    {
        _bound = _button != null ? _button : GetComponent<Button>();
        if (_bound == null)
        {
            Debug.LogWarning($"{nameof(LoadSceneOnButtonClick)}: Button이 없습니다. {gameObject.name}", this);
            return;
        }

        _bound.onClick.AddListener(LoadTargetScene);
    }

    private void OnDestroy()
    {
        if (_bound != null)
            _bound.onClick.RemoveListener(LoadTargetScene);
    }

    private void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(_sceneName))
        {
            Debug.LogWarning($"{nameof(LoadSceneOnButtonClick)}: 씬 이름이 비어 있습니다.", this);
            return;
        }

        SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
    }
}
