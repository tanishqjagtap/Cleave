using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;
    private PlayerController controller;

    private bool isDead;

    private void Start()
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

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (controller != null)
            controller.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (anim != null)
            anim.SetTrigger("Death");

        Invoke(nameof(Respawn), 1.5f);
    }

    private void Respawn()
    {
        transform.position = new Vector3(0, 1, 0); // temporary respawn point

        if (rb != null)
            rb.isKinematic = false;

        if (controller != null)
            controller.enabled = true;

        isDead = false;
    }
}