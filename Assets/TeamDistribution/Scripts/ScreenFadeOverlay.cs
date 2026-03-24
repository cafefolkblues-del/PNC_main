using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TeamDistribution
{
    /// <summary>전환용 검은 오버레이. SceneTransition이 자동 생성하거나 수동으로 붙여도 됨.</summary>
    public sealed class ScreenFadeOverlay : MonoBehaviour
    {
        CanvasGroup _canvasGroup;

        public static ScreenFadeOverlay CreateUnder(GameObject holder)
        {
            var root = new GameObject("ScreenFade");
            root.transform.SetParent(holder.transform, false);

            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32000;
            canvas.overrideSorting = true;

            root.AddComponent<GraphicRaycaster>();

            var cg = root.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            var imageGo = new GameObject("FadeImage");
            imageGo.transform.SetParent(root.transform, false);
            var rt = imageGo.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var img = imageGo.AddComponent<Image>();
            img.color = Color.black;
            img.raycastTarget = true;

            var overlay = root.AddComponent<ScreenFadeOverlay>();
            overlay._canvasGroup = cg;
            return overlay;
        }

        void Awake()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOut(float duration)
        {
            yield return Fade(0f, 1f, duration);
        }

        public IEnumerator FadeIn(float duration)
        {
            yield return Fade(1f, 0f, duration);
        }

        IEnumerator Fade(float from, float to, float duration)
        {
            if (_canvasGroup == null) yield break;

            bool blocking = to > 0.01f;
            _canvasGroup.blocksRaycasts = blocking;
            _canvasGroup.interactable = blocking;

            if (duration <= 0f)
            {
                _canvasGroup.alpha = to;
                _canvasGroup.blocksRaycasts = to > 0.01f;
                _canvasGroup.interactable = to > 0.01f;
                yield break;
            }

            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
                yield return null;
            }

            _canvasGroup.alpha = to;
            _canvasGroup.blocksRaycasts = to > 0.01f;
            _canvasGroup.interactable = to > 0.01f;
        }
    }
}
