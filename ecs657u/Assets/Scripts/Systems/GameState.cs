using UnityEngine;
using System.Collections.Generic;

// Acts as the central hub for all persistent data across the game
public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    // Using HashSets ensures O(1) lookup performance for progress tracking
    HashSet<string> defeated = new HashSet<string>();
    HashSet<string> keyItems = new HashSet<string>();

    [Header("Respawn / Checkpoint")]
    public string lastScene = "Main";
    public Vector3 checkpointPos;
    public Quaternion checkpointRot = Quaternion.identity;
    public bool hasCheckpoint = false;
    public bool pendingRespawn = false;

    [Header("Battle Context")]
    public string currentEnemyId = null;
    public string currentEnemyType = null; 

    void Awake()
    {
        // Singleton pattern: ensures only one 'Brain' exists and it never dies
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- Progression Logic ---
    public void MarkEnemyDefeated(string enemyId) { if (!string.IsNullOrEmpty(enemyId)) defeated.Add(enemyId); }
    public bool IsEnemyDefeated(string enemyId) => !string.IsNullOrEmpty(enemyId) && defeated.Contains(enemyId);

    // --- Inventory System ---
    public void AddKeyItem(string itemId) { if (!string.IsNullOrEmpty(itemId)) keyItems.Add(itemId); }
    public bool HasKeyItem(string itemId) => !string.IsNullOrEmpty(itemId) && keyItems.Contains(itemId);

    // --- Checkpoint System ---
    // Saves where the player was so we can return them there after a battle
    public void SetCheckpoint(string sceneName, Vector3 pos, Quaternion rot)
    {
        lastScene = sceneName;
        checkpointPos = pos;
        checkpointRot = rot;
        hasCheckpoint = true;
    }

    // --- Encounter Handling ---
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