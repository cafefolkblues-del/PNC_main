using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamDistribution
{
    /// <summary>전역 플래그 (퍼즐 분기, 배드엔딩, 진엔딩 조건 등). 키는 팀에서 문자열 규약만 맞추면 됩니다.</summary>
    public sealed class FlagStore : MonoBehaviour
    {
        public static FlagStore Instance { get; private set; }

        readonly Dictionary<string, int> _flags = new Dictionary<string, int>(StringComparer.Ordinal);

        public event Action<string, int, int> OnFlagChanged;

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

        public int Get(string key, int defaultValue = 0)
        {
            return _flags.TryGetValue(key, out int v) ? v : defaultValue;
        }

        public bool GetBool(string key) => Get(key) != 0;

        public void Set(string key, int value)
        {
            int old = Get(key);
            if (old == value) return;
            _flags[key] = value;
            OnFlagChanged?.Invoke(key, old, value);
        }

        public void SetBool(string key, bool value) => Set(key, value ? 1 : 0);

        public void Add(string key, int delta) => Set(key, Get(key) + delta);

        public IReadOnlyDictionary<string, int> Snapshot() => new Dictionary<string, int>(_flags);

        public void LoadSnapshot(IReadOnlyDictionary<string, int> data)
        {
            _flags.Clear();
            foreach (var kv in data)
                _flags[kv.Key] = kv.Value;
        }

        public void ClearAll()
        {
            _flags.Clear();
        }
    }
}
