using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RespawnFade : MonoBehaviour
{
    public static RespawnFade Instance;

    public float fadeInSpeed = 3f;
    public float fadeOutSpeed = 3f;
    public float blackScreenDuration = 0.2f;

    private Image fadeImage;

    private void Awake()
    {
        Instance = this;
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator Fade()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * fadeInSpeed;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        yield return new WaitForSeconds(blackScreenDuration);

        Checkpoint.RespawnBoth();

        yield return new WaitForSeconds(0.1f);

        t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * fadeOutSpeed;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }
}