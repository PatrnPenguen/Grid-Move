using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelUI : MonoBehaviour
{
    public GameObject panelRoot;

    public Slider musicSlider;
    public Slider sfxSlider;

    public TMP_Text musicValueText;
    public TMP_Text sfxValueText;

    private void Awake()
    {
        // Make sure slider ranges are correct
        if (musicSlider != null) { musicSlider.minValue = 0f; musicSlider.maxValue = 1f; musicSlider.wholeNumbers = false; }
        if (sfxSlider != null)   { sfxSlider.minValue = 0f;   sfxSlider.maxValue = 1f;   sfxSlider.wholeNumbers = false; }
    }

    private void Start()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        StartCoroutine(InitWhenAudioReady());
    }

    private IEnumerator InitWhenAudioReady()
    {
        // Wait until AudioManager exists (important if scene load order differs)
        while (AudioManager.Instance == null)
            yield return null;

        // Remove old listeners (prevents double-calls)
        if (musicSlider != null) musicSlider.onValueChanged.RemoveAllListeners();
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveAllListeners();

        // Set slider values WITHOUT triggering callbacks
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSfxVolume());

        // Add listeners
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxChanged);

        RefreshValueTexts();
    }

    private void Update()
    {
        if (panelRoot != null && panelRoot.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Open()
    {
        if (panelRoot != null) panelRoot.SetActive(true);

        if (AudioManager.Instance != null)
        {
            if (musicSlider != null) musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSfxVolume());
        }

        RefreshValueTexts();
    }

    public void Close()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    private void OnMusicChanged(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(v);
        RefreshValueTexts();
    }

    private void OnSfxChanged(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetSfxVolume(v);
        RefreshValueTexts();
    }

    private void RefreshValueTexts()
    {
        if (musicValueText != null && musicSlider != null)
            musicValueText.text = ToPercent(musicSlider.value);

        if (sfxValueText != null && sfxSlider != null)
            sfxValueText.text = ToPercent(sfxSlider.value);
    }

    private string ToPercent(float v)
    {
        int p = Mathf.RoundToInt(Mathf.Clamp01(v) * 100f);
        return p + "%";
    }
}