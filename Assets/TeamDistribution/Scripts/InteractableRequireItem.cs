using UnityEngine;
using UnityEngine.Events;

namespace TeamDistribution
{
    /// <summary>특정 아이템 보유 시에만 상호작용 허용.</summary>
    public sealed class InteractableRequireItem : MonoBehaviour
    {
        [SerializeField] string _requiredItemId;
        [SerializeField] UnityEvent _onSuccess;
        [SerializeField] UnityEvent _onFail;

        public void TryUse()
        {
            if (InventorySystem.Instance == null)
            {
                _onFail?.Invoke();
                return;
            }

            if (InventorySystem.Instance.Has(_requiredItemId))
                _onSuccess?.Invoke();
            else
                _onFail?.Invoke();
        }
    }
}
