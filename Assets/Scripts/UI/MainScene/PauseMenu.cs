using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenu;
    public GameObject settingsPanel;

    private bool isPaused = false;

    private void Start()
    {
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If settings is open, go back to pause menu
            if (settingsPanel.activeSelf)
            {
                CloseSettings();
                return;
            }

            if (pauseMenu.activeSelf)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        settingsPanel.SetActive(false);
        pauseMenu.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenSettings()
    {
        // DO NOT disable pauseMenu
        // this was causing your weird issues

        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Intro Menu");
    }
}