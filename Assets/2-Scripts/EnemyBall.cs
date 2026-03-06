using UnityEngine;

public class EnemyBall : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    private Camera cam;
    private float viewportMargin = 0.10f;

    private bool counted = false;

    public void Init(Vector3 moveDirection, float moveSpeed, Camera camera, float margin = 0.10f)
    {
        direction = moveDirection.normalized;
        speed = moveSpeed;

        cam = camera;
        viewportMargin = margin;

        counted = true;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        transform.position += direction * speed * Time.deltaTime;

        if (cam == null) return;

        Vector3 v = cam.WorldToViewportPoint(transform.position);

        bool outside =
            v.x < -viewportMargin || v.x > 1f + viewportMargin ||
            v.y < -viewportMargin || v.y > 1f + viewportMargin;

        if (outside)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!counted) return;
        if (GameManager.Instance == null) return;

        GameManager.Instance.NotifyEnemyDestroyed();
    }
}