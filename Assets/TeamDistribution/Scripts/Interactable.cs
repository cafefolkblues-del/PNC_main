using UnityEngine;
using UnityEngine.Events;

namespace TeamDistribution
{
    /// <summary>레이캐스트로 클릭되는 오브젝트. Collider 필수.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class Interactable : MonoBehaviour
    {
        [SerializeField] bool _enabled = true;

        [Tooltip("다른 Interactable보다 먼저 선택되게 할 때 값을 크게")]
        [SerializeField] int _sortOrder;

        public UnityEvent onInteract;

        public bool InteractionsEnabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public int SortOrder => _sortOrder;

        public void Interact()
        {
            if (!_enabled) return;
            onInteract?.Invoke();
        }
    }
}
