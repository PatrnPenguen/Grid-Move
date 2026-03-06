using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public SettingsPanelUI settingsUI;

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic();
    }

    public void PlayPressed()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic();

        SceneManager.LoadScene("Game");
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