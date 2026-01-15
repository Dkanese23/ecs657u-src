using UnityEngine;

public class CircularMazeGenerator : MonoBehaviour
{
    [Header("Testing")]
    public Difficulty debugDifficulty = Difficulty.Hard; // Use this to test in-scene

    [Header("Prefabs")]
    public GameObject wallPrefab;

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        // 1. Determine which difficulty to use
        Difficulty currentDiff;

        if (DifficultyManager.Instance != null)
        {
            // Use the real choice from Main Menu
            currentDiff = DifficultyManager.Instance.currentDifficulty;
        }
        else
        {
            // Fallback so I can still test in this scene
            currentDiff = debugDifficulty;
        }

        // 2. Set specific values for Easy, Normal, and Hard
        int rings = 5;
        int segmentsPerRing = 24;
        float ringSpacing = 5f;

        switch (currentDiff)
        {
            case Difficulty.Easy:
                rings = 4; 
                segmentsPerRing = 24;
                ringSpacing = 6f;
                break;
            case Difficulty.Normal:
                rings = 6;
                segmentsPerRing = 27;
                ringSpacing = 5.5f;
                break;
            case Difficulty.Hard:
                rings = 8;
                segmentsPerRing = 30;
                ringSpacing = 5f;
                break;
        }

        // 3. The Generation Loop
        for (int r = 1; r <= rings; r++)
        {
            float radius = r * ringSpacing;
            int guaranteedGap = Random.Range(0, segmentsPerRing);

            for (int s = 0; s < segmentsPerRing; s++)
            {
                if (s == guaranteedGap || Random.value < 0.2f) continue;

                float angle = s * Mathf.PI * 2f / segmentsPerRing;
                Vector3 position = transform.position + new Vector3(Mathf.Cos(angle) * radius, 1f, Mathf.Sin(angle) * radius);
                Quaternion rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg + 90f, 0);

                GameObject wall = Instantiate(wallPrefab, position, rotation, transform);

                float circum = 2f * Mathf.PI * radius;
                float wallWidth = (circum / segmentsPerRing) * 1.1f; 
                wall.transform.localScale = new Vector3(wallWidth, 3f, 0.5f);
            }
        }
    }
}