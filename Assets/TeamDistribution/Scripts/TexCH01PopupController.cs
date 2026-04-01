using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamDistribution
{
    /// <summary>
    /// Opens a TMP popup when Tex_CH_01 is clicked.
    /// Works even when Canvas/Text(TMP) is missing by creating them at runtime.
    /// </summary>
    public sealed class TexCH01PopupController : MonoBehaviour
    {
        private const string TargetName = "Tex_CH_01";
        private const string ButtonName = "Button_01";
        private const string CanvasName = "Canvas";
        private const string PopupName = "Text (TMP)";
        private const float PopupYOffset = 60f;

        [SerializeField] private string _popupText = "Test";
        [SerializeField] private int _fontSize = 42;
        [SerializeField] private string _nextSceneName = "NextStage";
        [SerializeField] private float _nextSceneDelay = 1f;

        private Camera _mainCamera;
        private GameObject _target;
        private RectTransform _popupRect;
        private TMP_Text _popupLabel;
        private GameObject _buttonObject;
        private Button _button;
        private RectTransform _buttonRect;
        private bool _isLoadingNextScene;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindFirstObjectByType<TexCH01PopupController>() != null)
            {
                return;
            }

            var go = new GameObject(nameof(TexCH01PopupController));
            DontDestroyOnLoad(go);
            go.AddComponent<TexCH01PopupController>();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            ResolveSceneObjects();
        }

        private void Update()
        {
            if (_target == null || _popupRect == null)
            {
                ResolveSceneObjects();
                return;
            }

            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            if (IsButtonClicked(Input.mousePosition))
            {
                HidePopupAndLoadNextScene();
                return;
            }

            if (!IsTargetClicked(Input.mousePosition))
            {
                return;
            }

            PositionPopupAboveTarget();
            _popupRect.gameObject.SetActive(!_popupRect.gameObject.activeSelf);
            if (_buttonObject != null)
            {
                _buttonObject.SetActive(true);
            }
        }

        private void ResolveSceneObjects()
        {
            _target = GameObject.Find(TargetName);
            if (_target == null)
            {
                return;
            }

            EnsureTargetCollider(_target);
            EnsurePopupUi();
            EnsureButtonUi();
        }

        private void EnsurePopupUi()
        {
            var canvas = GameObject.Find(CanvasName);
            if (canvas == null)
            {
                canvas = new GameObject(CanvasName, typeof(Canvas));
                var c = canvas.GetComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            var popup = GameObject.Find(PopupName);
            if (popup == null)
            {
                popup = new GameObject(PopupName, typeof(RectTransform), typeof(TextMeshProUGUI));
                popup.transform.SetParent(canvas.transform, false);
            }

            _popupRect = popup.GetComponent<RectTransform>();
            _popupLabel = popup.GetComponent<TextMeshProUGUI>();

            _popupRect.anchorMin = new Vector2(0.5f, 0.5f);
            _popupRect.anchorMax = new Vector2(0.5f, 0.5f);
            _popupRect.pivot = new Vector2(0.5f, 0.5f);
            _popupRect.sizeDelta = new Vector2(700f, 180f);

            _popupLabel.text = _popupText;
            _popupLabel.fontSize = _fontSize;
            _popupLabel.alignment = TextAlignmentOptions.Center;
            _popupLabel.enableWordWrapping = true;

            _popupRect.gameObject.SetActive(false);
        }

        private void EnsureButtonUi()
        {
            if (_buttonObject == null)
            {
                _buttonObject = GameObject.Find(ButtonName);
            }

            if (_buttonObject == null)
            {
                return;
            }

            EnsureButtonCollider(_buttonObject);
            _buttonObject.SetActive(false);

            if (_button == null)
            {
                _button = _buttonObject.GetComponent<Button>();
                if (_button == null)
                {
                    _button = _buttonObject.GetComponentInChildren<Button>(true);
                }
            }

            if (_button == null)
            {
                return;
            }

            if (_buttonRect == null)
            {
                _buttonRect = _button.GetComponent<RectTransform>();
            }

            _button.onClick.RemoveListener(HidePopupAndLoadNextScene);
            _button.onClick.AddListener(HidePopupAndLoadNextScene);
        }

        private void HidePopupAndLoadNextScene()
        {
            if (_isLoadingNextScene)
            {
                return;
            }

            _isLoadingNextScene = true;

            if (_popupRect != null)
            {
                _popupRect.gameObject.SetActive(false);
            }

            if (_buttonObject != null)
            {
                _buttonObject.SetActive(false);
            }

            StartCoroutine(LoadNextSceneAfterDelay());
        }

        private IEnumerator LoadNextSceneAfterDelay()
        {
            yield return new WaitForSeconds(_nextSceneDelay);
            SceneManager.LoadScene(_nextSceneName);
        }

        private bool IsButtonClicked(Vector2 mousePosition)
        {
            if (_buttonObject == null || !_buttonObject.activeInHierarchy)
            {
                return false;
            }

            if (IsButtonSpriteClicked(mousePosition))
            {
                return true;
            }

            if (IsButtonClickedOnUiRaycast(mousePosition))
            {
                return true;
            }

            if (IsButtonClickedOnWorldRaycast(mousePosition))
            {
                return true;
            }

            if (_buttonRect == null && _button != null)
            {
                _buttonRect = _button.GetComponent<RectTransform>();
            }

            if (_buttonRect == null)
            {
                return false;
            }

            return RectTransformUtility.RectangleContainsScreenPoint(_buttonRect, mousePosition, null);
        }

        private bool IsButtonSpriteClicked(Vector2 mousePosition)
        {
            var sprite = _buttonObject.GetComponent<SpriteRenderer>();
            if (sprite == null)
            {
                return false;
            }

            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    return false;
                }
            }

            var distance = Vector3.Distance(_mainCamera.transform.position, sprite.bounds.center);
            var worldPoint = _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distance));
            worldPoint.z = sprite.bounds.center.z;
            return sprite.bounds.Contains(worldPoint);
        }

        private static bool IsButtonClickedOnUiRaycast(Vector2 mousePosition)
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            var eventData = new PointerEventData(EventSystem.current) { position = mousePosition };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            for (int i = 0; i < results.Count; i++)
            {
                var go = results[i].gameObject;
                if (go.name == ButtonName || go.transform.IsChildOf(GameObject.Find(ButtonName)?.transform))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsButtonClickedOnWorldRaycast(Vector2 mousePosition)
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    return false;
                }
            }

            var ray = _mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit3D))
            {
                var t = _buttonObject.transform;
                if (hit3D.collider != null && (hit3D.collider.transform == t || hit3D.collider.transform.IsChildOf(t)))
                {
                    return true;
                }
            }

            var hit2D = Physics2D.GetRayIntersection(ray, float.MaxValue);
            if (hit2D.collider != null)
            {
                var t = _buttonObject.transform;
                if (hit2D.collider.transform == t || hit2D.collider.transform.IsChildOf(t))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsTargetClicked(Vector2 mousePosition)
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    return false;
                }
            }

            var ray = _mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                return hit.collider != null && (hit.collider.gameObject == _target || hit.collider.transform.IsChildOf(_target.transform));
            }

            return false;
        }

        private void PositionPopupAboveTarget()
        {
            var screenPos = _mainCamera.WorldToScreenPoint(_target.transform.position);
            screenPos.y += PopupYOffset;

            var parentRect = _popupRect.parent as RectTransform;
            if (parentRect == null)
            {
                return;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, null, out var localPos))
            {
                _popupRect.anchoredPosition = localPos;
            }
        }

        private static void EnsureTargetCollider(GameObject target)
        {
            if (target.GetComponent<Collider>() != null || target.GetComponent<Collider2D>() != null)
            {
                return;
            }

            target.AddComponent<BoxCollider>();
        }

        private static void EnsureButtonCollider(GameObject buttonObject)
        {
            if (buttonObject.GetComponent<Collider>() != null || buttonObject.GetComponent<Collider2D>() != null)
            {
                return;
            }

            if (buttonObject.GetComponent<RectTransform>() != null)
            {
                return;
            }

            buttonObject.AddComponent<BoxCollider>();
        }
    }
}
