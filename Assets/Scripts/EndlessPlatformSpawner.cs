using System.Collections.Generic;
using UnityEngine;

public class EndlessPlatformSpawner : MonoBehaviour
{
    [Header("Platform Prefabs")]
    [SerializeField] private List<GameObject> platformPrefabs = new List<GameObject>();

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnX = 10f;
    [SerializeField] private Vector2 spawnYRange = new Vector2(-2.5f, 2.5f);
    [SerializeField] private float spawnZ = 0f;

    [Header("Movement")]
    [SerializeField] private float moveLeftSpeed = 2f;

    [Header("Cleanup")]
    [SerializeField] private float destroyX = -10f;

    private float spawnTimer;
    private readonly List<Transform> spawnedPlatforms = new List<Transform>();

    private void Update()
    {
        HandleSpawning();
        MovePlatformsLeft();
        CleanupPlatforms();
    }

    private void HandleSpawning()
    {
        if (platformPrefabs == null || platformPrefabs.Count == 0)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnPlatform();
        }
    }

    private void SpawnPlatform()
    {
        int randomIndex = Random.Range(0, platformPrefabs.Count);
        GameObject prefab = platformPrefabs[randomIndex];

        float randomY = Random.Range(spawnYRange.x, spawnYRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, randomY, spawnZ);

        GameObject newPlatform = Instantiate(prefab, spawnPosition, Quaternion.identity);
        spawnedPlatforms.Add(newPlatform.transform);
    }

    private void MovePlatformsLeft()
    {
        for (int i = 0; i < spawnedPlatforms.Count; i++)
        {
            if (spawnedPlatforms[i] == null)
                continue;

            spawnedPlatforms[i].position += Vector3.left * moveLeftSpeed * Time.deltaTime;
        }
    }

    private void CleanupPlatforms()
    {
        for (int i = spawnedPlatforms.Count - 1; i >= 0; i--)
        {
            if (spawnedPlatforms[i] == null)
            {
                spawnedPlatforms.RemoveAt(i);
                continue;
            }

            if (spawnedPlatforms[i].position.x <= destroyX)
            {
                Destroy(spawnedPlatforms[i].gameObject);
                spawnedPlatforms.RemoveAt(i);
            }
        }
    }
}