using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 targetScale;
    private Vector3 targetPosition;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private TextMeshProUGUI text;

    private float targetAlpha = 1f;

    private void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;

        targetScale = originalScale;
        targetPosition = originalPosition;

        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        transform.localScale =
            Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * 10f);

        transform.localPosition =
            Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.deltaTime * 10f);

        if (text != null)
        {
            Color c = text.color;
            c.a = Mathf.Lerp(
                c.a,
                targetAlpha,
                Time.deltaTime * 10f);

            text.color = c;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * 1.1f;
        targetPosition = originalPosition + Vector3.up * 10f;
        targetAlpha = 0.8f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        targetPosition = originalPosition;
        targetAlpha = 1f;
    }
}