using UnityEngine;

// Responsible for dynamically instantiating the correct enemy type based on global state
public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public EnemyBase shamanPrefab;
    public EnemyBase berserkerPrefab;
    public EnemyBase tankPrefab;
    public EnemyBase dragonPrefab;

    [Header("Spawn Settings")]
    public Transform spawnPoint;

    [Header("References")]
    public BattleManager battleManager;

    [Header("Appearance")]
    public float enemyScale = 1.5f;

    void Start()
    {
        // Retrieves the enemy type from the persistent GameState singleton
        // Utilises the null-coalescing operator (??) to provide a safe fallback
        string t = GameState.I?.currentEnemyType ?? "Shaman";

        // Switch expression: A modern C# feature for cleaner and more readable branching
        EnemyBase prefab = t switch
        {
            "Berserker" => berserkerPrefab,
            "Tank"      => tankPrefab,
            "Dragon"    => dragonPrefab,
            _           => shamanPrefab // Default case ensures the game never crashes
        };

        // Determine spawn coordinates, defaulting to the spawner's transform if no point is set
        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : transform.rotation;

        // Instantiate the specific subclass into the scene
        EnemyBase enemy = Instantiate(prefab, pos, rot);

        // Apply visual scaling and link the instance to the BattleManager for turn logic
        enemy.transform.localScale *= enemyScale;
        battleManager.AttachEnemy(enemy);
    }
}