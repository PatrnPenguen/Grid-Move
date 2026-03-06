using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public SettingsPanelUI settingsUI;
    public TMP_Text scoreText;

    private void Start()
    {
        int lastScore = PlayerPrefs.GetInt("LAST_SCORE", 0);
        if (scoreText != null) scoreText.text = $"Score: {lastScore}";
    }

    public void RestartPressed()
    {
        SceneManager.LoadScene("Game");
    }

    public void MainMenuPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SettingsPressed()
    {
        if (settingsUI != null) settingsUI.Open();
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}