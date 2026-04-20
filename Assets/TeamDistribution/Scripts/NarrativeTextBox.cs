using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamDistribution
{
    /// <summary>독백·일기·대사 1줄씩 출력. Canvas 아래에 두고 TMP_Text·Button 연결.</summary>
    public sealed class NarrativeTextBox : MonoBehaviour
    {
        public static NarrativeTextBox Instance { get; private set; }

        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] TMP_Text _body;
        [SerializeField] Button _advanceButton;

        readonly Queue<string> _queue = new Queue<string>();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (_advanceButton != null)
                _advanceButton.onClick.AddListener(Advance);
            SetVisible(false);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (_advanceButton != null)
                _advanceButton.onClick.RemoveListener(Advance);
        }

        public void ShowLine(string line)
        {
            _queue.Clear();
            EnqueueAndShow(line);
        }

        public void EnqueueLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            _queue.Enqueue(line);
            if (_body != null && string.IsNullOrEmpty(_body.text))
                FlushNext();
        }

        public void ShowSequence(IEnumerable<string> lines)
        {
            _queue.Clear();
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                    _queue.Enqueue(line);
            }

            FlushNext();
        }

        void EnqueueAndShow(string line)
        {
            if (!string.IsNullOrEmpty(line))
                _queue.Enqueue(line);
            FlushNext();
        }

        public void Advance()
        {
            FlushNext();
        }

        void FlushNext()
        {
            if (_queue.Count == 0)
            {
                if (_body != null) _body.text = string.Empty;
                SetVisible(false);
                return;
            }

            SetVisible(true);
            if (_body != null) _body.text = _queue.Dequeue();
        }

        void SetVisible(bool on)
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = on ? 1f : 0f;
            _canvasGroup.blocksRaycasts = on;
            _canvasGroup.interactable = on;
        }
    }
}
