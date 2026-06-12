using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraOrbit cameraOrbit;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 40f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float maxFallSpeed = 20f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Header("Slide")]
    [SerializeField] private float slideForce = 16f;
    [SerializeField] private float slideDuration = 0.55f;
    [SerializeField] private float slideCooldown = 0.8f;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movementInput;
    private bool isGrounded;
    private bool isSliding;
    private bool isSprinting;
    private bool wasSprinting;
    private bool canSlide = true;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private string keyForward = "W";
    private string keyBackward = "S";
    private string keyLeft = "A";
    private string keyRight = "D";
    private string keyJump = "Space";
    private string keySprint = "LeftShift";
    private string keySlide = "LeftControl";
    private string keyInteract = "E";

    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimMoveX = Animator.StringToHash("MoveX");
    private static readonly int AnimMoveZ = Animator.StringToHash("MoveZ");
    private static readonly int AnimGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int AnimSliding = Animator.StringToHash("IsSliding");
    private static readonly int AnimSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int AnimJump = Animator.StringToHash("Jump");
    private static readonly int AnimRunStop = Animator.StringToHash("RunStop");

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Update()
    {
        RefreshKeybinds();
        GatherInput();
        CheckGround();
        HandleJump();
        HandleSlide();
        HandleInteract();
        HandleRunStop();
        UpdateAnimations();

        coyoteTimer -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        HandleRotation();
        BetterJump();
    }

    private void RefreshKeybinds()
    {
        var cm = ControlsManager.Instance;
        if (cm == null) return;
        if (cm.forwardField != null) keyForward = cm.forwardField.text;
        if (cm.backwardField != null) keyBackward = cm.backwardField.text;
        if (cm.leftField != null) keyLeft = cm.leftField.text;
        if (cm.rightField != null) keyRight = cm.rightField.text;
        if (cm.jumpField != null) keyJump = cm.jumpField.text;
        if (cm.sprintField != null) keySprint = cm.sprintField.text;
        if (cm.slideField != null) keySlide = cm.slideField.text;
        if (cm.interactField != null) keyInteract = cm.interactField.text;
    }

    private void GatherInput()
    {
        float h = 0f, v = 0f;
        if (ControlsManager.GetKey(keyForward)) v += 1f;
        if (ControlsManager.GetKey(keyBackward)) v -= 1f;
        if (ControlsManager.GetKey(keyLeft)) h -= 1f;
        if (ControlsManager.GetKey(keyRight)) h += 1f;

        movementInput = new Vector3(h, 0f, v).normalized;
        wasSprinting = isSprinting;
        isSprinting = ControlsManager.GetKey(keySprint) && movementInput != Vector3.zero;

        if (ControlsManager.GetKeyDown(keyJump))
            jumpBufferTimer = jumpBufferTime;
    }

    private Vector3 GetCameraRelativeMoveDir()
    {
        if (cameraOrbit == null) return Vector3.zero;

        Vector3 camForward = Quaternion.Euler(0f, cameraOrbit.yaw, 0f) * Vector3.forward;
        Vector3 camRight = Quaternion.Euler(0f, cameraOrbit.yaw, 0f) * Vector3.right;

        return (camForward * movementInput.z + camRight * movementInput.x);
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            coyoteTimer = coyoteTime;
        else if (wasGrounded)
            coyoteTimer = coyoteTime;
    }

    private void HandleJump()
    {
        bool canJump = coyoteTimer > 0f && !isSliding;

        if (jumpBufferTimer > 0f && canJump)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            animator.ResetTrigger(AnimJump);
            animator.SetTrigger(AnimJump);
        }
    }

    private void BetterJump()
    {
        bool jumpHeld = ControlsManager.GetKey(keyJump);

        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
            if (rb.linearVelocity.y < -maxFallSpeed)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, rb.linearVelocity.z);
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    private void MovePlayer()
    {
        if (isSliding) return;

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 moveDir = GetCameraRelativeMoveDir();
        Vector3 targetVelocity = moveDir.normalized * targetSpeed * (movementInput == Vector3.zero ? 0f : 1f);
        Vector3 currentVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        float moveRate = movementInput != Vector3.zero ? acceleration : deceleration;

        Vector3 smoothVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, moveRate * Time.fixedDeltaTime);

        if (movementInput == Vector3.zero && smoothVelocity.magnitude < 0.05f)
            smoothVelocity = Vector3.zero;

        rb.linearVelocity = new Vector3(smoothVelocity.x, rb.linearVelocity.y, smoothVelocity.z);
    }

    private void HandleRotation()
    {
        if (cameraOrbit == null) return;
        if (Time.timeScale == 0f) return;

        Quaternion targetRotation;

        if (movementInput != Vector3.zero)
        {
            // Face the direction of movement (camera-relative)
            Vector3 moveDir = GetCameraRelativeMoveDir();
            targetRotation = Quaternion.LookRotation(moveDir.normalized);
        }
        else
        {
            // Idle: follow camera yaw (mouse free-look)
            targetRotation = Quaternion.Euler(0f, cameraOrbit.yaw, 0f);
        }

        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }

    private void HandleSlide()
    {
        if (ControlsManager.GetKeyDown(keySlide) && canSlide && isGrounded && isSprinting)
            StartCoroutine(SlideCoroutine());
    }

    private void HandleInteract()
    {
        if (ControlsManager.GetKeyDown(keyInteract))
            Debug.Log("Interact Pressed");
    }

    private IEnumerator SlideCoroutine()
    {
        canSlide = false;
        isSliding = true;
        Vector3 slideDir = transform.forward;
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        rb.AddForce(slideDir * slideForce, ForceMode.VelocityChange);
        yield return new WaitForSeconds(slideDuration);
        isSliding = false;
        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    private void HandleRunStop()
    {
        if (wasSprinting && !isSprinting && movementInput == Vector3.zero && isGrounded && !isSliding)
            animator.SetTrigger(AnimRunStop);
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);

        animator.SetFloat(AnimMoveX, localVel.x, 0.03f, Time.deltaTime);
        animator.SetFloat(AnimMoveZ, localVel.z, 0.03f, Time.deltaTime);
        animator.SetFloat(AnimSpeed, new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude, 0.03f, Time.deltaTime);
        animator.SetBool(AnimGrounded, isGrounded);
        animator.SetBool(AnimSliding, isSliding);
        animator.SetBool(AnimSprinting, isSprinting);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) < 0.3f && rb.linearVelocity.y < 0.1f)
            {
                rb.position += Vector3.up * 0.02f;
            }
        }
    }
}