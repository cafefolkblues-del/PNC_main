using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 대사창 UI: 딤 + 흰 패널 + TMP 본문.
/// - 대사 내용은 보통 <see cref="TextMeshProUGUI"/> 컴포넌트의 Text 필드(인스펙터)에 적고,
///   <see cref="Show()"/>는 그대로 창만 연다 (본문을 코드에서 덮어쓰지 않음).
/// - 코드에서 문장을 넣고 싶을 때만 <see cref="Show(string)"/> 사용.
/// - 창이 열려 있는 동안 화면 아무 곳이나 클릭하면 닫힘.
/// - 한글: LiberationSans는 한글 미지원 → <see cref="koreanFontAsset"/>에 한글 포함 TMP Font Asset 지정
///   또는 Resources에 <c>Fonts/KoreanTMP</c> 로 넣기.
/// </summary>
public class DialoguePanel : MonoBehaviour
{
    [Header("Optional overrides")]
    [Tooltip("한글 등 비라틴 문자용. 비우면 Resources/Fonts/KoreanTMP → TMP Settings 순으로 시도.")]
    [SerializeField] TMP_FontAsset koreanFontAsset;

    [Header("UI references (비우면 BuildRuntimeUI로 생성)")]
    [SerializeField] GameObject panelRoot;
    [SerializeField] TextMeshProUGUI bodyText;
    int _openedFrame = -1;

    void Awake()
    {
        if (panelRoot == null)
            BuildRuntimeUI();

        if (bodyText == null && panelRoot != null)
            bodyText = panelRoot.GetComponentInChildren<TextMeshProUGUI>(true);

        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (bodyText != null)
            ApplyTmpFont(bodyText);

    }

    void Update()
    {
        if (!IsOpen)
            return;

        // 대사창을 연 바로 그 클릭 입력은 무시하고, 다음 클릭부터 닫기 처리.
        if (Time.frameCount == _openedFrame)
            return;

        if (Input.GetMouseButtonDown(0))
            Hide();
    }

    /// <summary>
    /// 대사창만 연다. 본문은 <see cref="bodyText"/>에 이미 적어 둔 TMP Text를 그대로 사용 (인스펙터에서 편집).
    /// </summary>
    public void Show()
    {
        if (bodyText != null)
        {
            ApplyTmpFont(bodyText);
            bodyText.ForceMeshUpdate(true);
        }

        if (panelRoot != null)
            panelRoot.SetActive(true);

        _openedFrame = Time.frameCount;
    }

    /// <summary>코드에서 문장을 넣을 때만 사용 (기본 워크플로는 <see cref="Show()"/>).</summary>
    public void Show(string message)
    {
        if (bodyText != null)
        {
            ApplyTmpFont(bodyText);
            bodyText.text = message;
            bodyText.ForceMeshUpdate(true);
        }

        if (panelRoot != null)
            panelRoot.SetActive(true);

        _openedFrame = Time.frameCount;
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

    /// <summary>
    /// 대사창용 UI 계층을 코드로 생성 (비주얼 노벨식: 화면 하단 전폭 바 + 본문).
    /// DialogueCanvas → DialogueRoot → Dimmer, DialogueBox → BodyText (TMP)
    /// </summary>
    void BuildRuntimeUI()
    {
        EnsureEventSystem();

        var canvasGo = new GameObject("DialogueCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        panelRoot = new GameObject("DialogueRoot");
        var rootRt = panelRoot.AddComponent<RectTransform>();
        rootRt.SetParent(canvasGo.transform, false);
        StretchFull(rootRt);

        var dim = new GameObject("Dimmer");
        var dimRt = dim.AddComponent<RectTransform>();
        dimRt.SetParent(rootRt, false);
        StretchFull(dimRt);
        var dimImg = dim.AddComponent<Image>();
        dimImg.color = new Color(0.08f, 0.1f, 0.15f, 0.55f);
        dimImg.raycastTarget = true;

        const float bottomInset = 16f;
        const float sideInset = 20f;
        const float barHeight = 236f;

        var box = new GameObject("DialogueBox");
        var boxRt = box.AddComponent<RectTransform>();
        boxRt.SetParent(rootRt, false);
        // 하단 전체 폭, 살짝 띄움 (비주얼 노벨 텍스트 창)
        boxRt.anchorMin = new Vector2(0f, 0f);
        boxRt.anchorMax = new Vector2(1f, 0f);
        boxRt.pivot = new Vector2(0.5f, 0f);
        boxRt.sizeDelta = new Vector2(-sideInset * 2f, barHeight);
        boxRt.anchoredPosition = new Vector2(0f, bottomInset);
        var boxImg = box.AddComponent<Image>();
        boxImg.color = new Color(0.96f, 0.97f, 0.99f, 1f);
        boxImg.raycastTarget = true;

        var bodyGo = new GameObject("BodyText");
        var bodyRt = bodyGo.AddComponent<RectTransform>();
        bodyRt.SetParent(boxRt, false);
        bodyRt.anchorMin = Vector2.zero;
        bodyRt.anchorMax = Vector2.one;
        bodyRt.offsetMin = new Vector2(20f, 16f);
        bodyRt.offsetMax = new Vector2(-20f, -16f);

        bodyText = bodyGo.AddComponent<TextMeshProUGUI>();
        bodyText.text = "";
        bodyText.fontSize = 28f;
        bodyText.alignment = TextAlignmentOptions.TopLeft;
        bodyText.color = new Color(0.15f, 0.17f, 0.22f, 1f);
        ApplyTmpFont(bodyText);

    }

    TMP_FontAsset ResolveTmpFont()
    {
        if (koreanFontAsset != null)
            return koreanFontAsset;

        var korean = Resources.Load<TMP_FontAsset>("Fonts/KoreanTMP");
        if (korean != null)
            return korean;

        if (TMP_Settings.defaultFontAsset != null)
            return TMP_Settings.defaultFontAsset;

        var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null)
            return font;

        return Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Fallback");
    }

    void ApplyTmpFont(TextMeshProUGUI tmp)
    {
        var font = ResolveTmpFont();
        if (tmp == null || font == null)
            return;

        tmp.font = font;
    }

    static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
    }
}
