using UnityEngine;

public class EnemyTrailEmitter : MonoBehaviour
{
    [Header("Dot Prefab")]
    public GameObject dotPrefab;

    [Header("Look")]
    public float dotLifeTime = 0.45f;
    [Range(0f, 1f)] public float dotStartAlpha = 0.25f;
    public float dotScale = 0.25f;
    public float shrinkTo = 0.6f;

    [Header("Spacing")]
    public float distanceSpacing = 0.30f; // smaller => more dots

    [Header("Sorting")]
    public int sortingOffset = -1; // dot behind enemy

    private Vector3 lastSpawnPos;
    private SpriteRenderer enemySR;

    private void Awake()
    {
        enemySR = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        lastSpawnPos = transform.position;
    }

    private void Update()
    {
        if (dotPrefab == null) return;

        float dist = Vector3.Distance(transform.position, lastSpawnPos);
        if (dist < distanceSpacing) return;

        SpawnDot();
        lastSpawnPos = transform.position;
    }

    private void SpawnDot()
    {
        GameObject dot = Instantiate(dotPrefab, transform.position, Quaternion.identity);
        dot.transform.localScale = new Vector3(dotScale, dotScale, 1f);

        // sorting (same layer, lower order)
        var dotSR = dot.GetComponent<SpriteRenderer>();
        if (enemySR != null && dotSR != null)
        {
            dotSR.sortingLayerID = enemySR.sortingLayerID;
            dotSR.sortingOrder = enemySR.sortingOrder + sortingOffset;
        }

        // fade setup
        var fade = dot.GetComponent<TrailDotFade>();
        if (fade != null)
            fade.Init(dotLifeTime, dotStartAlpha, shrinkTo);
    }
}