using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex = 0;

    private static int latestIndex = -1;
    public static float spawnZ;

    public static Transform maya;
    public static Transform lena;
    public static float mayaOriginalX;
    public static float lenaOriginalX;

    public static void SetDefaultSpawns(Vector3 mayaPos, Vector3 lenaPos)
    {
        spawnZ = mayaPos.z;
        latestIndex = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (checkpointIndex <= latestIndex) return;

        latestIndex = checkpointIndex;
        spawnZ = transform.position.z;
        Debug.Log("Checkpoint " + checkpointIndex + " set at Z=" + spawnZ);
    }

    public static void RespawnBoth()
    {
        maya.position = new Vector3(mayaOriginalX, maya.position.y, spawnZ);
        lena.position = new Vector3(lenaOriginalX, lena.position.y, spawnZ);

        Rigidbody mayaRb = maya.GetComponent<Rigidbody>();
        Rigidbody lenaRb = lena.GetComponent<Rigidbody>();

        if (mayaRb != null) mayaRb.linearVelocity = Vector3.zero;
        if (lenaRb != null) lenaRb.linearVelocity = Vector3.zero;

        Debug.Log("Respawned at Z=" + spawnZ);
    }
}