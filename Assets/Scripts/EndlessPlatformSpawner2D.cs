using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndlessPlatformSpawner2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private Transform player;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Platform Prefabs")]
    [SerializeField] private GameObject[] normalPlatformPrefabs;
    [SerializeField] private GameObject[] disappearingPlatformPrefabs;

    [Header("Spawn Chances")]
    [Range(0f, 1f)]
    [SerializeField] private float disappearingChance = 0.25f;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -2.2f;
    [SerializeField] private float maxX = 2.2f;

    [Header("Vertical Generation")]
    [SerializeField] private float startY = -2f;
    [SerializeField] private float initialSpawnHeight = 12f;
    [SerializeField] private float spawnAheadDistance = 10f;
    [SerializeField] private float minYGap = 1.2f;
    [SerializeField] private float maxYGap = 2.2f;

    [Header("Cleanup")]
    [SerializeField] private float destroyBelowPlayerDistance = 8f;

    [Header("Feel")]
    [SerializeField] private float minXDistanceFromPrevious = 0.8f;

    private readonly List<GameObject> spawnedPlatforms = new List<GameObject>();
    private readonly HashSet<GameObject> scoredPlatforms = new HashSet<GameObject>();

    private float highestSpawnedY;
    private float highestReachedY;
    private float lastSpawnX;

    private int score = 0;
    private int highScore = 0;

    private const string HighScoreKey = "HighScore";

    public int CurrentScore => score;
    public int HighScore => highScore;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned.");
            enabled = false;
            return;
        }

        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        highestSpawnedY = startY;
        highestReachedY = player.position.y;

        UpdateScoreUI();
        UpdateHighScoreUI();
        SpawnInitialPlatforms();
    }

    private void Update()
    {
        UpdateHighestReachedY();
        SpawnAheadOfProgress();
        CleanupOldPlatforms();
        UpdateScoreFromPassedPlatforms();
    }

    private void UpdateHighestReachedY()
    {
        if (player.position.y > highestReachedY)
            highestReachedY = player.position.y;
    }

    private void SpawnInitialPlatforms()
    {
        float targetY = highestReachedY + initialSpawnHeight;

        while (highestSpawnedY < targetY)
            SpawnNextPlatform();
    }

    private void SpawnAheadOfProgress()
    {
        float targetY = highestReachedY + spawnAheadDistance;

        while (highestSpawnedY < targetY)
            SpawnNextPlatform();
    }

    private void SpawnNextPlatform()
    {
        highestSpawnedY += Random.Range(minYGap, maxYGap);

        float x = GetNextXPosition();
        GameObject prefab = GetRandomPlatformPrefab();

        if (prefab == null)
            return;

        GameObject platform = Instantiate(prefab, new Vector3(x, highestSpawnedY, 0f), Quaternion.identity);
        spawnedPlatforms.Add(platform);
        lastSpawnX = x;
    }

    private float GetNextXPosition()
    {
        float x = Random.Range(minX, maxX);

        int tries = 0;
        while (Mathf.Abs(x - lastSpawnX) < minXDistanceFromPrevious && tries < 10)
        {
            x = Random.Range(minX, maxX);
            tries++;
        }

        return x;
    }

    private GameObject GetRandomPlatformPrefab()
    {
        bool spawnDisappearing =
            disappearingPlatformPrefabs != null &&
            disappearingPlatformPrefabs.Length > 0 &&
            Random.value < disappearingChance;

        if (spawnDisappearing)
        {
            return disappearingPlatformPrefabs[
                Random.Range(0, disappearingPlatformPrefabs.Length)
            ];
        }

        if (normalPlatformPrefabs != null && normalPlatformPrefabs.Length > 0)
        {
            return normalPlatformPrefabs[
                Random.Range(0, normalPlatformPrefabs.Length)
            ];
        }

        return null;
    }

    private void CleanupOldPlatforms()
    {
        float minY = player.position.y - destroyBelowPlayerDistance;

        for (int i = spawnedPlatforms.Count - 1; i >= 0; i--)
        {
            if (spawnedPlatforms[i] == null)
            {
                spawnedPlatforms.RemoveAt(i);
                continue;
            }

            if (spawnedPlatforms[i].transform.position.y < minY)
            {
                scoredPlatforms.Remove(spawnedPlatforms[i]);
                Destroy(spawnedPlatforms[i]);
                spawnedPlatforms.RemoveAt(i);
            }
        }
    }

    private void UpdateScoreFromPassedPlatforms()
    {
        for (int i = 0; i < spawnedPlatforms.Count; i++)
        {
            GameObject platform = spawnedPlatforms[i];

            if (platform == null)
                continue;

            if (scoredPlatforms.Contains(platform))
                continue;

            if (player.position.y > platform.transform.position.y)
            {
                scoredPlatforms.Add(platform);
                score++;

                if (score > highScore)
                {
                    highScore = score;
                    PlayerPrefs.SetInt(HighScoreKey, highScore);
                    PlayerPrefs.Save();
                }

                UpdateScoreUI();
                UpdateHighScoreUI();
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "" + score;

        if (gameOverScoreText != null)
            gameOverScoreText.text = score.ToString();
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
            highScoreText.text = "" + highScore;
    }

    public void RefreshScoreUI()
    {
        UpdateScoreUI();
        UpdateHighScoreUI();
    }

    public void ResetRunScore()
    {
        score = 0;
        UpdateScoreUI();
        UpdateHighScoreUI();
    }
}