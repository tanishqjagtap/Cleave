using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour
{
    public float openHeight = 5f;
    public float speed = 3f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool opened;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    public void OpenGate()
    {
        if (opened) return;

        opened = true;
        StartCoroutine(OpenRoutine());
    }

    IEnumerator OpenRoutine()
    {
        while (Vector3.Distance(transform.position, openPos) > 0.05f)
        {
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    openPos,
                    speed * Time.deltaTime);

            yield return null;
        }
    }
}