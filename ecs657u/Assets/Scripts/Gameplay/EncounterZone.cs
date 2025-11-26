using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class LoadBattleOnTrigger : MonoBehaviour
{
    public string enemyId;          // copy from OverworldEnemy on the visible enemy
    public string enemyType = "Shaman";
    public string battleSceneName = "Battle";
    public string mainSceneName = "Main(prototype)";

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Save checkpoint (so we can put player back here on defeat or after battle)
        var player = other.transform;
        GameState.I.SetCheckpoint(SceneManager.GetActiveScene().name, player.position, player.rotation);

        // Set up encounter context
        GameState.I.StartEncounter(enemyId, enemyType);

        // Go to battle
        SceneManager.LoadScene(battleSceneName);
    }
}

