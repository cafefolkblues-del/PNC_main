using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamDistribution
{
    /// <summary>스테이지 간 아이템 보유. 아이템 ID는 문자열 규약으로 통일.</summary>
    public sealed class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Instance { get; private set; }

        readonly HashSet<string> _items = new HashSet<string>(StringComparer.Ordinal);

        public event Action<string> OnItemAdded;
        public event Action<string> OnItemRemoved;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool Has(string itemId) => _items.Contains(itemId);

        public void Add(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return;
            if (_items.Add(itemId))
                OnItemAdded?.Invoke(itemId);
        }

        public bool Remove(string itemId)
        {
            if (_items.Remove(itemId))
            {
                OnItemRemoved?.Invoke(itemId);
                return true;
            }

            return false;
        }

        public void Clear() => _items.Clear();

        public IReadOnlyCollection<string> Snapshot() => new HashSet<string>(_items);

        public void LoadSnapshot(IEnumerable<string> ids)
        {
            _items.Clear();
            foreach (var id in ids)
            {
                if (!string.IsNullOrEmpty(id))
                    _items.Add(id);
            }
        }
    }
}
