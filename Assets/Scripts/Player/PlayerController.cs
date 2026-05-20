using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // ─────────────────────────────────────────
    //  Movement
    // ─────────────────────────────────────────
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float acceleration = 15f;   // how fast we reach target speed
    [SerializeField] private float deceleration = 20f;   // how fast we brake when no input
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float airControlFactor = 0.4f; // 0 = no air-steer, 1 = full control

    // ─────────────────────────────────────────
    //  Jump
    // ─────────────────────────────────────────
    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private int maxExtraJumps = 0;   // set to 1 for double-jump

    [Header("Coyote Time & Jump Buffer")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.12f;

    // ─────────────────────────────────────────
    //  Ground Check
    // ─────────────────────────────────────────
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;

    // ─────────────────────────────────────────
    //  Slide
    // ─────────────────────────────────────────
    [Header("Slide")]
    [SerializeField] private float slideForce = 8f;
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float slideCooldown = 1f;

    // ─────────────────────────────────────────
    //  Footstep Audio (optional)
    // ─────────────────────────────────────────
    [Header("Footsteps (optional)")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.45f;  // seconds between steps at walk
    [SerializeField] private float sprintStepFactor = 0.65f;  // multiplier while sprinting

    // ─────────────────────────────────────────
    //  Cached refs & state
    // ─────────────────────────────────────────
    private Rigidbody rb;
    private Animator animator;

    private Vector3 movementInput;
    private Vector3 currentVelocityXZ; // smooth-damp reference

    private bool isGrounded;
    private bool wasGrounded;
    private bool isSliding;
    private bool isSprinting;
    private bool canSlide = true;

    // Jump bookkeeping
    private int extraJumpsLeft;
    private float coyoteTimer;
    private float jumpBufferTimer;

    // Footstep bookkeeping
    private float stepTimer;

    // Animator hash IDs (avoid string-lookup cost every frame)
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int AnimSliding = Animator.StringToHash("IsSliding");
    private static readonly int AnimSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int AnimJump = Animator.StringToHash("Jump");
    private static readonly int AnimLand = Animator.StringToHash("Land");

    // ─────────────────────────────────────────
    //  Unity lifecycle
    // ─────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // Prevent physics engine from tipping the capsule over
        rb.freezeRotation = true;

        extraJumpsLeft = maxExtraJumps;
    }

    private void Update()
    {
        GatherInput();
        CheckGround();
        UpdateTimers();
        HandleJumpInput();
        HandleSlide();
        RotatePlayer();
        HandleFootsteps();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        BetterJump();
    }

    // ─────────────────────────────────────────
    //  Input
    // ─────────────────────────────────────────
    private void GatherInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        movementInput = new Vector3(h, 0f, v).normalized;

        isSprinting = Input.GetKey(KeyCode.LeftShift) && movementInput != Vector3.zero;
    }

    // ─────────────────────────────────────────
    //  Timers (coyote + jump buffer)
    // ─────────────────────────────────────────
    private void UpdateTimers()
    {
        // Coyote: counts up after leaving ground
        if (!isGrounded)
            coyoteTimer += Time.deltaTime;
        else
            coyoteTimer = 0f;

        // Jump buffer: counts down after jump key press
        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;
    }

    // ─────────────────────────────────────────
    //  Ground Check
    // ─────────────────────────────────────────
    private void CheckGround()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Reset extra jumps when landing
        if (isGrounded && !wasGrounded)
            extraJumpsLeft = maxExtraJumps;
    }

    // ─────────────────────────────────────────
    //  Movement (smooth acceleration / decel)
    // ─────────────────────────────────────────
    private void MovePlayer()
    {
        if (isSliding) return;

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // The desired XZ velocity this frame
        Vector3 targetVelocity = movementInput * targetSpeed;

        // Reduce steering authority in the air
        float control = isGrounded ? 1f : airControlFactor;

        float rate = movementInput != Vector3.zero
            ? acceleration * control
            : deceleration * control;

        // Smoothly interpolate horizontal velocity
        Vector3 newXZ = Vector3.MoveTowards(
            new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z),
            targetVelocity,
            rate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(newXZ.x, rb.linearVelocity.y, newXZ.z);
    }

    // ─────────────────────────────────────────
    //  Rotation
    // ─────────────────────────────────────────
    private void RotatePlayer()
    {
        if (movementInput == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(movementInput);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // ─────────────────────────────────────────
    //  Jump (coyote time + jump buffer)
    // ─────────────────────────────────────────
    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;

        bool canJumpNow =
            (isGrounded || coyoteTimer <= coyoteTime) &&   // grounded or coyote window
            jumpBufferTimer > 0f;

        bool canExtraJump =
            !isGrounded &&
            extraJumpsLeft > 0 &&
            jumpBufferTimer > 0f &&
            coyoteTimer > coyoteTime; // must be truly airborne

        if (canJumpNow || canExtraJump)
        {
            if (canExtraJump) extraJumpsLeft--;

            jumpBufferTimer = 0f;
            coyoteTimer = coyoteTime + 1f; // consume coyote

            // Zero out downward velocity before adding jump force
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (animator != null)
                animator.SetTrigger(AnimJump);
        }
    }

    // ─────────────────────────────────────────
    //  Better jump (gravity scaling)
    // ─────────────────────────────────────────
    private void BetterJump()
    {
        if (rb.linearVelocity.y < 0f)
        {
            // Falling: add extra gravity
            rb.linearVelocity += Vector3.up *
                Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetKey(KeyCode.Space))
        {
            // Rising but button released: short-hop
            rb.linearVelocity += Vector3.up *
                Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    // ─────────────────────────────────────────
    //  Slide
    // ─────────────────────────────────────────
    private void HandleSlide()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) &&
            canSlide && isGrounded && movementInput != Vector3.zero)
        {
            StartCoroutine(SlideCoroutine());
        }
    }

    private IEnumerator SlideCoroutine()
    {
        canSlide = false;
        isSliding = true;

        Vector3 slideDir = transform.forward;

        // Kill current XZ speed, then blast in slide direction
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        rb.AddForce(slideDir * slideForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(slideDuration);
        isSliding = false;

        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    // ─────────────────────────────────────────
    //  Footsteps
    // ─────────────────────────────────────────
    private void HandleFootsteps()
    {
        if (footstepSource == null || footstepClips == null || footstepClips.Length == 0)
            return;

        if (!isGrounded || isSliding || movementInput == Vector3.zero)
        {
            stepTimer = 0f;
            return;
        }

        float interval = isSprinting
            ? stepInterval * sprintStepFactor
            : stepInterval;

        stepTimer += Time.deltaTime;

        if (stepTimer >= interval)
        {
            stepTimer = 0f;
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepSource.PlayOneShot(clip);
        }
    }

    // ─────────────────────────────────────────
    //  Animations
    // ─────────────────────────────────────────
    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetFloat(AnimSpeed, movementInput.magnitude);
        animator.SetBool(AnimGrounded, isGrounded);
        animator.SetBool(AnimSliding, isSliding);
        animator.SetBool(AnimSprinting, isSprinting);

        // Landing event
        if (isGrounded && !wasGrounded)
            animator.SetTrigger(AnimLand);
    }

    // ─────────────────────────────────────────
    //  Gizmos
    // ─────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}