using UnityEngine;

public class PlayerDistanceManager : MonoBehaviour
{
    public Transform maya;
    public Transform lena;

    public float maxDistance = 30f;

    void Update()
    {
        if (maya == null || lena == null)
            return;

        float zDistance = Mathf.Abs(maya.position.z - lena.position.z);

        if (zDistance > maxDistance)
        {
            CheckpointManager.Instance.RespawnPlayers();
        }
    }
}