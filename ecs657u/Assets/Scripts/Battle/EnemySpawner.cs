using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public EnemyBase shamanPrefab;
    public EnemyBase berserkerPrefab;
    public EnemyBase tankPrefab;

    [Header("Spawn")]
    public Transform spawnPoint;

    [Header("Refs")]
    public BattleManager battleManager;

    void Start()
    {
        string t = GameState.I?.currentEnemyType ?? "Shaman";
        EnemyBase prefab = t switch
        {
            "Berserker"  => berserkerPrefab,
            "Tank" => tankPrefab,
            _        => shamanPrefab
        };

        var pos = spawnPoint ? spawnPoint.position : transform.position;
        var rot = spawnPoint ? spawnPoint.rotation : transform.rotation;

        var enemy = Instantiate(prefab, pos, rot);
        battleManager.AttachEnemy(enemy);     // see method below
    }
}
