using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Players")]
    public Transform maya;
    public Transform lena;

    [Header("Current Checkpoint")]
    public Transform currentMayaCheckpoint;
    public Transform currentLenaCheckpoint;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetCheckpoint(Transform mayaCheckpoint, Transform lenaCheckpoint)
    {
        currentMayaCheckpoint = mayaCheckpoint;
        currentLenaCheckpoint = lenaCheckpoint;

        Debug.Log("Checkpoint Updated");
    }

    public void RespawnPlayers()
    {
        if (currentMayaCheckpoint == null || currentLenaCheckpoint == null)
            return;

        maya.position = currentMayaCheckpoint.position;
        lena.position = currentLenaCheckpoint.position;

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

        Debug.Log("Players Respawned");
    }
}