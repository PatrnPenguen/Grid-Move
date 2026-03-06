using System.Collections;
using UnityEngine;

public class PlayerGridMover : MonoBehaviour
{
    private Vector2Int gridPos = Vector2Int.zero;

    private const int Min = -1;
    private const int Max = 1;

    [SerializeField] private float moveDuration = 0.25f; // bigger = slower

    private bool isMoving = false;

    // Input buffer: while moving, remember the next direction
    private bool hasBufferedInput = false;
    private Vector2Int bufferedDir = Vector2Int.zero;

    private void Start()
    {
        transform.position = new Vector3(gridPos.x, gridPos.y, 0f);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;
        Vector2Int dir = ReadDirectionDown();
        if (dir == Vector2Int.zero) return;

        if (!isMoving)
        {
            TryMove(dir);
        }
        else
        {
            // Buffer the latest input while moving (overwrites previous buffer)
            bufferedDir = dir;
            hasBufferedInput = true;
        }
    }

    private Vector2Int ReadDirectionDown()
    {
        // else-if so only one direction is taken per frame
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) return Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) return Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) return Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) return Vector2Int.right;

        return Vector2Int.zero;
    }

    private void TryMove(Vector2Int dir)
    {
        Vector2Int next = gridPos + dir;
        next.x = Mathf.Clamp(next.x, Min, Max);
        next.y = Mathf.Clamp(next.y, Min, Max);

        if (next != gridPos)
            StartCoroutine(MoveTo(next));
    }

    private IEnumerator MoveTo(Vector2Int targetGridPos)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(targetGridPos.x, targetGridPos.y, 0f);

        float rawT = 0f;
        while (rawT < 1f)
        {
            rawT += Time.deltaTime / moveDuration;

            // Smooth easing: starts slow -> speeds up -> slows down
            float t = Mathf.SmoothStep(0f, 1f, rawT);

            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
        gridPos = targetGridPos;

        isMoving = false;

        // If we buffered an input while moving, apply it immediately after finishing
        if (hasBufferedInput)
        {
            hasBufferedInput = false;
            Vector2Int dir = bufferedDir;
            bufferedDir = Vector2Int.zero;

            TryMove(dir);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.GameOver(transform.position);
        }
    }
}