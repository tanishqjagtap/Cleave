using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Respawn Points")]
    public Transform mayaSpawn;
    public Transform lenaSpawn;

    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (!other.CompareTag("Player"))
            return;

        activated = true;

        CheckpointManager.Instance.SetCheckpoint(
            mayaSpawn,
            lenaSpawn);

        Debug.Log("Checkpoint Activated");
    }
}