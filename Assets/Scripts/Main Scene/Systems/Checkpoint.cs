using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex = 0;

    private static int latestIndex = -1;

    // Current active checkpoint
    public static Transform currentCheckpoint;

    // Player references
    public static Transform maya;
    public static Transform lena;

    // Original X positions so each player stays on their own side
    public static float mayaOriginalX;
    public static float lenaOriginalX;

    // Called once at game start
    public static void SetDefaultSpawns(Vector3 mayaPos, Vector3 lenaPos)
    {
        mayaOriginalX = mayaPos.x;
        lenaOriginalX = lenaPos.x;

        latestIndex = 0;

        // Start checkpoint
        if (maya != null)
            currentCheckpoint = maya;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (checkpointIndex <= latestIndex)
            return;

        latestIndex = checkpointIndex;
        currentCheckpoint = transform;

        Debug.Log("Checkpoint " + checkpointIndex + " activated.");
    }

    public static void RespawnBoth()
    {
        if (currentCheckpoint == null)
        {
            Debug.LogWarning("No checkpoint has been activated!");
            return;
        }

        Vector3 mayaSpawn = new Vector3(
            mayaOriginalX,
            currentCheckpoint.position.y,
            currentCheckpoint.position.z
        );

        Vector3 lenaSpawn = new Vector3(
            lenaOriginalX,
            currentCheckpoint.position.y,
            currentCheckpoint.position.z
        );

        maya.position = mayaSpawn;
        lena.position = lenaSpawn;

        Rigidbody mayaRb = maya.GetComponent<Rigidbody>();
        Rigidbody lenaRb = lena.GetComponent<Rigidbody>();

        if (mayaRb != null)
        {
            mayaRb.linearVelocity = Vector3.zero;
            mayaRb.angularVelocity = Vector3.zero;
        }

        if (lenaRb != null)
        {
            lenaRb.linearVelocity = Vector3.zero;
            lenaRb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Respawned both players.");
    }
}