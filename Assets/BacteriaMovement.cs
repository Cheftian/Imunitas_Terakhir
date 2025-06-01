using UnityEngine;
using System.Collections;

public class BacteriaMovement : MonoBehaviour
{
    public float moveSpeed = 2f;       // Kecepatan dasar gerakan
    public float waitTime = 1f;        // Waktu berhenti setelah mencapai tujuan
    public float pulseDuration = 0.5f; // Durasi setiap "denyutan"
    private Vector2 minBounds, maxBounds;
    private Vector2 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        // Mencari batas area dari collider GameObject "Area"
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

    IEnumerator MoveRandomly()
    {
        while (true)
        {
            if (!isMoving)
            {
                // Menentukan tujuan acak di dalam area
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
                // Gunakan Lerp dengan easing untuk menciptakan efek terseret-seret
                float t = elapsedTime / pulseDuration;
                t = Mathf.SmoothStep(0, 1, t); // Efek slime yang smooth

                transform.position = Vector2.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime * moveSpeed;
                yield return null;
            }

            transform.position = targetPosition;

            // Sampai di tujuan, berhenti sejenak
            isMoving = false;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
