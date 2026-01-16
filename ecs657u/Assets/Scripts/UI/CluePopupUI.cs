using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class CluePopupUI : MonoBehaviour
{
    public static CluePopupUI Instance;

    [Header("UI References")]
    public GameObject popupPanel;
    public TMP_Text clueText; 

    // Reference to player to lock movement (optional but recommended)
    private PlayerController_NewInput player;

    void Awake()
    {
        Instance = this;
        popupPanel.SetActive(false);
    }

    public void ShowClue(string text)
    {
        // 1. Set the text
        clueText.text = text;
        
        // 2. Show the panel
        popupPanel.SetActive(true);

        // 3. Pause Game / Unlock Cursor
        Time.timeScale = 0f; // Pauses physics/movement
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseClue()
    {
        // 1. Hide panel
        popupPanel.SetActive(false);

        // 2. Unpause
        Time.timeScale = 1f;

        // 3. Lock Cursor again (if you want to return to FPS controls immediately)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Safety: If you have a specific player script, you might need to re-enable it here
        // if Time.timeScale isn't enough to stop input.
    }
}