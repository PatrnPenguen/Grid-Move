using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Prefabs")]
    public GameObject collectiblePrefab;
    public GameObject enemyPrefab;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;
    public TMP_Text gameOverText;
    
    [Header("Level Popup UI")]
    public TMP_Text levelPopupText;
    public CanvasGroup levelPopupGroup;
    public float levelPopupHold = 0.6f;  // visible time
    public float levelPopupFade = 0.5f;  // fade-out duration

    private Coroutine levelPopupRoutine;

    [Header("References")]
    public Transform playerTransform;

    [Header("Collectible Settings")]
    public float respawnDelay = 0.5f;

    [Header("Enemy Difficulty")]
    public float baseEnemySpeed = 1.6f;       // level 1 speed
    public float speedPerLevel = 0.35f;       // + per level
    public float baseSpawnInterval = 1.2f;    // level 1 spawn time
    public float spawnFasterPerLevel = 0.10f; // decrease per level
    public float minSpawnInterval = 0.35f;

    private int score = 0;
    private int level = 1;
    private bool isGameOver = false;

    // 3x3 grid cells
    private Vector2Int[] cells =
    {
        new Vector2Int(-1,-1), new Vector2Int(0,-1), new Vector2Int(1,-1),
        new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0),
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1)
    };

    private GameObject currentCollectible;
    private Coroutine collectibleRoutine;
    private Coroutine enemySpawnRoutine;
    
    private int activeEnemyCount = 0;
    private string lastLaneKey = ""; // for "no back-to-back lane" rule

    [Header("Enemy Limit")]
    public int baseMaxEnemies = 2;      // Level 1 limit
    public int increaseEveryLevels = 2; // e.g. 2 => every 2 levels
    public int increaseAmount = 1;      // e.g. 2 => +2 each step
    public int maxIncreaseSteps = 5;    // increase at most 5 times

    [Header("Lane Rules")]
    public int allowSameLaneStartLevel = 3; // level 3+ : allow same lane consecutively
    
    [Header("Game Over FX")]
    public GameObject explosionPrefab;
    public float gameOverTextDelay = 2.0f; // seconds
    
    [Header("Camera Shake")]
    public CameraShake cameraShake;
    public float shakeDuration = 0.18f;
    public float shakeStrength = 0.12f;
    
    private Coroutine gameOverRoutine;
    
    private const string BestScoreKey = "BEST_SCORE";
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        UpdateUI();
        ShowLevelPopup(level);

        RequestCollectibleSpawn(0f);
        enemySpawnRoutine = StartCoroutine(EnemySpawnLoop());
    }

    public bool IsGameOver() => isGameOver;

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;

        int currentBest = PlayerPrefs.GetInt(BestScoreKey, 0);
        if (score > currentBest)
        {
            PlayerPrefs.SetInt(BestScoreKey, score);
        }

        int newLevel = (score / 10) + 1;

        if (newLevel != level)
        {
            level = newLevel;
            ShowLevelPopup(level);
        }

        UpdateUI();
        RequestCollectibleSpawn(respawnDelay);
    }
    
    private void ShowLevelPopup(int levelNumber)
    {
        if (levelPopupText == null || levelPopupGroup == null) return;

        if (levelPopupRoutine != null)
            StopCoroutine(levelPopupRoutine);

        levelPopupRoutine = StartCoroutine(LevelPopupRoutine(levelNumber));
    }

    private IEnumerator LevelPopupRoutine(int levelNumber)
    {
        levelPopupText.text = $"LEVEL {levelNumber}";
        levelPopupGroup.alpha = 1f;

        // stay visible
        yield return new WaitForSeconds(levelPopupHold);

        // fade out
        float t = 0f;
        float fadeTime = Mathf.Max(0.01f, levelPopupFade);

        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            levelPopupGroup.alpha = 1f - t;
            yield return null;
        }

        levelPopupGroup.alpha = 0f;
    }

    public void GameOver(Vector3 hitWorldPos)
    {
        if (isGameOver) return;

        isGameOver = true;

        // Stop spawning immediately
        if (enemySpawnRoutine != null) StopCoroutine(enemySpawnRoutine);

        // Stop background music immediately
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();

        // Optional: hide player so explosion is clearer
        if (playerTransform != null)
        {
            var sr = playerTransform.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            var col = playerTransform.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    
        if (cameraShake != null)
            cameraShake.Shake(shakeDuration, shakeStrength);
    
        if (SfxPlayer.Instance != null) SfxPlayer.Instance.PlayHit();
    
        // Spawn explosion
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, hitWorldPos, Quaternion.identity);
    
        // Save score and go to GameOver scene after delay
        PlayerPrefs.SetInt("LAST_SCORE", score);
        StartCoroutine(LoadGameOverAfterDelay());
    }
    
    private IEnumerator LoadGameOverAfterDelay()
    {
        yield return new WaitForSeconds(gameOverTextDelay); // senin delay değişkenin
        SceneManager.LoadScene("GameOver");
    }

    private IEnumerator GameOverTextSequence()
    {
        // make sure it's hidden at first
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        yield return new WaitForSeconds(gameOverTextDelay);

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"{score}";

        int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        if (bestScoreText != null)
            bestScoreText.text = $"Best: {bestScore}";
    }

    // ---------- Collectible spawn (with forbidden area) ----------
    private void RequestCollectibleSpawn(float delay)
    {
        if (collectibleRoutine != null)
            StopCoroutine(collectibleRoutine);

        collectibleRoutine = StartCoroutine(SpawnCollectibleAfterDelay(delay));
    }

    private IEnumerator SpawnCollectibleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isGameOver) yield break;

        if (currentCollectible != null)
            Destroy(currentCollectible);

        Vector2Int playerCell = GetPlayerCell();

        // Allowed cells: NOT player's cell and NOT 4-neighbors
        List<Vector2Int> allowed = new List<Vector2Int>();
        foreach (var cell in cells)
        {
            int dx = Mathf.Abs(cell.x - playerCell.x);
            int dy = Mathf.Abs(cell.y - playerCell.y);

            // Manhattan distance <= 1 => same OR up/down/left/right
            if (dx + dy <= 1) continue;

            allowed.Add(cell);
        }

        Vector2Int chosen = allowed[Random.Range(0, allowed.Count)];
        currentCollectible = Instantiate(collectiblePrefab, new Vector3(chosen.x, chosen.y, 0f), Quaternion.identity);

        bool isPreLevelUpCollectible = (score % 10 == 9);

        Collectible collectible = currentCollectible.GetComponent<Collectible>();
        if (collectible != null)
        {
            collectible.SetPreLevelUpState(isPreLevelUpCollectible);
        }
    }

    private Vector2Int GetPlayerCell()
    {
        Vector3 p = playerTransform.position;
        return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
    }

    // ---------- Enemy spawning ----------
    private IEnumerator EnemySpawnLoop()
    {
        while (!isGameOver)
        {
            float interval = GetCurrentSpawnInterval();
            yield return new WaitForSeconds(interval);

            if (!isGameOver)
                SpawnEnemy();
        }
    }

    private float GetCurrentSpawnInterval()
    {
        float interval = baseSpawnInterval - (level - 1) * spawnFasterPerLevel;
        return Mathf.Clamp(interval, minSpawnInterval, baseSpawnInterval);
    }

    private float GetCurrentEnemySpeed()
    {
        return baseEnemySpeed + (level - 1) * speedPerLevel;
    }

    private void SpawnEnemy()
{
    // LIMIT CHECK
    int maxEnemies = GetMaxEnemiesForLevel();
    if (activeEnemyCount >= maxEnemies) return;

    Camera cam = Camera.main;
    if (cam == null) return;

    float zDist = Mathf.Abs(cam.transform.position.z);
    float margin = 0.10f;

    float leftX   = cam.ViewportToWorldPoint(new Vector3(-margin, 0.5f, zDist)).x;
    float rightX  = cam.ViewportToWorldPoint(new Vector3(1f + margin, 0.5f, zDist)).x;
    float bottomY = cam.ViewportToWorldPoint(new Vector3(0.5f, -margin, zDist)).y;
    float topY    = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f + margin, zDist)).y;

    int[] lanes = { -1, 0, 1 };

    // Avoid spawning same lane consecutively
    bool avoidSameLane = level < allowSameLaneStartLevel;
    
    Vector3 spawnPos = Vector3.zero;
    Vector3 dir = Vector3.right;
    string laneKey = "";

    int tries = 0;
    do
    {
        tries++;

        bool horizontal = Random.value < 0.5f;

        if (horizontal)
        {
            int laneY = lanes[Random.Range(0, lanes.Length)];
            bool leftToRight = Random.value < 0.5f;

            spawnPos = new Vector3(leftToRight ? leftX : rightX, laneY, 0f);
            dir = leftToRight ? Vector3.right : Vector3.left;

            laneKey = "H:" + laneY; // same y means same horizontal lane
        }
        else
        {
            int laneX = lanes[Random.Range(0, lanes.Length)];
            bool bottomToTop = Random.value < 0.5f;

            spawnPos = new Vector3(laneX, bottomToTop ? bottomY : topY, 0f);
            dir = bottomToTop ? Vector3.up : Vector3.down;

            laneKey = "V:" + laneX; // same x means same vertical lane
        }

        // Try a few times to avoid same lane (Level 3+)
        if (!avoidSameLane) break;
        if (laneKey != lastLaneKey) break;

    } while (tries < 20);

    lastLaneKey = laneKey;

    GameObject e = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

    float speed = GetCurrentEnemySpeed();
    e.GetComponent<EnemyBall>().Init(dir, speed, cam, margin);

    activeEnemyCount++;
}
    
    public void NotifyEnemyDestroyed()
    {
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }

    private int GetMaxEnemiesForLevel()
    {
        int every = Mathf.Max(1, increaseEveryLevels); // safety: no divide by 0

        // How many "increase steps" have we reached?
        // Level 1 => 0, Level (1+every) => 1, etc.
        int steps = (level - 1) / every;
        steps = Mathf.Clamp(steps, 0, maxIncreaseSteps);

        return baseMaxEnemies + steps * increaseAmount;
    }
    
}