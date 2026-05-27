using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public GameObject settingsPanel;

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