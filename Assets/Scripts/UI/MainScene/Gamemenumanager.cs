using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuCanvas;
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Settings Sub Panels")]
    public GameObject generalPanel;
    public GameObject graphicsPanel;
    public GameObject controlsPanel;
    public GameObject soundPanel;

    [Header("Selection Arrow")]
    public RectTransform selectionArrow;

    [Header("Arrow Speed")]
    public float arrowSpeed = 10f;

    readonly float arrowX = -689f;
    readonly float generalY = 350f;
    readonly float graphicsY = 140f;
    readonly float controlsY = -72f;
    readonly float soundY = -301f;

    float arrowTargetY;
    bool isPaused = false;

    void Start()
    {
        menuCanvas.SetActive(false);
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        arrowTargetY = generalY;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
            {
                if (settingsPanel.activeSelf)
                    OpenPausePanel();
                else if (pausePanel.activeSelf)
                    ResumeGame();
            }
        }

        if (selectionArrow != null)
        {
            Vector2 pos = selectionArrow.anchoredPosition;
            pos.x = arrowX;
            pos.y = Mathf.Lerp(pos.y, arrowTargetY, Time.unscaledDeltaTime * arrowSpeed);
            selectionArrow.anchoredPosition = pos;
        }
    }

    // ── Pause / Resume ──────────────────────────────────────────

    public void PauseGame()
    {
        menuCanvas.SetActive(true);
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        menuCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        isPaused = false;
    }

    // ── Panel Switching ─────────────────────────────────────────

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
        ShowSubPanel(generalPanel);
        arrowTargetY = generalY;
    }

    public void OpenPausePanel()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // ── Settings Tab Buttons ────────────────────────────────────

    public void OnGeneralClicked()
    {
        ShowSubPanel(generalPanel);
        arrowTargetY = generalY;
    }

    public void OnGraphicsClicked()
    {
        ShowSubPanel(graphicsPanel);
        arrowTargetY = graphicsY;
    }

    public void OnControlsClicked()
    {
        ShowSubPanel(controlsPanel);
        arrowTargetY = controlsY;
    }

    public void OnSoundClicked()
    {
        ShowSubPanel(soundPanel);
        arrowTargetY = soundY;
    }

    void ShowSubPanel(GameObject target)
    {
        if (generalPanel != null) generalPanel.SetActive(false);
        if (graphicsPanel != null) graphicsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (soundPanel != null) soundPanel.SetActive(false);
        if (target != null) target.SetActive(true);
    }

    // ── Quit ────────────────────────────────────────────────────

    public void QuitToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Intro Menu");
    }
}