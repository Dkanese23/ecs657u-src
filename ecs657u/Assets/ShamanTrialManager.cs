using UnityEngine;
using TMPro;

// Manages the objective state and progression logic for the Shaman Trial
public class ShamanTrialManager : MonoBehaviour
{
    public static ShamanTrialManager Instance;
    
    [Header("Trial State")]
    public int totalTotems;
    private int collectedTotems = 0;

    [Header("Level References")]
    public GameObject exitGate; // The physical barrier blocking the end of the maze
    public TextMeshProUGUI objectiveText; 

    void Awake() 
    { 
        // Singleton ensures global access from any totem or script
        if (Instance == null) Instance = this; 
    }

    // Called by GridMazeGenerator once the maze size is decided
    public void SetTotalTotems(int amount)
    {
        totalTotems = amount;
        collectedTotems = 0; 
        UpdateUI();
    }

    // Called by individual Totem scripts when player interacts with them
    public void CollectTotem()
    {
        collectedTotems++;
        UpdateUI();

        // Check if the win condition has been met
        if (collectedTotems >= totalTotems && totalTotems > 0)
        {
            OpenExit();
        }
    }

    void UpdateUI()
    {
        if (objectiveText != null)
            objectiveText.text = $"Totems: {collectedTotems} / {totalTotems}";
    }

    void OpenExit()
    {
        Debug.Log("Trial Complete! The barrier has vanished.");
        
        // Disabling the gate allows the player to walk through to the next scene
        if (exitGate != null) 
        {
            exitGate.SetActive(false); 
        }
    }
}