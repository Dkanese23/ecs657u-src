using UnityEngine;

public class OverworldEnemy : MonoBehaviour
{
    [Tooltip("Unique ID for this enemy in the overworld (e.g., 'forest_shaman_01')")]
    public string enemyId;

    [Tooltip("Optional type label, e.g. 'Shaman' (used for spawning correct enemy in battle)")]
    public string enemyType = "Shaman";

    void Start()
    {
        if (GameState.I != null && GameState.I.IsEnemyDefeated(enemyId))
            gameObject.SetActive(false);
    }
}
