using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Added this to make calls shorter

public class BossZone : MonoBehaviour
{
    // 1. TYPE THE SCENE NAME HERE IN THE INSPECTOR (Default is "BossBattle")
    public string battleSceneName = "BossBattle"; 
    
    // public string mainSceneName = "Main(prototype)"; // Unused, usually handled by GameState checkpoint
    public string[] requiredMiniBosses = { "MiniBoss1" };

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Save checkpoint using current scene name
        var player = other.transform;
        if (GameState.I != null)
        {
            GameState.I.SetCheckpoint(SceneManager.GetActiveScene().name, player.position, player.rotation);
        }

        // 2. USE THE VARIABLE HERE TO LOAD THE BATTLE SCENE
        SceneManager.LoadScene(battleSceneName);
    }
}