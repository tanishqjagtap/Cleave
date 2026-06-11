using UnityEngine;
using System.Collections;

public class ButtonInteract : MonoBehaviour
{
    public GateController gate;

    private Vector3 startPos;
    private bool pressed;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (pressed) return;

        string interactKey = ControlsManager.Instance.interactField.text;

        if (ControlsManager.GetKeyDown(interactKey))
        {
            float distance =
                Vector3.Distance(
                    GameObject.Find("Lena").transform.position,
                    transform.position);

            if (distance < 3f)
            {
                pressed = true;

                StartCoroutine(PressButton());

                if (gate != null)
                    gate.OpenGate();
            }
        }
    }

    IEnumerator PressButton()
    {
        Vector3 pressedPos =
            startPos + Vector3.forward * 0.1f;

        while (Vector3.Distance(transform.localPosition, pressedPos) > 0.01f)
        {
            transform.localPosition =
                Vector3.MoveTowards(
                    transform.localPosition,
                    pressedPos,
                    2f * Time.deltaTime);

            yield return null;
        }
    }
}