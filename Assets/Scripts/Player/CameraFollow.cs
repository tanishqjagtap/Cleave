using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Camera Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);

    [Header("Follow Smoothness")]
    [SerializeField] private float smoothSpeed = 8f;

    [Header("Strafe Nudge")]
    [Tooltip("How far the camera shifts sideways when strafing (A/D)")]
    [SerializeField] private float strafeNudgeAmount = 1.2f;

    [Tooltip("How fast the nudge blends in and out")]
    [SerializeField] private float nudgeSmoothSpeed = 4f;

    [Tooltip("How much the camera subtly rotates (yaw) toward the strafe direction")]
    [SerializeField] private float strafeYawAmount = 4f;          // degrees

    // ── private state ─────────────────────────────────────────────
    private float _currentNudge;      // current smoothed sideways offset
    private float _currentYaw;        // current smoothed yaw offset

    private void LateUpdate()
    {
        if (target == null) return;

        // ── 1. Read strafe input (works with both keyboard and gamepad) ──
        float strafeInput = Input.GetAxis("Horizontal"); // A/D or Left Stick X

        // ── 2. Smooth the nudge value toward the input ──────────────────
        _currentNudge = Mathf.Lerp(_currentNudge, strafeInput * strafeNudgeAmount, nudgeSmoothSpeed * Time.deltaTime);
        _currentYaw = Mathf.Lerp(_currentYaw, strafeInput * strafeYawAmount, nudgeSmoothSpeed * Time.deltaTime);

        // ── 3. Build world-space right vector from the TARGET'S facing ──
        //    (so nudge is always relative to where the player looks)
        Vector3 targetRight = target.right;
        targetRight.y = 0f;
        targetRight.Normalize();

        // ── 4. Desired position = base follow + sideways nudge ──────────
        Vector3 desiredPosition = target.position + offset + targetRight * _currentNudge;

        // ── 5. Smooth follow (your original logic, unchanged) ────────────
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // ── 6. Soft yaw lean toward strafe direction ────────────────────
        //    Keep the camera looking at the player but with a slight turn bias
        Vector3 lookTarget = target.position + new Vector3(0f, offset.y * 0.5f, 0f);
        Quaternion baseLook = Quaternion.LookRotation(lookTarget - transform.position);
        Quaternion yawLean = Quaternion.Euler(0f, _currentYaw, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            yawLean * baseLook,
            nudgeSmoothSpeed * Time.deltaTime
        );
    }
}