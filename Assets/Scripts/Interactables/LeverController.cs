using UnityEngine;
using System.Collections;

public class LeverController : MonoBehaviour
{
    public float interactDistance = 3f;
    public Transform maya;
    public Transform lena;
    public float duration = 0.5f;
    public HologramWall gate;

    private bool activated;

    private Vector3 startPos = new Vector3(0f, 0f, 0f);
    private Quaternion startRot = Quaternion.Euler(-90f, 0f, 0f);

    private Vector3 targetPos = new Vector3(0f, 0.015f, -0.072f);
    private Quaternion targetRot = Quaternion.Euler(-134.008f, 0f, 0f);

    private void Start()
    {
        transform.localPosition = startPos;
        transform.localRotation = startRot;
    }

    private void Update()
    {
        if (activated) return;

        string interactKey = ControlsManager.Instance.interactField.text;

        if (ControlsManager.GetKeyDown(interactKey))
        {
            float distMaya = Vector3.Distance(maya.position, transform.position);
            float distLena = Vector3.Distance(lena.position, transform.position);

            if (distMaya < interactDistance || distLena < interactDistance)
            {
                Debug.Log("Lever Pulled!");
                PullLever();
            }
        }
    }

    public void PullLever()
    {
        if (activated) return;
        activated = true;
        if (gate != null) gate.OpenGate();
        StartCoroutine(RotateLever());
    }

    IEnumerator RotateLever()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            transform.localPosition = Vector3.Lerp(startPos, targetPos, smooth);
            transform.localRotation = Quaternion.Lerp(startRot, targetRot, smooth);

            yield return null;
        }

        transform.localPosition = targetPos;
        transform.localRotation = targetRot;
    }
}