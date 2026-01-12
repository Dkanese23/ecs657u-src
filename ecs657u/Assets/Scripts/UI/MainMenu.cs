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
        SceneManager.LoadScene("Main(Prototype)");
    }
}

