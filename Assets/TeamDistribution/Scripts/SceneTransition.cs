using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamDistribution
{
    /// <summary>페이드 아웃 → 씬 로드 → 페이드 인. DontDestroyOnLoad 싱글톤.</summary>
    public sealed class SceneTransition : MonoBehaviour
    {
        public static SceneTransition Instance { get; private set; }

        [SerializeField] float _fadeOutDuration = 0.35f;
        [SerializeField] float _fadeInDuration = 0.35f;

        ScreenFadeOverlay _fade;
        bool _busy;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _fade = GetComponentInChildren<ScreenFadeOverlay>(true);
            if (_fade == null) _fade = ScreenFadeOverlay.CreateUnder(gameObject);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>기본 페이드 시간으로 Single 모드 로드.</summary>
        public void LoadScene(string sceneName)
        {
            LoadScene(sceneName, _fadeOutDuration, _fadeInDuration, LoadSceneMode.Single);
        }

        public void LoadScene(string sceneName, float fadeOut, float fadeIn, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (_busy) return;
            if (string.IsNullOrEmpty(sceneName)) return;
            StartCoroutine(LoadRoutine(sceneName, fadeOut, fadeIn, mode));
        }

        IEnumerator LoadRoutine(string sceneName, float fadeOut, float fadeIn, LoadSceneMode mode)
        {
            _busy = true;
            if (_fade != null) yield return _fade.FadeOut(fadeOut);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
            if (op == null)
            {
                Debug.LogError($"[SceneTransition] 씬을 찾을 수 없습니다: '{sceneName}' (Build Settings에 추가했는지 확인)");
                if (_fade != null) yield return _fade.FadeIn(fadeIn);
                _busy = false;
                yield break;
            }

            while (!op.isDone) yield return null;

            yield return null;

            if (_fade != null) yield return _fade.FadeIn(fadeIn);
            _busy = false;
        }
    }
}
