using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip collectClip;
    public AudioClip hitClip;
    public AudioClip uiClickClip;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        if (sfxSource == null && AudioManager.Instance != null)
            sfxSource = AudioManager.Instance.sfxSource;
    }

    public void PlayCollect() { if (collectClip) sfxSource.PlayOneShot(collectClip); }
    public void PlayHit()     { if (hitClip)     sfxSource.PlayOneShot(hitClip); }
    public void PlayClick()   { if (uiClickClip) sfxSource.PlayOneShot(uiClickClip); }
}