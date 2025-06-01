using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PointSystem : MonoBehaviour
{
    public int points = 0; // Jumlah poin pemain
    public TMP_Text pointsText; // UI untuk menampilkan poin dengan TMP

    void Start()
    {
        UpdatePointsUI();
    }

    public void AddPoints(int amount)
    {
        points += amount;
        UpdatePointsUI();
    }

    public bool SpendPoints(int amount)
    {
        if (points >= amount)
        {
            points -= amount;
            UpdatePointsUI();
            return true;
        }
        return false;
    }

    private void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = "Poin: " + points;
        }
    }
}