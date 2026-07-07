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
        // Register players with Checkpoint system
        Checkpoint.maya = maya;
        Checkpoint.lena = lena;

        // Save their original X positions
        Checkpoint.mayaOriginalX = maya.position.x;
        Checkpoint.lenaOriginalX = lena.position.x;

        // Set the very first checkpoint
        Checkpoint.SetDefaultSpawns(maya.position, lena.position);
    }

    private void Update()
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

        // Respawn both players at the latest checkpoint
        Checkpoint.RespawnBoth();

        isRespawning = false;
    }
}