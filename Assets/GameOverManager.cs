using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Panel Game Over
    private bool isGameOver = false;

    void Start()
    {
        gameOverPanel.SetActive(false); // Pastikan panel tidak terlihat di awal
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverPanel.SetActive(true); // Tampilkan overlay Game Over
        Time.timeScale = 0f; // Hentikan semua pergerakan
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Kembalikan kecepatan normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene
    }
}
