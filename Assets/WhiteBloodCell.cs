using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBloodCell : MonoBehaviour
{
    public float pulseSpeed = 2f; // Kecepatan denyutan
    public float pulseAmount = 0.05f; // Intensitas denyutan

    private Vector3 originalScale;
    private Dictionary<GameObject, Coroutine> activeAttacks = new Dictionary<GameObject, Coroutine>();

    private void Start()
    {
        originalScale = transform.localScale;
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
        FindFirstObjectByType<PointSystem>().AddPoints(2); // Tambahkan 10 poin saat bakteri mati
        activeAttacks.Remove(bacteria);
    }
}
