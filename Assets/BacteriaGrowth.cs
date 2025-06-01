using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BacteriaGrowth : MonoBehaviour
{
    public int level = 1;  
    public int requiredChildren = 5; 
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.1f;
    public Color level2Color = Color.yellow;
    public Color level3Color = Color.red;
    public int damage1 = 1;
    public int damage2 = 5;
    public int damage3 = 10;

    private List<GameObject> children = new List<GameObject>(); 
    private bool isLevelingUp = false; 

    void Update()
    {
        if (!isLevelingUp && CanLevelUp())
        {
            StartCoroutine(LevelUp());
        }
    }

    public void AddChild(GameObject child)
    {
        children.Add(child);
    }

    private bool CanLevelUp()
    {
        if (level == 1)
        {
            return children.Count >= requiredChildren && AllChildrenAreLevel(1);
        }
        else if (level == 2)
        {
            return children.Count >= requiredChildren && AllChildrenAreLevel(2);
        }
        return false;
    }

    private bool AllChildrenAreLevel(int requiredLevel)
    {
        int count = 0;
        foreach (GameObject child in children)
        {
            if (child != null)
            {
                BacteriaGrowth childGrowth = child.GetComponent<BacteriaGrowth>();
                if (childGrowth != null && childGrowth.level == requiredLevel)
                {
                    count++;
                }
            }
        }
        return count >= requiredChildren;
    }

    IEnumerator LevelUp()
    {
        isLevelingUp = true;

        CameraShake.Instance.ShakeCamera(shakeDuration, shakeIntensity);
        yield return new WaitForSeconds(0.5f);

        // Hapus anak-anak
        int removed = 0;
        for (int i = children.Count - 1; i >= 0; i--)
        {
            if (children[i] != null)
            {
                BacteriaGrowth childGrowth = children[i].GetComponent<BacteriaGrowth>();
                if (childGrowth != null && ((level == 1 && childGrowth.level == 1) || (level == 2 && childGrowth.level == 2)))
                {
                    Destroy(children[i]);
                    children.RemoveAt(i);
                    removed++;
                }
            }

            if (removed >= requiredChildren) break;
        }

        if (level < 3)
        {
            level++;
            transform.localScale *= 1.2f; 

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (level == 2) sr.color = level2Color;
            else if (level == 3) sr.color = level3Color;
        }

        // Perbarui durasi hancur di Bacteria.cs
        Bacteria bacteriaScript = GetComponent<Bacteria>();
        if (bacteriaScript != null)
        {
            bacteriaScript.UpdateDestructionTime();
        }

        isLevelingUp = false;
    }

    public int GetDamagePerSecond()
    {
        if (level == 1) return damage1;
        if (level == 2) return damage2;
        if (level == 3) return damage3;
        return 0;
    }

    private void OnEnable()
    {
        HealthSystem healthSystem = FindFirstObjectByType<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.RegisterBacteria(this);
        }
    }

    private void OnDisable()
    {
        HealthSystem healthSystem = FindFirstObjectByType<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.UnregisterBacteria(this);
        }
    }

}
