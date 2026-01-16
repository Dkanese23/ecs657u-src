using UnityEngine;
using UnityEngine.UI;

// Syncs the UI slider position with the actual sensitivity value in the PlayerController
public class UpdateSensitivitySlider : MonoBehaviour
{
    [Header("References")]
    public PlayerController_NewInput player; 
    private Slider mySlider;

    // Runs every time the UI object is turned on (e.g., opening the settings menu)
    void OnEnable()
    {
        mySlider = GetComponent<Slider>();

        // Ensure both the player and the slider exist before trying to sync
        if(player != null && mySlider != null)
        {
            // Pull the current value from the logic and push it to the visual UI
            mySlider.value = player.lookSensitivity;
            Debug.Log($"[Settings] Syncing slider to current sensitivity: {player.lookSensitivity}");
        }
    }
}