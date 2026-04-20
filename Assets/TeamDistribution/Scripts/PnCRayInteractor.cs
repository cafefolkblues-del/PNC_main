using UnityEngine;
using UnityEngine.EventSystems;

namespace TeamDistribution
{
    /// <summary>메인 카메라에서 마우스 레이로 Interactable / 드래그 처리. UI 위에서는 클릭 무시(GraphicRaycaster 사용 시 EventSystem 필요).</summary>
    [DisallowMultipleComponent]
    public sealed class PnCRayInteractor : MonoBehaviour
    {
        [SerializeField] Camera _camera;
        [SerializeField] LayerMask _hitMask = ~0;
        [SerializeField] float _maxDistance = 100f;
        [SerializeField] Texture2D _hoverCursor;
        [SerializeField] Vector2 _hotspot = Vector2.zero;

        WorldDraggable _dragging;
        Interactable _hovered;

        void Reset()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null) _camera = Camera.main;
        }

        void Awake()
        {
            if (_camera == null) _camera = GetComponent<Camera>();
            if (_camera == null) _camera = Camera.main;
        }

        void Update()
        {
            if (_camera == null) return;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (_dragging != null)
            {
                _dragging.DragUpdate(ray);
                if (Input.GetMouseButtonUp(0))
                {
                    _dragging.EndDrag(ray, _hitMask, _maxDistance);
                    _dragging = null;
                }

                ClearHover();
                return;
            }

            UpdateHover(ray);

            if (Input.GetMouseButtonDown(0) && !IsPointerOverUi())
            {
                if (TryGetDraggable(ray, out WorldDraggable drag))
                {
                    _dragging = drag;
                    drag.BeginDrag(ray, _camera, _hitMask, _maxDistance);
                    return;
                }

                if (TryGetInteractable(ray, out Interactable hit))
                    hit.Interact();
            }
        }

        static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        void UpdateHover(Ray ray)
        {
            Interactable next = null;
            if (Physics.Raycast(ray, out RaycastHit rh, _maxDistance, _hitMask, QueryTriggerInteraction.Collide))
            {
                next = rh.collider.GetComponentInParent<Interactable>();
                if (next != null && !next.InteractionsEnabled) next = null;
            }

            if (next == _hovered) return;

            _hovered = next;
            if (_hoverCursor != null)
            {
                if (_hovered != null) Cursor.SetCursor(_hoverCursor, _hotspot, CursorMode.Auto);
                else Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        void ClearHover()
        {
            if (_hovered != null && _hoverCursor != null)
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            _hovered = null;
        }

        bool TryGetInteractable(Ray ray, out Interactable best)
        {
            best = null;
            var hits = Physics.RaycastAll(ray, _maxDistance, _hitMask, QueryTriggerInteraction.Collide);
            int bestOrder = int.MinValue;
            foreach (var h in hits)
            {
                var i = h.collider.GetComponentInParent<Interactable>();
                if (i == null || !i.InteractionsEnabled) continue;
                if (i.SortOrder >= bestOrder)
                {
                    bestOrder = i.SortOrder;
                    best = i;
                }
            }

            return best != null;
        }

        bool TryGetDraggable(Ray ray, out WorldDraggable best)
        {
            best = null;
            var hits = Physics.RaycastAll(ray, _maxDistance, _hitMask, QueryTriggerInteraction.Collide);
            int bestOrder = int.MinValue;
            foreach (var h in hits)
            {
                var d = h.collider.GetComponentInParent<WorldDraggable>();
                if (d == null || !d.DragEnabled) continue;
                if (d.SortOrder >= bestOrder)
                {
                    bestOrder = d.SortOrder;
                    best = d;
                }
            }

            return best != null;
        }
    }
}
