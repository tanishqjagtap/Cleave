using UnityEngine;

public class PlayerDistanceManager : MonoBehaviour
{
    public Transform maya;
    public Transform lena;

    public float maxDistance = 20f;

    private float respawnCooldown = 3f;
    private float lastRespawnTime = -999f;
    private bool isRespawning = false;

    private void Start()
    {
        Checkpoint.maya = maya;
        Checkpoint.lena = lena;
        Checkpoint.mayaOriginalX = maya.position.x;
        Checkpoint.lenaOriginalX = lena.position.x;
        Checkpoint.spawnZ = maya.position.z;
    }

    void Update()
    {
        if (maya == null || lena == null) return;
        if (isRespawning) return;

        float zDistance = Mathf.Abs(maya.position.z - lena.position.z);

        if (zDistance > maxDistance && Time.time - lastRespawnTime > respawnCooldown)
        {
            lastRespawnTime = Time.time;
            isRespawning = true;
            StartCoroutine(RespawnWithFade());
        }
    }

    private System.Collections.IEnumerator RespawnWithFade()
    {
        yield return StartCoroutine(RespawnFade.Instance.Fade());
        isRespawning = false;
    }
}