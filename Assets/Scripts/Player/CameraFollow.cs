using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -7);

    void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}