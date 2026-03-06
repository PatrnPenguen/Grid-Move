using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources (2 AudioSources on this object)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Defaults")]
    [Range(0f, 1f)] public float defaultMusicVolume = 0.7f;
    [Range(0f, 1f)] public float defaultSfxVolume = 0.7f;

    private const string MusicKey = "MUSIC_VOL";
    private const string SfxKey = "SFX_VOL";

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Auto-pick 2 AudioSources if not assigned in Inspector
        if (musicSource == null || sfxSource == null)
        {
            var sources = GetComponents<AudioSource>();
            if (sources.Length >= 2)
            {
                if (musicSource == null) musicSource = sources[0];
                if (sfxSource == null) sfxSource = sources[1];
            }
        }

        // If PlayerPrefs is empty, set defaults (70%)
        if (!PlayerPrefs.HasKey(MusicKey)) PlayerPrefs.SetFloat(MusicKey, defaultMusicVolume);
        if (!PlayerPrefs.HasKey(SfxKey)) PlayerPrefs.SetFloat(SfxKey, defaultSfxVolume);

        // Apply
        SetMusicVolume(PlayerPrefs.GetFloat(MusicKey));
        SetSfxVolume(PlayerPrefs.GetFloat(SfxKey));

        // Ensure music plays
        if (musicSource != null)
        {
            musicSource.spatialBlend = 0f; // 2D safety
            if (musicSource.clip != null && !musicSource.isPlaying && musicSource.volume > 0.001f)
                musicSource.Play();
        }

        if (sfxSource != null)
            sfxSource.spatialBlend = 0f;

        Debug.Log($"[AudioManager] Init -> MusicVol={GetMusicVolume():0.00} SfxVol={GetSfxVolume():0.00}");
    }

    public float GetMusicVolume() => musicSource ? musicSource.volume : 0f;
    public float GetSfxVolume() => sfxSource ? sfxSource.volume : 0f;

    public void SetMusicVolume(float v)
    {
        if (!musicSource) return;

        musicSource.volume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(MusicKey, musicSource.volume);

        // If user turns volume up and music isn't playing, start it
        if (musicSource.volume > 0.001f && musicSource.clip != null && !musicSource.isPlaying)
            musicSource.Play();
    }

    public void SetSfxVolume(float v)
    {
        if (!sfxSource) return;

        sfxSource.volume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(SfxKey, sfxSource.volume);
    }

    // Optional: call once for debugging
    public void ResetToDefaults()
    {
        PlayerPrefs.SetFloat(MusicKey, defaultMusicVolume);
        PlayerPrefs.SetFloat(SfxKey, defaultSfxVolume);
        SetMusicVolume(defaultMusicVolume);
        SetSfxVolume(defaultSfxVolume);
        Debug.Log($"[AudioManager] Reset -> MusicVol={GetMusicVolume():0.00} SfxVol={GetSfxVolume():0.00}");
    }
}