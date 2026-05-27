using UnityEngine;

public class SettingsArrow : MonoBehaviour
{
    public RectTransform arrow;

    private float targetY;

    private void Start()
    {
        targetY = arrow.anchoredPosition.y;
    }

    private void Update()
    {
        Vector2 pos = arrow.anchoredPosition;

        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 10f);

        arrow.anchoredPosition = pos;
    }

    public void SelectGeneral()
    {
        targetY = 356f;
    }

    public void SelectGraphics()
    {
        targetY = 144f;
    }

    public void SelectControls()
    {
        targetY = -78f;
    }

    public void SelectSounds()
    {
        targetY = -305f;
    }
}