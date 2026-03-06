using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 startLocalPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    public void Shake(float duration, float strength)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float x = Random.Range(-strength, strength);
            float y = Random.Range(-strength, strength);

            transform.localPosition = startLocalPos + new Vector3(x, y, 0f);

            yield return null;
        }

        transform.localPosition = startLocalPos;
    }
}