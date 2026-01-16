using UnityEngine;
using UnityEngine.SceneManagement;

// Triggers the transition from the overworld to the battle scene upon player contact
[RequireComponent(typeof(Collider))]
public class LoadBattleOnTrigger : MonoBehaviour
{
    [Header("Encounter Settings")]
    public string enemyId;           // Unique ID to track if this specific enemy is defeated
    public string enemyType = "Shaman";
    public string battleSceneName = "Battle";
    public string mainSceneName = "Main(prototype)";

    // Automatically configures the physics components for the designer
    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // CharacterControllers require a kinematic Rigidbody to trigger events reliably
        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    // Called when the player's collider enters this trigger volume
    private void OnTriggerEnter(Collider other)
    {
        // Tag-based filtering ensures only the player can initiate combat
        if (!other.CompareTag("Player")) return;

        // Persistence: Record current position so the player can return here post-battle
        var player = other.transform;
        if (GameState.I != null)
        {
            GameState.I.SetCheckpoint(SceneManager.GetActiveScene().name, player.position, player.rotation);

            // Passes the enemy context to the BattleManager via the Global State
            GameState.I.StartEncounter(enemyId, enemyType);
        }

        Debug.Log($"[Encounter] Initiating battle with {enemyType} ({enemyId})");
        
        // Transition to the dedicated combat scene
        SceneManager.LoadScene(battleSceneName);
    }
}