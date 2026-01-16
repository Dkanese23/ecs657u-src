using UnityEngine;

// Manages the persistent presence of an enemy in the overworld
public class OverworldEnemy : MonoBehaviour
{
    [Tooltip("Unique ID for this enemy in the overworld (e.g., 'forest_shaman_01')")]
    public string enemyId;

    [Tooltip("Optional type label, e.g. 'Shaman' (used for spawning correct enemy in battle)")]
    public string enemyType = "Shaman";

    // Standard Unity lifecycle method called when the object is initialised
    void Start()
    {
        // Persistence Check: Self-destruct/disable if the global state marks this ID as defeated
        if (GameState.I != null && GameState.I.IsEnemyDefeated(enemyId))
        {
            Debug.Log($"[Persistence] Enemy {enemyId} has already been defeated. Removing from scene.");
            gameObject.SetActive(false);
        }
    }
}