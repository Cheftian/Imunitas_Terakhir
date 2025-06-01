using UnityEngine;
using System.Collections;

public class BacteriaSpawner : MonoBehaviour
{
    [Header("Bacteria Prefabs")]
    public GameObject bacteriaIndukPrefab;   // Prefab bakteri Induk
    public GameObject bacteriaType1Prefab;   // Prefab bakteri Tipe 1
    public GameObject bacteriaType2Prefab;   // Prefab bakteri Tipe 2

    [Header("Spawn Settings")]
    public float spawnInterval = 10f; 
    public float minSpawnInterval = 2f;
    public float spawnReductionFactor = 0.1f;

    [Header("Spawn Probabilities (0-1)")]
    [Range(0f, 1f)] public float probabilityInduk = 0.5f;
    [Range(0f, 1f)] public float probabilityType1 = 0.3f;
    [Range(0f, 1f)] public float probabilityType2 = 0.2f;

    [Header("Spawn Count Settings")]
    public int minSpawnInduk = 1;
    public int maxSpawnInduk = 5;
    public int minSpawnOther = 1;
    public int maxSpawnOther = 1;

    [Header("Mutation Settings")]
    public int mutationTriggerCount = 5;
    public float mutationPhaseDuration = 10f;
    public float cameraShakeIntensity = 0.2f;

    private int spawnCounter = 0;
    private BoxCollider2D spawnArea;
    private Camera mainCamera;
    private bool isMutationPhase = false;

    void Start()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        mainCamera = Camera.main;

        StartCoroutine(SpawnBacteriaLoop());
    }

    IEnumerator SpawnBacteriaLoop()
    {
        while (true)
        {
            float currentInterval = isMutationPhase ? spawnInterval / 2 : spawnInterval;
            yield return new WaitForSeconds(currentInterval);
            
            SpawnBacteria();

            if (!isMutationPhase)
            {
                spawnInterval = Mathf.Max(spawnInterval - spawnReductionFactor, minSpawnInterval);
            }

            spawnCounter++;
            if (spawnCounter >= mutationTriggerCount)
            {
                StartCoroutine(StartMutationPhase());
                spawnCounter = 0;
            }
        }
    }

    IEnumerator StartMutationPhase()
    {
        Debug.Log("MUTATION PHASE STARTED!");
        isMutationPhase = true;
        StartCoroutine(CameraShake());

        // **Tambahan:** Spawn 1 bakteri tipe 1 atau 2 selama Mutation Phase
        SpawnMutationBacteria();

        yield return new WaitForSeconds(mutationPhaseDuration);

        Debug.Log("MUTATION PHASE ENDED!");
        isMutationPhase = false;
    }

    IEnumerator CameraShake()
    {
        Vector3 originalPosition = mainCamera.transform.position;
        while (isMutationPhase)
        {
            Vector3 randomShake = new Vector3(
                Random.Range(-cameraShakeIntensity, cameraShakeIntensity),
                Random.Range(-cameraShakeIntensity, cameraShakeIntensity),
                0
            );

            mainCamera.transform.position = originalPosition + randomShake;
            yield return new WaitForSeconds(0.05f);
        }
        mainCamera.transform.position = originalPosition;
    }

    void SpawnBacteria()
    {
        float randomValue = Random.value;

        if (randomValue <= probabilityInduk)
        {
            // Menyesuaikan jumlah maksimum spawn berdasarkan waktu interval
            if (spawnInterval > 8) maxSpawnInduk = 1;
            else if (spawnInterval <= 8 && spawnInterval > 7) maxSpawnInduk = 2;
            else if (spawnInterval <= 7 && spawnInterval > 5) maxSpawnInduk = 3;
            else if (spawnInterval <= 5 && spawnInterval > 3) maxSpawnInduk = 4;
            else if (spawnInterval <= 3) maxSpawnInduk = 5;
            SpawnBacteriaOfType(bacteriaIndukPrefab, minSpawnInduk, maxSpawnInduk);
        }
        else if (randomValue <= probabilityInduk + probabilityType1)
        {
            SpawnBacteriaOfType(bacteriaType1Prefab, minSpawnOther, maxSpawnOther);
        }
        else
        {
            SpawnBacteriaOfType(bacteriaType2Prefab, minSpawnOther, maxSpawnOther);
        }
    }

    void SpawnMutationBacteria()
    {
        GameObject selectedBacteria = Random.value < 0.5f ? bacteriaType1Prefab : bacteriaType2Prefab;
        SpawnBacteriaOfType(selectedBacteria, 1, 1); // Selalu spawn 1 unit
    }

    void SpawnBacteriaOfType(GameObject bacteriaPrefab, int minSpawn, int maxSpawn)
    {
        if (bacteriaPrefab == null || spawnArea == null)
        {
            Debug.LogWarning("Prefab bakteri atau area spawn tidak ditemukan!");
            return;
        }

        int spawnCount = Random.Range(minSpawn, maxSpawn + 1);
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPosition = GetRandomPositionInArea();
            Instantiate(bacteriaPrefab, randomPosition, Quaternion.identity);
        }

        Debug.Log($"Spawned {spawnCount} bacteria of type {bacteriaPrefab.name}.");
    }

    Vector2 GetRandomPositionInArea()
    {
        Bounds bounds = spawnArea.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }
}
