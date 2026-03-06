using UnityEngine;

public class TrailDotFade : MonoBehaviour
{
    private SpriteRenderer sr;
    private float lifeTime;
    private float startAlpha;
    private float timer;

    private Vector3 startScale;

    public void Init(float dotLifeTime, float dotStartAlpha, float shrinkTo = 0.6f)
    {
        sr = GetComponent<SpriteRenderer>();
        lifeTime = Mathf.Max(0.01f, dotLifeTime);
        startAlpha = Mathf.Clamp01(dotStartAlpha);

        startScale = transform.localScale;

        // set initial alpha
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, startAlpha);

        // shrink target factor
        _shrinkTo = Mathf.Clamp(shrinkTo, 0.1f, 1f);
    }

    private float _shrinkTo = 0.6f;

    private void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifeTime);

        // fade alpha -> 0
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, Mathf.Lerp(startAlpha, 0f, t));

        // shrink a bit
        transform.localScale = Vector3.Lerp(startScale, startScale * _shrinkTo, t);

        if (t >= 1f)
            Destroy(gameObject);
    }
}