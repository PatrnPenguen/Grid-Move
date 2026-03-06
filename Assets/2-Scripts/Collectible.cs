using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.blue;
    [SerializeField] private Color preLevelUpColor = Color.yellow;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetPreLevelUpState(bool isPreLevelUp)
    {
        if (sr == null) return;

        sr.color = isPreLevelUp ? preLevelUpColor : normalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (SfxPlayer.Instance != null) SfxPlayer.Instance.PlayCollect();
        Destroy(gameObject);
        GameManager.Instance.AddScore(1);
    }
}