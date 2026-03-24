using UnityEngine;
using UnityEngine.EventSystems;

namespace TeamDistribution
{
    /// <summary>첫 씬에 하나 두면 공용 싱글톤·EventSystem·씬 전환을 준비.</summary>
    public sealed class TeamDistributionRoot : MonoBehaviour
    {
        [SerializeField] bool _ensureFlagStore = true;
        [SerializeField] bool _ensureInventory = true;
        [SerializeField] bool _ensureEventSystem = true;
        [SerializeField] bool _ensureSceneTransition = true;

        void Awake()
        {
            if (_ensureFlagStore) EnsureSingleton<FlagStore>(true);
            if (_ensureInventory) EnsureSingleton<InventorySystem>(true);
            if (_ensureEventSystem) EnsureEventSystem();
            if (_ensureSceneTransition) EnsureSceneTransition();
        }

        static void EnsureSingleton<T>(bool dontDestroy) where T : Component
        {
            if (FindObjectOfType<T>() != null) return;
            var go = new GameObject(typeof(T).Name);
            go.AddComponent<T>();
            if (dontDestroy) DontDestroyOnLoad(go);
        }

        static void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(es);
        }

        static void EnsureSceneTransition()
        {
            if (FindObjectOfType<SceneTransition>() != null) return;
            var go = new GameObject("SceneTransition");
            go.AddComponent<SceneTransition>();
        }
    }
}
