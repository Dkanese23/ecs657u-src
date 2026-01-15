using UnityEngine;

public class CircularMazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    public int rings = 4;
    public int segmentsPerRing = 16;
    public float ringSpacing = 4f;

    [Header("Prefabs")]
    public GameObject wallPrefab;

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
{
    // Clear existing walls
    foreach (Transform child in transform) Destroy(child.gameObject);

    for (int r = 1; r <= rings; r++)
    {
        float radius = r * ringSpacing;
        
        // Pick one segment that is ALWAYS a gap to ensure traversal
        int guaranteedGap = Random.Range(0, segmentsPerRing);

        for (int s = 0; s < segmentsPerRing; s++)
        {
            // Skip the guaranteed gap AND random ones
            if (s == guaranteedGap || Random.value < 0.2f) continue;

            float angle = s * Mathf.PI * 2f / segmentsPerRing;
            Vector3 position = transform.position + new Vector3(Mathf.Cos(angle) * radius, 1f, Mathf.Sin(angle) * radius);

            //Makes the walls form a circle
            Quaternion rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg + 90f, 0);

            GameObject wall = Instantiate(wallPrefab, position, rotation, transform);

            // Math to make walls touch without gaps
            float circum = 2f * Mathf.PI * radius;
            float wallWidth = (circum / segmentsPerRing) * 1.1f; // 1.1f overlap prevents "cracks"
            wall.transform.localScale = new Vector3(wallWidth, 3f, 0.5f);
        }
    }
}
}