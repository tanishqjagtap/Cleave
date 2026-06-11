using UnityEngine;
using System.Collections;

public class HologramWall : MonoBehaviour
{
    public GameObject gate;
    public float glitchDuration = 0.6f;

    public void OpenGate()
    {
        StartCoroutine(GlitchDisappear());
    }

    IEnumerator GlitchDisappear()
    {
        Renderer rend = gate.GetComponent<Renderer>();
        Material mat = rend.material;

        Vector3 originalPos = gate.transform.position;
        Color originalColor = mat.color;

        float elapsed = 0f;

        while (elapsed < glitchDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / glitchDuration;

            // Random position shake
            gate.transform.position = originalPos + new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.05f, 0.05f),
                0f);

            // Flicker color between blue and white and dark
            if (Random.value > 0.5f)
                mat.color = Color.white;
            else if (Random.value > 0.7f)
                mat.color = Color.black;
            else
                mat.color = originalColor;

            // Fade alpha out progressively
            Color c = mat.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            mat.color = c;

            // Random scale glitch
            float scaleGlitch = Random.Range(0.95f, 1.05f);
            gate.transform.localScale = new Vector3(
                scaleGlitch,
                gate.transform.localScale.y,
                gate.transform.localScale.z);

            yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));
        }

        gate.transform.position = originalPos;
        gate.SetActive(false);
    }
}