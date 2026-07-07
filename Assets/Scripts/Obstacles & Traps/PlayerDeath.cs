using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Header("References")]
    public PlayerDeath otherPlayer;

    private Animator anim;
    private Rigidbody rb;
    private PlayerController controller;

    private bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // Freeze THIS player
        controller.enabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        anim.SetTrigger("Death");

        // Freeze OTHER player
        if (otherPlayer != null)
            otherPlayer.StopPlayer();

        Invoke(nameof(RespawnBoth), 1.5f);
    }

    public void StopPlayer()
    {
        controller.enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    void RespawnBoth()
    {
        // Move BOTH players to latest checkpoint
        Checkpoint.RespawnBoth();

        // Reactivate both players
        Respawn();

        if (otherPlayer != null)
            otherPlayer.Respawn();
    }

    public void Respawn()
    {
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        controller.enabled = true;

        anim.ResetTrigger("Death");
        anim.Play("MainIdle", 0);

        isDead = false;
    }
}