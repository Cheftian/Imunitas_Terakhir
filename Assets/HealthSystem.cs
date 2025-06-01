using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100; // Kapasitas maksimum nyawa
    public int currentHealth; // Nyawa saat ini
    public int healthRegenAmount = 5; // Jumlah nyawa bertambah setiap interval
    public float regenInterval = 10f; // Waktu setiap pemulihan nyawa

    public float damageInterval = 2f; // Interval pengurangan HP (dapat diatur di Inspector)

    public Image healthBarFill; // UI Image untuk Health Bar
    private List<BacteriaGrowth> activeBacteria = new List<BacteriaGrowth>(); // Daftar bakteri aktif
    private Coroutine healthBarCoroutine; // Menyimpan coroutine animasi health bar

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBarInstant();
        StartCoroutine(HealthRegeneration());
        StartCoroutine(HealthReduction());
    }

    private void UpdateHealthBarInstant()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void UpdateHealthBarSmooth()
    {
        if (healthBarCoroutine != null)
        {
            StopCoroutine(healthBarCoroutine);
        }
        healthBarCoroutine = StartCoroutine(SmoothUpdateHealthBar());
    }

    private IEnumerator SmoothUpdateHealthBar()
    {
        float targetFill = (float)currentHealth / maxHealth;
        float startFill = healthBarFill.fillAmount;
        float duration = 0.5f; // Waktu animasi
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthBarFill.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            yield return null;
        }

        healthBarFill.fillAmount = targetFill;
    }

    private IEnumerator HealthRegeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);
            currentHealth = Mathf.Min(currentHealth + healthRegenAmount, maxHealth);
            UpdateHealthBarSmooth();
        }
    }

    private IEnumerator HealthReduction()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageInterval); // Interval pengurangan HP sesuai Inspector
            int totalDamage = 0;

            foreach (BacteriaGrowth bacteria in activeBacteria)
            {
                if (bacteria != null)
                {
                    totalDamage += bacteria.GetDamagePerSecond();
                }
            }

            currentHealth = Mathf.Max(currentHealth - totalDamage, 0);
            UpdateHealthBarSmooth();

            if (currentHealth <= 0)
            {
                GameOver();
            }
        }
    }

    public void RegisterBacteria(BacteriaGrowth bacteria)
    {
        if (!activeBacteria.Contains(bacteria))
        {
            activeBacteria.Add(bacteria);
        }
    }

    public void UnregisterBacteria(BacteriaGrowth bacteria)
    {
        if (activeBacteria.Contains(bacteria))
        {
            activeBacteria.Remove(bacteria);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Nyawa habis.");
        FindFirstObjectByType<GameOverManager>().TriggerGameOver();
    }
}
