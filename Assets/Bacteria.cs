using UnityEngine;

public class Bacteria : MonoBehaviour
{
    public float destructionTime = 1.0f; // Default waktu hancur
    public float level1DestructionTime = 0.5f;
    public float level2DestructionTime = 1.0f;
    public float level3DestructionTime = 2.5f;

    private BacteriaGrowth bacteriaGrowth;

    private void Start()
    {
        bacteriaGrowth = GetComponent<BacteriaGrowth>();
        UpdateDestructionTime();
    }

    public void UpdateDestructionTime()
    {
        if (bacteriaGrowth != null)
        {
            switch (bacteriaGrowth.level)
            {
                case 1:
                    destructionTime = level1DestructionTime;
                    break;
                case 2:
                    destructionTime = level2DestructionTime;
                    break;
                case 3:
                    destructionTime = level3DestructionTime;
                    break;
            }
        }
    }
}
