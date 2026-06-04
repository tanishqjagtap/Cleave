using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public GameObject settingsPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("Main Scene 1");
    }

    public void OpenSettings()
    {
        loadingScreen.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        loadingScreen.SetActive(true);
    }
}