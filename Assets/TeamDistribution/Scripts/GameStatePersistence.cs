using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TeamDistribution
{
    /// <summary>플래그·인벤토리 JSON 저장/로드 (기획서 오토 세이브 대응 기초).</summary>
    public sealed class GameStatePersistence : MonoBehaviour
    {
        [SerializeField] string _fileName = "dollhouse_save.json";

        [Serializable]
        class FlagEntry
        {
            public string key;
            public int value;
        }

        [Serializable]
        class SaveDto
        {
            public List<FlagEntry> flags = new List<FlagEntry>();
            public List<string> inventory = new List<string>();
        }

        public void SaveToDisk()
        {
            var dto = new SaveDto();
            if (FlagStore.Instance != null)
            {
                foreach (var kv in FlagStore.Instance.Snapshot())
                    dto.flags.Add(new FlagEntry { key = kv.Key, value = kv.Value });
            }

            if (InventorySystem.Instance != null)
                dto.inventory.AddRange(InventorySystem.Instance.Snapshot());

            string path = Path.Combine(Application.persistentDataPath, _fileName);
            File.WriteAllText(path, JsonUtility.ToJson(dto, true));
        }

        public bool LoadFromDisk()
        {
            string path = Path.Combine(Application.persistentDataPath, _fileName);
            if (!File.Exists(path)) return false;
            var dto = JsonUtility.FromJson<SaveDto>(File.ReadAllText(path));
            if (dto == null) return false;

            if (FlagStore.Instance != null)
            {
                FlagStore.Instance.ClearAll();
                foreach (var e in dto.flags)
                {
                    if (e != null && !string.IsNullOrEmpty(e.key))
                        FlagStore.Instance.Set(e.key, e.value);
                }
            }

            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.Clear();
                InventorySystem.Instance.LoadSnapshot(dto.inventory);
            }

            return true;
        }

        public void DeleteSave()
        {
            string path = Path.Combine(Application.persistentDataPath, _fileName);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
