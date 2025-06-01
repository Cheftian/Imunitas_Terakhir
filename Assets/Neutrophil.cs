using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Neutrophil : MonoBehaviour
{
    public float moveSpeed = 0.2f;       // Kecepatan gerakan
    public float waitTime = 1f;        // Waktu berhenti setelah mencapai tujuan
    public float pulseDuration = 0.5f; // Durasi setiap "denyutan"
    public float detectionRadius = 5f; // Radius deteksi bakteri
    public float pulseSpeed = 2f;      // Kecepatan denyutan
    public float pulseAmount = 0.05f;  // Intensitas denyutan
    
    private Vector2 minBounds, maxBounds;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private Vector3 originalScale;
    private Dictionary<GameObject, Coroutine> activeAttacks = new Dictionary<GameObject, Coroutine>();

    void Start()
    {
        originalScale = transform.localScale;
        GameObject area = GameObject.Find("Area");
        if (area != null)
        {
            BoxCollider2D areaCollider = area.GetComponent<BoxCollider2D>();
            if (areaCollider != null)
            {
                Vector2 areaCenter = areaCollider.bounds.center;
                Vector2 areaSize = areaCollider.bounds.size / 2;
                minBounds = areaCenter - areaSize;
                maxBounds = areaCenter + areaSize;
            }
        }
        StartCoroutine(MoveRandomly());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bacteria") && !activeAttacks.ContainsKey(other.gameObject))
        {
            Coroutine attackCoroutine = StartCoroutine(AttackBacteria(other.gameObject));
            activeAttacks.Add(other.gameObject, attackCoroutine);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bacteria") && activeAttacks.ContainsKey(other.gameObject))
        {
            StopCoroutine(activeAttacks[other.gameObject]);
            activeAttacks.Remove(other.gameObject);
        }
    }

    private IEnumerator AttackBacteria(GameObject bacteria)
    {
        Bacteria bacteriaScript = bacteria.GetComponent<Bacteria>();
        if (bacteriaScript == null) yield break;

        float attackTime = bacteriaScript.destructionTime;
        float timer = 0f;

        while (timer < attackTime)
        {
            float scaleModifier = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = originalScale * scaleModifier;

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(bacteria);
        FindFirstObjectByType<PointSystem>().AddPoints(1);
        activeAttacks.Remove(bacteria);
    }

    private IEnumerator MoveRandomly()
    {
        while (true)
        {
            GameObject farthestBacteria = FindFarthestBacteria();
            if (farthestBacteria != null)
            {
                targetPosition = farthestBacteria.transform.position;
            }
            else if (!isMoving)
            {
                targetPosition = new Vector2(
                    Random.Range(minBounds.x, maxBounds.x),
                    Random.Range(minBounds.y, maxBounds.y)
                );
                isMoving = true;
            }

            float elapsedTime = 0f;
            Vector2 startPosition = transform.position;

            while (elapsedTime < pulseDuration)
            {
                float t = elapsedTime / pulseDuration;
                t = Mathf.SmoothStep(0, 1, t);
                transform.position = Vector2.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime * moveSpeed;
                yield return null;
            }

            transform.position = targetPosition;
            isMoving = false;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private GameObject FindFarthestBacteria()
    {
        GameObject[] bacteriaObjects = GameObject.FindGameObjectsWithTag("Bacteria");
        GameObject farthestBacteria = null;
        float maxDistance = 0f;

        foreach (GameObject bacteria in bacteriaObjects)
        {
            float distance = Vector2.Distance(transform.position, bacteria.transform.position);
            if (distance > maxDistance && distance <= detectionRadius)
            {
                maxDistance = distance;
                farthestBacteria = bacteria;
            }
        }
        return farthestBacteria;
    }
}
