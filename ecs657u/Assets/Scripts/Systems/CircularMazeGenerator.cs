using UnityEngine;

// Procedurally generates a circular maze layout based on global difficulty settings
public class CircularMazeGenerator : MonoBehaviour
{
    [Header("Debugging")]
    [Tooltip("Fall-back difficulty used if DifficultyManager is not present in the scene.")]
    public Difficulty debugDifficulty = Difficulty.Hard; 

    [Header("Assets")]
    public GameObject wallPrefab;

    void Start()
    {
        GenerateMaze();
    }

    // Logic for procedural construction using polar coordinates
    void GenerateMaze()
    {
        // Clean up previous generation for iterative testing
        foreach (Transform child in transform) Destroy(child.gameObject);

        // Difficulty Integration: Fetches the player's choice from the persistent manager
        Difficulty currentDiff = (DifficultyManager.Instance != null) 
                                 ? DifficultyManager.Instance.currentDifficulty 
                                 : debugDifficulty;

        // Parametric Scaling: Values adjust based on difficulty level
        int rings = 5;
        int segmentsPerRing = 24;
        float ringSpacing = 5f;

        switch (currentDiff)
        {
            case Difficulty.Easy:
                rings = 4; segmentsPerRing = 24; ringSpacing = 6f;
                break;
            case Difficulty.Normal:
                rings = 6; segmentsPerRing = 27; ringSpacing = 5.5f;
                break;
            case Difficulty.Hard:
                rings = 8; segmentsPerRing = 30; ringSpacing = 5f;
                break;
        }

        // Procedural Loop: Spawns walls in concentric circles
        for (int r = 1; r <= rings; r++)
        {
            float radius = r * ringSpacing;
            // Ensures at least one path exists through the ring to guarantee solvability
            int guaranteedGap = Random.Range(0, segmentsPerRing);

            for (int s = 0; s < segmentsPerRing; s++)
            {
                // Logic for creating gaps in the maze
                if (s == guaranteedGap || Random.value < 0.2f) continue;

                // Trigonometry: Converting polar (angle/radius) to Cartesian (X/Z) coordinates
                float angle = s * Mathf.PI * 2f / segmentsPerRing;
                Vector3 position = transform.position + new Vector3(Mathf.Cos(angle) * radius, 1f, Mathf.Sin(angle) * radius);
                
                // Rotation: Orients the wall to face the centre of the maze
                Quaternion rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg + 90f, 0);

                GameObject wall = Instantiate(wallPrefab, position, rotation, transform);

                // Dynamic Scaling: Calculates wall width based on circumference to prevent gaps
                float circum = 2f * Mathf.PI * radius;
                float wallWidth = (circum / segmentsPerRing) * 1.1f; 
                wall.transform.localScale = new Vector3(wallWidth, 3f, 0.5f);
            }
        }
    }
}