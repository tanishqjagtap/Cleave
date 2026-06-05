using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider loadingBar;

    public void LoadGame()
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation =
            SceneManager.LoadSceneAsync("Main Scene 1");

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progress =
                Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.value = progress;

            yield return null;
        }

        loadingBar.value = 1f;

        yield return new WaitForSeconds(0.5f);

        operation.allowSceneActivation = true;
    }
}