using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour
{
    public HealthSystem healthSystem;
    public DragMovement playerMovement;
    public PointSystem pointSystem;
    public Button healthButton, speedButton, regenButton, sizeButton, summonButton;
    public GameObject neutrophilPrefab;

    public int healthBoostCost = 50;
    public int speedBoostCost = 50;
    public int regenBoostCost = 75;
    public int sizeBoostCost = 100;
    public int summonNeutrophilCost = 150;

    public int maxHealthUses = 5;
    public int maxSpeedUses = 5;
    public int maxRegenUses = 5;
    public int maxSizeUses = 3;
    public int maxSummonUses = 10;

    private Dictionary<Button, (int cost, int maxUses, int currentUses)> buttonData;

    private void Start()
    {
        buttonData = new Dictionary<Button, (int, int, int)>
        {
            { healthButton, (healthBoostCost, maxHealthUses, 0) },
            { speedButton, (speedBoostCost, maxSpeedUses, 0) },
            { regenButton, (regenBoostCost, maxRegenUses, 0) },
            { sizeButton, (sizeBoostCost, maxSizeUses, 0) },
            { summonButton, (summonNeutrophilCost, maxSummonUses, 0) }
        };

        UpdateAllButtons();
    }

    private void Update()
    {
        UpdateAllButtons();
    }

    private void UpdateAllButtons()
    {
        foreach (var buttonEntry in buttonData)
        {
            UpdateButtonState(buttonEntry.Key, buttonEntry.Value.cost, buttonEntry.Value.maxUses, buttonEntry.Value.currentUses);
        }
    }

    private void UpdateButtonState(Button button, int cost, int maxUses, int currentUses)
    {
        if (button != null)
        {
            bool canAfford = pointSystem.points >= cost;
            bool canUse = currentUses < maxUses;
            button.interactable = canAfford && canUse;

            ColorBlock colors = button.colors;
            colors.normalColor = canAfford && canUse ? Color.white : Color.gray;
            button.colors = colors;
        }
    }

    private void ApplyPowerUp(Button button, System.Action powerUpEffect)
    {
        if (buttonData.ContainsKey(button) && pointSystem.SpendPoints(buttonData[button].cost))
        {
            powerUpEffect();

            int newCost = button == healthButton ? buttonData[button].cost : buttonData[button].cost * 2; // Gandakan biaya kecuali HealthBoost
            buttonData[button] = (newCost, buttonData[button].maxUses, buttonData[button].currentUses + 1);

            UpdateAllButtons();
        }
    }

    public void BuyHealthBoost()
    {
        ApplyPowerUp(healthButton, () =>
        {
            healthSystem.currentHealth += 1000;
            healthSystem.UpdateHealthBarSmooth();
        });
    }

    public void BuySpeedBoost()
    {
        ApplyPowerUp(speedButton, () =>
        {
            playerMovement.dragSpeed += 0.5f;
        });
    }

    public void BuyRegenBoost()
    {
        ApplyPowerUp(regenButton, () =>
        {
            healthSystem.healthRegenAmount += 500;
        });
    }

    public void BuySizeBoost()
    {
        ApplyPowerUp(sizeButton, () =>
        {
            transform.localScale *= 2f;
        });
    }

    public void BuySummonNeutrophil()
    {
        ApplyPowerUp(summonButton, () =>
        {
            Instantiate(neutrophilPrefab, transform.position, Quaternion.identity);
        });
    }
}
