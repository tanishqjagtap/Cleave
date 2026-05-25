using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

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

    private static readonly int AnimSpeed =
        Animator.StringToHash("Speed");

    private static readonly int AnimGrounded =
        Animator.StringToHash("IsGrounded");

    private static readonly int AnimSliding =
        Animator.StringToHash("IsSliding");

    private static readonly int AnimSprinting =
        Animator.StringToHash("IsSprinting");

    private static readonly int AnimJump =
        Animator.StringToHash("Jump");

    private static readonly int AnimRunStop =
        Animator.StringToHash("RunStop");

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        animator = GetComponentInChildren<Animator>();

        rb.freezeRotation = true;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;
    }

    private void Update()
    {
        GatherInput();

        CheckGround();

        HandleJump();

        HandleSlide();

        HandleRunStop();

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        MovePlayer();

        BetterJump();
    }

    private void GatherInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        movementInput =
            new Vector3(h, 0f, v).normalized;

        wasSprinting = isSprinting;

        isSprinting =
            Input.GetKey(KeyCode.LeftShift) &&
            movementInput != Vector3.zero;
    }

    private void MovePlayer()
    {
        if (isSliding)
            return;

        float targetSpeed =
            isSprinting
            ? sprintSpeed
            : walkSpeed;

        Vector3 targetVelocity =
            movementInput * targetSpeed;

        Vector3 currentVelocity =
            new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

        float moveRate =
            movementInput != Vector3.zero
            ? acceleration
            : deceleration;

        Vector3 smoothVelocity =
            Vector3.MoveTowards(
                currentVelocity,
                targetVelocity,
                moveRate * Time.fixedDeltaTime
            );

        rb.linearVelocity =
            new Vector3(
                smoothVelocity.x,
                rb.linearVelocity.y,
                smoothVelocity.z
            );

        if (movementInput != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    movementInput
                );

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed *
                    Time.fixedDeltaTime
                )
            );
        }
    }

    private void HandleJump()
    {
        if (
            Input.GetKeyDown(KeyCode.Space)
            && isGrounded
            && !isSliding
        )
        {
            rb.linearVelocity =
                new Vector3(
                    rb.linearVelocity.x,
                    0f,
                    rb.linearVelocity.z
                );

            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );

            animator.ResetTrigger(
                AnimJump
            );

            animator.SetTrigger(
                AnimJump
            );
        }
    }

    private void BetterJump()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity +=
                Vector3.up *
                Physics.gravity.y *
                (fallMultiplier - 1f) *
                Time.fixedDeltaTime;
        }
        else if (
            rb.linearVelocity.y > 0f &&
            !Input.GetKey(KeyCode.Space)
        )
        {
            rb.linearVelocity +=
                Vector3.up *
                Physics.gravity.y *
                (lowJumpMultiplier - 1f) *
                Time.fixedDeltaTime;
        }
    }

    private void HandleSlide()
    {
        if (
            Input.GetKeyDown(KeyCode.LeftControl)
            && canSlide
            && isGrounded
            && isSprinting
        )
        {
            StartCoroutine(
                SlideCoroutine()
            );
        }
    }

    private IEnumerator SlideCoroutine()
    {
        canSlide = false;

        isSliding = true;

        Vector3 slideDir =
            transform.forward;

        rb.linearVelocity =
            new Vector3(
                0f,
                rb.linearVelocity.y,
                0f
            );

        rb.AddForce(
            slideDir * slideForce,
            ForceMode.VelocityChange
        );

        yield return new WaitForSeconds(
            slideDuration
        );

        isSliding = false;

        yield return new WaitForSeconds(
            slideCooldown
        );

        canSlide = true;
    }

    private void HandleRunStop()
    {
        if (
            wasSprinting &&
            !isSprinting &&
            movementInput == Vector3.zero &&
            isGrounded &&
            !isSliding
        )
        {
            animator.SetTrigger(
                AnimRunStop
            );
        }
    }

    private void CheckGround()
    {
        isGrounded =
            Physics.CheckSphere(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
    }

    private void UpdateAnimations()
    {
        if (animator == null)
            return;

        Vector3 horizontalVelocity =
            new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

        animator.SetFloat(
            AnimSpeed,
            horizontalVelocity.magnitude
        );

        animator.SetBool(
            AnimGrounded,
            isGrounded
        );

        animator.SetBool(
            AnimSliding,
            isSliding
        );

        animator.SetBool(
            AnimSprinting,
            isSprinting
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color =
            isGrounded
            ? Color.green
            : Color.red;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}