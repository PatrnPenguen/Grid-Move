using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public SettingsPanelUI settingsUI;

    public void PlayPressed()
    {
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