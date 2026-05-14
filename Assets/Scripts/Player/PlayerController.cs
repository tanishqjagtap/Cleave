using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 7f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody rb;

    private Vector3 movementInput;

    private bool isGrounded;
    private bool canDash = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GatherInput();
        RotatePlayer();
        HandleJump();
        HandleDash();
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

        movementInput = new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void MovePlayer()
    {
        Vector3 movement =
            movementInput * moveSpeed * Time.fixedDeltaTime;

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
        CheckGround();

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
            rb.linearVelocity += Vector3.up *
                Physics.gravity.y *
                (fallMultiplier - 1) *
                Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 &&
                 !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector3.up *
                Physics.gravity.y *
                (lowJumpMultiplier - 1) *
                Time.fixedDeltaTime;
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private System.Collections.IEnumerator DashCoroutine()
    {
        canDash = false;

        Vector3 dashDirection =
            movementInput != Vector3.zero
            ? movementInput
            : transform.forward;

        rb.linearVelocity = Vector3.zero;

        rb.AddForce(
            dashDirection * dashForce,
            ForceMode.VelocityChange
        );

        yield return new WaitForSeconds(0.15f);

        rb.linearVelocity = new Vector3(
            0f,
            rb.linearVelocity.y,
            0f
        );

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }
}