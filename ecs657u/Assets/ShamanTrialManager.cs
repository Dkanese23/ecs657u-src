using UnityEngine;
using TMPro;

public class ShamanTrialManager : MonoBehaviour
{
    public static ShamanTrialManager Instance;
    
    public int totalTotems;
    private int collectedTotems = 0;
    public GameObject exitGate; 
    public TextMeshProUGUI objectiveText; 

    void Awake() 
    { 
        if (Instance == null) Instance = this; 
    }

    // --- THE FIX: Call this to update the UI correctly ---
    public void SetTotalTotems(int amount)
    {
        totalTotems = amount;
        collectedTotems = 0; // Reset for new generation
        UpdateUI();
    }

    public void CollectTotem()
    {
        collectedTotems++;
        UpdateUI();

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
    
    // This will now HIDE the object instead of showing it
    if (exitGate != null) 
    {
        exitGate.SetActive(false); 
    }
}
}