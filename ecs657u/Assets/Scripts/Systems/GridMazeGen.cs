using UnityEngine;
using System.Collections.Generic;

public class GridMazeGenerator : MonoBehaviour
{
    [Header("Difficulty Settings")]
    public bool useDebug = true;
    public Difficulty debugDiff = Difficulty.Hard;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject totemPrefab; 
    public GameObject bossGoalPrefab; 
    public float gridSpacing = 4f;

    private List<Vector3> pathPositions = new List<Vector3>();

    void Start() => GenerateSolidMaze();

    void GenerateSolidMaze()
    {
        //Clean up
        foreach (Transform child in transform) Destroy(child.gameObject);
        pathPositions.Clear();

        //Determine Difficulty
        Difficulty activeDiff = (DifficultyManager.Instance != null && !useDebug) 
                                ? DifficultyManager.Instance.currentDifficulty 
                                : debugDiff;

        // APPLY DIFFICULTY LOGIC
        int size = 11;
        int totemsToSpawn = 1;

        if (activeDiff == Difficulty.Easy) 
        { 
            size = 11; 
            totemsToSpawn = 1; 
        }
        else if (activeDiff == Difficulty.Normal) 
        { 
            size = 19; 
            totemsToSpawn = 3; 
        }
        else if (activeDiff == Difficulty.Hard) 
        { 
            size = 27; 
            totemsToSpawn = 5; 
        }

        //Sync with Manager
        if (ShamanTrialManager.Instance != null)
        {
            ShamanTrialManager.Instance.SetTotalTotems(totemsToSpawn);
        }

        // Build Maze Data
        int[,] grid = new int[size, size];
        CarvePath(1, 1, grid, size);

        grid[1, 0] = 1; 
        grid[size - 2, size - 1] = 1;

        //Build Physical Walls
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                Vector3 worldPos = transform.position + new Vector3(x * gridSpacing, 0, z * gridSpacing);
                
                if (grid[x, z] == 0)
                {
                    GameObject wall = Instantiate(wallPrefab, worldPos + Vector3.up * 1.5f, Quaternion.identity, transform);
                    wall.transform.localScale = new Vector3(gridSpacing * 1.05f, 4f, gridSpacing * 1.05f);
                }
                else
                {
                    if (x > 1 && x < size - 2) pathPositions.Add(worldPos + Vector3.up * 1f);
                }
            }
        }

        SpawnTrialObjectives(totemsToSpawn);
        SpawnExitGoal(size);
    }

    
    void SpawnTrialObjectives(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (pathPositions.Count == 0) break;
            int randomIndex = Random.Range(0, pathPositions.Count);
            Instantiate(totemPrefab, pathPositions[randomIndex], Quaternion.identity, transform);
            pathPositions.RemoveAt(randomIndex);
        }
    }

    void SpawnExitGoal(int size)
    {
        Vector3 exitPos = transform.position + new Vector3((size - 2) * gridSpacing, 0.1f, (size - 1) * gridSpacing);
        GameObject exitBlock = Instantiate(bossGoalPrefab, exitPos, Quaternion.identity, transform);
        
        if (ShamanTrialManager.Instance != null)
        {
            ShamanTrialManager.Instance.exitGate = exitBlock;
        }
    }

    void CarvePath(int x, int z, int[,] grid, int size)
    {
        grid[x, z] = 1;
        int[] dirs = { 0, 1, 2, 3 };
        for (int i = 0; i < dirs.Length; i++) {
            int tmp = dirs[i]; int r = Random.Range(i, dirs.Length);
            dirs[i] = dirs[r]; dirs[r] = tmp;
        }
        foreach (int dir in dirs) {
            int dx = (dir == 1) ? 2 : (dir == 3) ? -2 : 0;
            int dz = (dir == 0) ? 2 : (dir == 2) ? -2 : 0;
            int nx = x + dx; int nz = z + dz;
            if (nx > 0 && nx < size - 1 && nz > 0 && nz < size - 1 && grid[nx, nz] == 0) {
                grid[x + (dx / 2), z + (dz / 2)] = 1;
                CarvePath(nx, nz, grid, size);
            }
        }
    }
}