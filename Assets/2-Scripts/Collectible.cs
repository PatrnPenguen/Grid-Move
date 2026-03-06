using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (SfxPlayer.Instance != null) SfxPlayer.Instance.PlayCollect();
        Destroy(gameObject);                 // disappear immediately
        GameManager.Instance.AddScore(1);    // score + respawn request
    }
}