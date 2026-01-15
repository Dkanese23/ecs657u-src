using UnityEngine;
using UnityEngine.SceneManagement; // Required for switching scenes

public class ReturnToMenu : MonoBehaviour
{
    [Header("Settings")]
    // Type the exact name of your Menu scene here in the Inspector
    public string menuSceneName = "StartMenu"; 

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}