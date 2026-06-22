using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public Transform maya;
    public Transform lena;

    public Transform currentCheckpoint;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    public void RespawnPlayers()
    {
        maya.position = currentCheckpoint.position;
        lena.position = currentCheckpoint.position;

        Debug.Log("Players Respawned");
    }
}