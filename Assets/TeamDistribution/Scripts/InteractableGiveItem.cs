using UnityEngine;

namespace TeamDistribution
{
    /// <summary>클릭 시 인벤토리에 아이템 추가 (유품·쪽지 등).</summary>
    public sealed class InteractableGiveItem : MonoBehaviour
    {
        [SerializeField] string _itemId;
        [SerializeField] bool _consumeObject = true;

        public void Give()
        {
            if (InventorySystem.Instance == null || string.IsNullOrEmpty(_itemId)) return;
            InventorySystem.Instance.Add(_itemId);
            if (_consumeObject) gameObject.SetActive(false);
        }
    }
}
