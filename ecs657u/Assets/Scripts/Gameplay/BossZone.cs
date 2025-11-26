using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 

public class BossZone : MonoBehaviour
{
    // triggers boss battle when player collides and has defeated the three required mini-bosses
    public string battleSceneName = "BossBattle";
    public string mainSceneName = "Main(prototype)";
    public string[] requiredMiniBosses = { "MiniBoss1", "MiniBoss2", "MiniBoss3" };

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Check if all required mini-bosses have been defeated
        foreach (var bossId in requiredMiniBosses)
        {
            if (!GameState.I.IsEnemyDefeated(bossId))
            {
                Debug.Log("Cannot enter boss battle yet. Mini-boss " + bossId + " not defeated.");
                return;
            }
        }

        // Save checkpoint (so we can put player back here on defeat or after battle)
        var player = other.transform;
        GameState.I.SetCheckpoint(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, player.position, player.rotation);

        // Set up encounter context for the boss battle
        GameState.I.StartEncounter("BossEnemy", "BossType");

        // Go to boss battle (not implemented yet)
        // UnityEngine.SceneManagement.SceneManager.LoadScene(battleSceneName);
        WinUI.Show("Congrats! You Win 🎉", "Boss coming later…");
    }
}
