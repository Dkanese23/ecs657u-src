using UnityEngine;
using UnityEngine.SceneManagement;

// Synchronises the scene state with the global GameState upon loading
public class SceneBootstrap : MonoBehaviour
{
    public Transform player;
    public float respawnBackDistance = 4f; // Buffer to prevent re-triggering encounters
    public float respawnUpOffset = 0.2f;   // Prevents player from getting stuck in the floor
    

    void Start()
    {
        // 1. Persistence Cleanup: Ensures defeated enemies remain gone
        var enemies = FindObjectsOfType<OverworldEnemy>(true);
        foreach (var e in enemies)
        {
            if (GameState.I != null && GameState.I.IsEnemyDefeated(e.enemyId))
                e.gameObject.SetActive(false);
        }

        // 2. Respawn Logic: Places the player safely after a defeat or retreat
        if (GameState.I != null && GameState.I.pendingRespawn && GameState.I.hasCheckpoint)
        {
            Vector3 cpPos = GameState.I.checkpointPos;
            Quaternion cpRot = GameState.I.checkpointRot;

            // Calculate a 'safe' spot behind where they were facing
            Vector3 back = -(cpRot * Vector3.forward) * respawnBackDistance;
            Vector3 safePos = cpPos + back + Vector3.up * respawnUpOffset;

            // Update the player's physical location
            player.SetPositionAndRotation(safePos, cpRot);
            
            // Reset the flag so this doesn't happen every time the scene loads
            GameState.I.pendingRespawn = false;
        }
    }
}