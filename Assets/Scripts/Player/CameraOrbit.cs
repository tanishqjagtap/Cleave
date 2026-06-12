using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float mouseSensitivity = 3f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public float collisionOffset = 0.2f;
    public float minDistance = 0.5f;

    [HideInInspector] public float yaw;
    [HideInInspector] public float pitch = 15f;

    private void LateUpdate()
    {
        if (target == null) return;
        if (Time.timeScale == 0f) return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivotPos = target.position + Vector3.up * height;
        Vector3 desiredPosition = pivotPos - (rotation * Vector3.forward * distance);

        float actualDistance = distance;

        if (Physics.Linecast(pivotPos, desiredPosition, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Player"))
                actualDistance = Mathf.Clamp(hit.distance - collisionOffset, minDistance, distance);
        }

        Vector3 finalPosition = pivotPos - (rotation * Vector3.forward * actualDistance);

        transform.position = finalPosition;
        transform.LookAt(pivotPos);
    }
}