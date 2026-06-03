using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Effect")]
    public float hoverScale = 1.1f;
    public float speed = 8f;

    [Header("Color Effect")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;

    Vector3 originalScale;
    Vector3 targetScale;
    Color targetColor;

    Text labelText;
    TMPro.TextMeshProUGUI labelTMP;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        targetColor = normalColor;

        // grab whichever text component exists
        labelText = GetComponentInChildren<Text>();
        labelTMP = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        SetColor(normalColor);
    }

    void Update()
    {
        // smooth scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);

        // smooth color
        SetColor(Color.Lerp(GetColor(), targetColor, Time.unscaledDeltaTime * speed));
    }

    public void OnPointerEnter(PointerEventData e)
    {
        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;
    }

    public void OnPointerExit(PointerEventData e)
    {
        targetScale = originalScale;
        targetColor = normalColor;
    }

    void SetColor(Color c)
    {
        if (labelTMP != null) labelTMP.color = c;
        if (labelText != null) labelText.color = c;
    }

    Color GetColor()
    {
        if (labelTMP != null) return labelTMP.color;
        if (labelText != null) return labelText.color;
        return normalColor;
    }
}