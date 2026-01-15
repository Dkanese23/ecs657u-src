using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBootstrap : MonoBehaviour
{
    public Transform player;  // drag your player here
    public float respawnBackDistance = 4f;
    public float respawnUpOffset = 0.2f;
    

    void Start()
    {
        // 1) Hide defeated overworld enemies (OverworldEnemy does this on Start() too,
        //    but doing it here ensures it's cleaned even if enemies are part of a pool)
        var enemies = FindObjectsOfType<OverworldEnemy>(true);
        foreach (var e in enemies)
            if (GameState.I != null && GameState.I.IsEnemyDefeated(e.enemyId))
                e.gameObject.SetActive(false);

        // 2) Respawn player at last checkpoint if pending
        if (GameState.I != null && GameState.I.pendingRespawn && GameState.I.hasCheckpoint)
        {
            // Optional: ensure checkpoint belongs to this scene
            // If not, you can load that scene first. For now we assume Main.
            Vector3 cpPos = GameState.I.checkpointPos;
            Quaternion cpRot = GameState.I.checkpointRot;

            Vector3 back = -(cpRot * Vector3.forward) * respawnBackDistance;
            Vector3 safePos = cpPos + back + Vector3.up * respawnUpOffset;

            player.SetPositionAndRotation(safePos, GameState.I.checkpointRot);
            GameState.I.pendingRespawn = false;
        }

        // 3) (Optional) Open a small toast/UI about new key items, etc.
        // if (GameState.I.HasKeyItem("key_forest_shaman")) { ... }
    }
}
