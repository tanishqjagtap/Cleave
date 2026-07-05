using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Header("References")]
    public PlayerDeath otherPlayer;
    public Transform respawnPoint;

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
        {
            otherPlayer.StopPlayer();
        }

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
        Respawn();

        if (otherPlayer != null)
            otherPlayer.Respawn();
    }

    public void Respawn()
    {
        // Move player
        transform.position = respawnPoint.position;

        // Reset physics
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset animator completely
        anim.Rebind();
        anim.Update(0f);

        // Enable movement again
        controller.enabled = true;

        isDead = false;
    }
}