using UnityEngine;
using UnityEngine.Events;

namespace TeamDistribution
{
    /// <summary>방별 인지 필터 파괴 여부를 플래그로 기록. 연출(포스트 프로세스 등)은 이 값을 구독해 처리.</summary>
    public sealed class CognitiveFilterRoomState : MonoBehaviour
    {
        [SerializeField] string _roomKey = "kitchen";

        [SerializeField] UnityEvent _onRealityRevealed;

        const string Prefix = "cognitive.real.";

        public string RoomKey => _roomKey;

        public bool IsRealityRevealed =>
            FlagStore.Instance != null && FlagStore.Instance.GetBool(Prefix + _roomKey);

        public void RevealReality()
        {
            if (FlagStore.Instance == null) return;
            if (IsRealityRevealed) return;
            FlagStore.Instance.SetBool(Prefix + _roomKey, true);
            _onRealityRevealed?.Invoke();
        }
    }
}
