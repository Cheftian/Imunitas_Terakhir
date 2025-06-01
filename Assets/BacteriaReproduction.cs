using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BacteriaReproduction : MonoBehaviour
{
    public GameObject bacteriaPrefab;
    private float reproductionTime;
    public float gapReproductionTime = 5f;
    public float childScaleFactor = 0.7f;
    public float spawnRadius = 1f;
    public float minDistanceBetween = 0.5f;
    public float movementSpeed = 2f;
    public int maxChildren = 5;
    public float minScaleToReproduce = 0.3f;

    private List<GameObject> children = new List<GameObject>();
    private Vector3 lastDirection = Vector3.right;
    private BacteriaGrowth bacteriaGrowth;
    private BacteriaSpawner bacteriaSpawner; // Referensi ke spawner

    void Start()
    {
        bacteriaGrowth = GetComponent<BacteriaGrowth>();
        bacteriaSpawner = FindFirstObjectByType<BacteriaSpawner>(); // Temukan Spawner

        if (bacteriaSpawner != null)
        {
            reproductionTime = bacteriaSpawner.spawnInterval-gapReproductionTime; // Pastikan selalu sinkron
        }

        StartCoroutine(Reproduce());
    }

    void Update()
    {
        if (bacteriaSpawner != null)
        {
            reproductionTime = bacteriaSpawner.spawnInterval-gapReproductionTime; // Pastikan selalu sinkron
            if (reproductionTime < 2.0f)
            {
                reproductionTime = 2.0f;
            }
        }

        Vector3 movement = (transform.position - lastDirection).normalized;
        if (movement.magnitude > 0.01f)
        {
            lastDirection = movement;
        }

        UpdateChildrenMovement();
    }

    IEnumerator Reproduce()
    {
        if (transform.localScale.x <= minScaleToReproduce)
        {
            Debug.Log("Bakteri terlalu kecil untuk bereproduksi!");
            yield break;
        }

        while (children.Count < maxChildren)
        {
            yield return new WaitForSeconds(reproductionTime); // Gunakan reproductionTime yang diperbarui

            Vector3 spawnPosition;
            bool positionFound = false;
            int attempt = 0;

            do
            {
                Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
                spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
                positionFound = IsPositionValid(spawnPosition);
                attempt++;
            } while (!positionFound && attempt < 10);

            if (positionFound)
            {
                GameObject child = Instantiate(bacteriaPrefab, spawnPosition, Quaternion.identity);
                child.transform.localScale = transform.localScale * childScaleFactor;

                // **Jangan jadikan child dalam hirarki**
                // child.transform.SetParent(transform);  // <-- Dihapus
                
                children.Add(child);

                if (bacteriaGrowth != null)
                {
                    bacteriaGrowth.AddChild(child);
                }

                // **Pastikan BacteriaMovement tidak aktif dulu**
                BacteriaMovement movement = child.GetComponent<BacteriaMovement>();
                if (movement != null)
                {
                    movement.enabled = false;
                }
            }

            if (!positionFound)
            {
                spawnPosition = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0);
                Debug.LogWarning("Gagal menemukan posisi ideal, paksa spawn di posisi: " + spawnPosition);
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        float relaxedMinDistance = minDistanceBetween * 0.7f;

        if (Vector3.Distance(transform.position, position) < relaxedMinDistance)
        {
            return false;
        }

        foreach (GameObject child in children)
        {
            if (child != null && Vector3.Distance(child.transform.position, position) < relaxedMinDistance)
            {
                return false;
            }
        }

        return true;
    }

    void UpdateChildrenMovement()
    {
        for (int i = 0; i < children.Count; i++)
        {
            GameObject child = children[i];
            if (child != null)
            {
                Vector3 spreadOffset = new Vector3(
                    Mathf.Sin(Time.time + i) * spawnRadius * 0.5f,
                    Mathf.Cos(Time.time + i) * spawnRadius * 0.5f,
                    0
                );

                Vector3 targetPosition = transform.position - (lastDirection * (0.5f * (i + 1))) + spreadOffset;

                child.transform.position = Vector3.MoveTowards(
                    child.transform.position, targetPosition, movementSpeed * Time.deltaTime
                );
            }
        }
    }

    // **Aktifkan BacteriaMovement pada semua child ketika induk mati**
    private void OnDestroy()
    {
        foreach (GameObject child in children)
        {
            if (child != null)
            {
                BacteriaMovement movement = child.GetComponent<BacteriaMovement>();
                if (movement != null)
                {
                    movement.enabled = true;
                }
            }
        }
    }

    public void ResetChildren()
    {
        children.Clear();
        StopAllCoroutines();
        StartCoroutine(Reproduce());
    }
}
