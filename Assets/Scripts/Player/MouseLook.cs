using UnityEngine;

/// <summary>
/// Third-Person Camera Controller — Split Fiction Style
/// Attach this script to your Camera GameObject.
/// Assign the "target" field to your Player transform in the Inspector.
/// </summary>
public class MouseLook: MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Drag your Player transform here")]
    public Transform target;

    [Header("Camera Positioning")]  
    public Vector3 offset = new Vector3(0.6f, 1.6f, -3.0f); // Shoulder offset (right shoulder by default)
    public float followSmoothSpeed = 10f;                     // How fast camera follows player

    [Header("Mouse Look")]
    public float mouseSensitivityX = 3.5f;
    public float mouseSensitivityY = 2.5f;
    public float minVerticalAngle = -30f;   // Look down limit
    public float maxVerticalAngle = 60f;    // Look up limit

    [Header("Zoom / Distance")]
    public float defaultDistance = 3.0f;
    public float minDistance = 1.0f;
    public float maxDistance = 6.0f;
    public float zoomSpeed = 2f;
    public float zoomSmoothSpeed = 8f;

    [Header("Collision")]
    public LayerMask collisionLayers;           // Set to "Default" or whatever your environment uses
    public float collisionRadius = 0.2f;
    public float collisionBuffer = 0.1f;

    [Header("Aim / Lock-On")]
    public bool enableAimMode = true;
    public KeyCode aimKey = KeyCode.Mouse1;     // Right mouse button
    public Vector3 aimOffset = new Vector3(0.4f, 1.5f, -1.5f);
    public float aimFOV = 55f;
    public float defaultFOV = 70f;
    public float fovSmoothSpeed = 8f;

    // ── Private state ────────────────────────────────────────────────
    private float _yaw;           // Horizontal rotation
    private float _pitch;         // Vertical rotation
    private float _currentDistance;
    private float _targetDistance;
    private Camera _cam;
    private bool _isAiming;

    // ── Unity Lifecycle ──────────────────────────────────────────────
    void Start()
    {
        _cam = GetComponent<Camera>();
        if (_cam == null)
            Debug.LogError("[TPSCamera] No Camera component found on this GameObject!");

        // Initialise rotation from current transform so camera doesn't snap on play
        _yaw = transform.eulerAngles.y;
        _pitch = transform.eulerAngles.x;

        _currentDistance = defaultDistance;
        _targetDistance = defaultDistance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleInput();
    }

    void LateUpdate()
    {
        if (target == null) return;

        UpdateCameraPosition();
        UpdateFOV();
    }

    // ── Input ────────────────────────────────────────────────────────
    void HandleInput()
    {
        // Mouse look
        _yaw += Input.GetAxis("Mouse X") * mouseSensitivityX;
        _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
        _pitch = Mathf.Clamp(_pitch, minVerticalAngle, maxVerticalAngle);

        // Scroll zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _targetDistance -= scroll * zoomSpeed;
        _targetDistance = Mathf.Clamp(_targetDistance, minDistance, maxDistance);

        // Aim mode toggle
        _isAiming = enableAimMode && Input.GetKey(aimKey);

        // Unlock cursor on Escape (optional QoL)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // ── Camera Movement ──────────────────────────────────────────────
    void UpdateCameraPosition()
    {
        // Choose offset based on aim state
        Vector3 activeOffset = _isAiming ? aimOffset : offset;

        // Build the rotation
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);

        // Smooth zoom
        _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, Time.deltaTime * zoomSmoothSpeed);

        // Pivot point = player position + world-space shoulder offset (horizontal/vertical only)
        Vector3 pivot = target.position + new Vector3(activeOffset.x, activeOffset.y, 0f);

        // Desired camera position
        Vector3 desiredPos = pivot + rotation * new Vector3(0f, 0f, -_currentDistance);

        // Collision check
        float safeDistance = CheckCollision(pivot, desiredPos);
        Vector3 safePos = pivot + rotation * new Vector3(0f, 0f, -safeDistance);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, safePos, Time.deltaTime * followSmoothSpeed);

        // Always look at the pivot
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * followSmoothSpeed);
    }

    float CheckCollision(Vector3 from, Vector3 to)
    {
        RaycastHit hit;
        Vector3 dir = (to - from).normalized;
        float dist = Vector3.Distance(from, to);

        if (Physics.SphereCast(from, collisionRadius, dir, out hit, dist, collisionLayers))
        {
            return Mathf.Clamp(hit.distance - collisionBuffer, minDistance, _currentDistance);
        }
        return _currentDistance;
    }

    void UpdateFOV()
    {
        if (_cam == null) return;
        float targetFOV = _isAiming ? aimFOV : defaultFOV;
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
    }

    // ── Public Helpers (call from your PlayerMovement script) ────────

    /// <summary>Returns the camera's current yaw so the player can rotate to match.</summary>
    public float GetYaw() => _yaw;

    /// <summary>Returns the camera's forward projected flat on the XZ plane.</summary>
    public Vector3 GetFlatForward()
    {
        Vector3 fwd = transform.forward;
        fwd.y = 0f;
        return fwd.normalized;
    }

    /// <summary>Returns the camera's right projected flat on the XZ plane.</summary>
    public Vector3 GetFlatRight()
    {
        Vector3 right = transform.right;
        right.y = 0f;
        return right.normalized;
    }

    /// <summary>True when the player is holding aim.</summary>
    public bool IsAiming() => _isAiming;
}