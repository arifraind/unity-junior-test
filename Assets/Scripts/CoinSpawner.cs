using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 5f;

    [Header("Lifetime")]
    [SerializeField] private float coinLifetime = 5f;

    [Header("Optional Random X Offset")]
    [SerializeField] private bool useRandomXOffset = false;
    [SerializeField] private float minXOffset = -1f;
    [SerializeField] private float maxXOffset = 1f;

    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnCoin();
            timer = spawnInterval;
        }
    }

    private void SpawnCoin()
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning("CoinSpawner: coinPrefab is missing.");
            return;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;

        if (useRandomXOffset)
        {
            float randomX = Random.Range(minXOffset, maxXOffset);
            spawnPosition.x += randomX;
        }

        GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);

        // 🔥 Destroy after X seconds
        Destroy(coin, coinLifetime);
    }
}