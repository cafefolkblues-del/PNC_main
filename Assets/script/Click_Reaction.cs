using System.Collections;
using UnityEngine;

/// <summary>
/// 2D 사각형(SpriteRenderer + BoxCollider2D)에 마우스를 올리면 색이 바뀌고,
/// 클릭하면 짧게 커졌다가 돌아오며 색이 깜빡입니다.
/// </summary>
public class Click_Reaction : MonoBehaviour
{
    // --- 대사창: 클릭 시 Show(english)로 본문 표시. 나중에 인스펙터 TMP만 쓰려면 DialoguePanel.Show() 무인자로 바꿀 수 있음. ---

    [Header("Dialogue (optional)")]
    [Tooltip("비우면 씬에서 DialoguePanel을 찾고, 없으면 DialogueSystem 오브젝트를 만들어 붙임.")]
    [SerializeField] DialoguePanel dialoguePanel;

    [TextArea(2, 5)]
    [SerializeField] string dialogueMessage =
        "You opened this dialogue by clicking the rectangle.\nPress Close when you are done.";

    [Header("Visual feedback")]
    [SerializeField] Color normalColor = new Color(0.4f, 0.55f, 0.85f, 1f);
    [SerializeField] Color hoverColor = new Color(0.55f, 0.75f, 1f, 1f);
    [SerializeField] Color clickFlashColor = new Color(1f, 0.85f, 0.5f, 1f);
    [SerializeField] float clickPulseScale = 1.12f;
    [SerializeField] float clickPulseDuration = 0.12f;

    SpriteRenderer _sprite;
    Vector3 _baseScale;
    bool _hovering;
    Coroutine _clickRoutine;

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _baseScale = transform.localScale;

        if (_sprite != null && _sprite.sprite == null)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            _sprite.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        if (_sprite != null)
            _sprite.color = normalColor;

        var col = GetComponent<BoxCollider2D>();
        if (col != null && _sprite != null && _sprite.sprite != null)
            col.size = _sprite.sprite.bounds.size;

        // 대사창: 인스펙터에 안 넣었으면 씬에서 찾고, 그래도 없으면 DialoguePanel이 자체 UI를 만들도록 DialogueSystem 생성
        if (dialoguePanel == null)
            dialoguePanel = FindFirstObjectByType<DialoguePanel>();

        if (dialoguePanel == null)
        {
            var sys = new GameObject("DialogueSystem");
            dialoguePanel = sys.AddComponent<DialoguePanel>();
        }
    }

    void OnMouseEnter()
    {
        _hovering = true;
        if (_sprite != null)
            _sprite.color = hoverColor;
    }

    void OnMouseExit()
    {
        _hovering = false;
        if (_sprite != null)
            _sprite.color = normalColor;
    }

    void OnMouseDown()
    {
        if (dialoguePanel != null)
            dialoguePanel.Show(dialogueMessage);

        if (_clickRoutine != null)
            StopCoroutine(_clickRoutine);
        _clickRoutine = StartCoroutine(ClickFeedback());
    }

    IEnumerator ClickFeedback()
    {
        if (_sprite != null)
            _sprite.color = clickFlashColor;

        transform.localScale = _baseScale * clickPulseScale;
        yield return new WaitForSeconds(clickPulseDuration);

        transform.localScale = _baseScale;
        if (_sprite != null)
            _sprite.color = _hovering ? hoverColor : normalColor;

        _clickRoutine = null;
    }
}
