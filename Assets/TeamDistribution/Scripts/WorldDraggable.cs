using UnityEngine;

namespace TeamDistribution
{
    /// <summary>월드 콜라이더를 드래그해 드롭 슬롯에 맞추는 용도. ItemId로 슬롯 매칭.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class WorldDraggable : MonoBehaviour
    {
        [SerializeField] string _itemId;

        [Tooltip("Interactable보다 우선할 때 정렬값을 높임")]
        [SerializeField] int _sortOrder;

        [SerializeField] bool _dragEnabled = true;

        [SerializeField] bool _returnToStartIfRejected = true;

        Vector3 _startPosition;
        Quaternion _startRotation;
        Vector3 _grabOffset;
        Plane _dragPlane;

        public string ItemId => _itemId;
        public int SortOrder => _sortOrder;
        public bool DragEnabled => _dragEnabled;

        void Awake()
        {
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        public void BeginDrag(Ray ray, Camera cam, LayerMask hitMask, float maxDistance)
        {
            _dragPlane = new Plane(-cam.transform.forward, transform.position);
            if (_dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hit = ray.GetPoint(enter);
                _grabOffset = transform.position - hit;
            }
            else
                _grabOffset = Vector3.zero;
        }

        public void DragUpdate(Ray ray)
        {
            if (_dragPlane.Raycast(ray, out float enter))
                transform.position = ray.GetPoint(enter) + _grabOffset;
        }

        public void EndDrag(Ray ray, LayerMask hitMask, float maxDistance)
        {
            var hits = Physics.RaycastAll(ray, maxDistance, hitMask, QueryTriggerInteraction.Collide);
            foreach (var h in hits)
            {
                var slot = h.collider.GetComponentInParent<WorldDropSlot>();
                if (slot != null && slot.TryReceive(_itemId, this))
                    return;
            }

            if (_returnToStartIfRejected)
            {
                transform.position = _startPosition;
                transform.rotation = _startRotation;
            }
        }
    }
}
