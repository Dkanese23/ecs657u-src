using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Defeated enemies (by ID)")]
    HashSet<string> defeated = new HashSet<string>();

    [Header("Inventory / Key Items")]
    HashSet<string> keyItems = new HashSet<string>();

    [Header("Respawn / Checkpoint")]
    public string lastScene = "Main";
    public Vector3 checkpointPos;
    public Quaternion checkpointRot = Quaternion.identity;
    public bool hasCheckpoint = false;
    public bool pendingRespawn = false;

    [Header("Current Encounter (for Battle scene)")]
    public string currentEnemyId = null;
    public string currentEnemyType = null;   // optional: "Shaman", "Brute" etc.

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- Defeated enemies ---
    public void MarkEnemyDefeated(string enemyId) { if (!string.IsNullOrEmpty(enemyId)) defeated.Add(enemyId); }
    public bool IsEnemyDefeated(string enemyId) => !string.IsNullOrEmpty(enemyId) && defeated.Contains(enemyId);

    // --- Inventory ---
    public void AddKeyItem(string itemId) { if (!string.IsNullOrEmpty(itemId)) keyItems.Add(itemId); }
    public bool HasKeyItem(string itemId) => !string.IsNullOrEmpty(itemId) && keyItems.Contains(itemId);

    // --- Checkpoint ---
    public void SetCheckpoint(string sceneName, Vector3 pos, Quaternion rot)
    {
        lastScene = sceneName;
        checkpointPos = pos;
        checkpointRot = rot;
        hasCheckpoint = true;
    }

    // --- Encounter context ---
    public void StartEncounter(string enemyId, string enemyType = null)
    {
        currentEnemyId = enemyId;
        currentEnemyType = enemyType;
    }

    public void ClearEncounter()
    {
        currentEnemyId = null;
        currentEnemyType = null;
    }
}
