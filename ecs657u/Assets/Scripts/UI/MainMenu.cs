using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    // Call this function when the button is clicked
    public void StartGame()
    {
        // Replace "MainScene" with the EXACT name of your game scene file
        SceneManager.LoadScene("StoryScene");
    }

    public void Credits()
    {

        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        // Logs a message so you know it works in the Editor
        Debug.Log("Quit Game triggered!"); 
        
        // This command closes the build
        Application.Quit();
    }
}

