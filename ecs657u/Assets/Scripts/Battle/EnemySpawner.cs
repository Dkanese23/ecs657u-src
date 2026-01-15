using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public EnemyBase shamanPrefab;
    public EnemyBase berserkerPrefab;
    public EnemyBase tankPrefab;
    public EnemyBase dragonPrefab;

    [Header("Spawn")]
    public Transform spawnPoint;

    [Header("Refs")]
    public BattleManager battleManager;

    [Header("Appearance")]
    public float enemyScale = 1.5f;

    void Start()
    {
        string t = GameState.I?.currentEnemyType ?? "Shaman";
        EnemyBase prefab = t switch
        {
            "Berserker"  => berserkerPrefab,
            "Tank" => tankPrefab,
            "Dragon" => dragonPrefab,
            _        => shamanPrefab
        };

        var pos = spawnPoint ? spawnPoint.position : transform.position;
        var rot = spawnPoint ? spawnPoint.rotation : transform.rotation;

        var enemy = Instantiate(prefab, pos, rot);

        enemy.transform.localScale *= enemyScale;
        battleManager.AttachEnemy(enemy);     // see method below
    }
}
