using UnityEngine;

namespace TeamDistribution
{
    /// <summary>클릭 시 NarrativeTextBox에 한 줄 표시.</summary>
    public sealed class InteractableShowLine : MonoBehaviour
    {
        [SerializeField] [TextArea(2, 6)] string _line;

        public void Show()
        {
            if (NarrativeTextBox.Instance == null) return;
            NarrativeTextBox.Instance.ShowLine(_line);
        }
    }
}
