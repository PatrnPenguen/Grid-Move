using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public SettingsPanelUI settingsUI;
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;

    private const string BestScoreKey = "BEST_SCORE";

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();
        
        int lastScore = PlayerPrefs.GetInt("LAST_SCORE", 0);
        int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        if (scoreText != null)
            scoreText.text = $"Score: {lastScore}";

        if (bestScoreText != null)
            bestScoreText.text = $"Best: {bestScore}";
    }

    public void RestartPressed()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic();
        
        SceneManager.LoadScene("Game");
    }

    public void MainMenuPressed()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic();
        
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