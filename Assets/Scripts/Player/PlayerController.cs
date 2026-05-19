using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Header("Slide")]
    [SerializeField] private float slideForce = 8f;
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float slideCooldown = 1f;

    private Rigidbody rb;
    private Animator animator;

    private Vector3 movementInput;

    private bool isGrounded;
    private bool isSliding;
    private bool isSprinting;
    private bool canSlide = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        GatherInput();

        CheckGround();

        RotatePlayer();

        HandleJump();

        HandleSlide();

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        BetterJump();
    }

    private void GatherInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movementInput =
            new Vector3(horizontal, 0f, vertical).normalized;

        isSprinting =
            Input.GetKey(KeyCode.LeftShift) &&
            movementInput != Vector3.zero;
    }

    private void MovePlayer()
    {
        if (isSliding)
            return;

        float currentSpeed =
            isSprinting ? sprintSpeed : walkSpeed;

        Vector3 movement =
            movementInput *
            currentSpeed *
            Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);
    }

    private void RotatePlayer()
    {
        if (movementInput == Vector3.zero)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(movementInput);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );
        }
    }

    private void BetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity +=
                Vector3.up *
                Physics.gravity.y *
                (fallMultiplier - 1) *
                Time.fixedDeltaTime;
        }
        else if (
            rb.linearVelocity.y > 0 &&
            !Input.GetKey(KeyCode.Space)
        )
        {
            rb.linearVelocity +=
                Vector3.up *
                Physics.gravity.y *
                (lowJumpMultiplier - 1) *
                Time.fixedDeltaTime;
        }
    }

    private void HandleSlide()
    {
        if (
            Input.GetKeyDown(KeyCode.LeftControl) &&
            canSlide &&
            isGrounded &&
            movementInput != Vector3.zero
        )
        {
            StartCoroutine(SlideCoroutine());
        }
    }

    private System.Collections.IEnumerator SlideCoroutine()
    {
        canSlide = false;
        isSliding = true;

        Vector3 slideDirection = transform.forward;

        rb.linearVelocity = new Vector3(
            0f,
            rb.linearVelocity.y,
            0f
        );

        rb.AddForce(
            slideDirection * slideForce,
            ForceMode.VelocityChange
        );

        yield return new WaitForSeconds(slideDuration);

        isSliding = false;

        yield return new WaitForSeconds(slideCooldown);

        canSlide = true;
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void UpdateAnimations()
    {
        if (animator == null)
            return;

        animator.SetFloat(
            "Speed",
            movementInput.magnitude
        );

        animator.SetBool(
            "IsGrounded",
            isGrounded
        );

        animator.SetBool(
            "IsSliding",
            isSliding
        );

        animator.SetBool(
            "IsSprinting",
            isSprinting
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}