using System.Collections;
using UnityEngine;

/// <summary>
/// 2D 스프라이트(SpriteRenderer + BoxCollider2D): 마우스 오버/클릭 시 선택적으로 색·스케일 피드백.
/// 기본값은 idle에서 스프라이트 원색(틴트 없음)을 유지합니다.
/// </summary>
public class Click_Reaction : MonoBehaviour
{
    // --- 대사창: 클릭 시 Show(english)로 본문 표시. 나중에 인스펙터 TMP만 쓰려면 DialoguePanel.Show() 무인자로 바꿀 수 있음. ---

    [Header("Dialogue (optional)")]
    [Tooltip("비우면 씬에서 DialoguePanel을 찾고, 없으면 DialogueSystem 오브젝트를 만들어 붙임.")]
    [SerializeField] DialoguePanel dialoguePanel;

    [TextArea(2, 5)]
    [SerializeField] string dialogueMessage = "somthing smells wrong";

    [Header("Visual feedback")]
    [Tooltip("마우스를 올리지 않을 때 스프라이트에 곱할 색 (기본 흰색 = 원본 그대로)")]
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color hoverColor = Color.white;
    [SerializeField] Color clickFlashColor = Color.white;
    [SerializeField] float clickPulseScale = 1.12f;
    [SerializeField] float clickPulseDuration = 0.12f;

    SpriteRenderer _sprite;
    Vector3 _baseScale;
    bool _hovering;
    Coroutine _clickRoutine;
    Color _idleColor;

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _baseScale = transform.localScale;

        // 피드백 색이 모두 동일하면 반응이 보이지 않으므로 기본 어둡기 변화를 자동 적용.
        if (normalColor == hoverColor && normalColor == clickFlashColor)
        {
            hoverColor = new Color(0.86f, 0.86f, 0.86f, 1f);
            clickFlashColor = new Color(0.72f, 0.72f, 0.72f, 1f);
        }

        if (_sprite != null && _sprite.sprite == null)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            _sprite.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        if (_sprite != null)
        {
            _idleColor = normalColor;
            _sprite.color = _idleColor;
        }

        var col = GetComponent<BoxCollider2D>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider2D>();

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
            _sprite.color = _idleColor;
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
            _sprite.color = _hovering ? hoverColor : _idleColor;

        _clickRoutine = null;
    }
}
