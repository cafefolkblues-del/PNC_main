using UnityEngine;
using UnityEngine.Events;

namespace TeamDistribution
{
    /// <summary>드롭 존. 허용 ID 목록 또는 빈 목록(아무 ID) 처리.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class WorldDropSlot : MonoBehaviour
    {
        [SerializeField] string[] _acceptedIds;

        [Tooltip("비어 있으면 모든 ItemId 허용")]
        [SerializeField] bool _acceptAnyIfListEmpty = true;

        [SerializeField] bool _deactivatePieceOnAccept = true;

        public UnityEvent<string> onAccepted;

        public bool TryReceive(string itemId, WorldDraggable piece)
        {
            if (!Accepts(itemId)) return false;
            onAccepted?.Invoke(itemId);
            if (_deactivatePieceOnAccept && piece != null) piece.gameObject.SetActive(false);
            return true;
        }

        bool Accepts(string itemId)
        {
            if (_acceptedIds == null || _acceptedIds.Length == 0)
                return _acceptAnyIfListEmpty;

            foreach (var id in _acceptedIds)
            {
                if (string.IsNullOrEmpty(id)) continue;
                if (id == itemId) return true;
            }

            return false;
        }
    }
}
