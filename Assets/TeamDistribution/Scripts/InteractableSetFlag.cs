using UnityEngine;

namespace TeamDistribution
{
    /// <summary>Interactable.onInteract에서 플래그 설정.</summary>
    public sealed class InteractableSetFlag : MonoBehaviour
    {
        [SerializeField] string _key;
        [SerializeField] int _value = 1;

        public void Apply()
        {
            if (FlagStore.Instance == null || string.IsNullOrEmpty(_key)) return;
            FlagStore.Instance.Set(_key, _value);
        }
    }
}
