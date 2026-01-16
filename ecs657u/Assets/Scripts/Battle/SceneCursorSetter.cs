using UnityEngine;

// Manages cursor state and time flow upon scene entry to ensure consistent UX
public class SceneCursorSetter : MonoBehaviour
{
    [Tooltip("Should the cursor be hidden and confined to the centre? (e.g. for First Person control)")]
    public bool lockCursorInThisScene = false;

    // Called when the object is enabled; ideal for scene-entry initialisation
    void OnEnable()
    {
        // Toggle cursor visibility and lock state based on scene requirements
        Cursor.lockState = lockCursorInThisScene ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible   = !lockCursorInThisScene;

        // Reset the time scale to ensure gameplay logic resumes if coming from a paused menu
        Time.timeScale   = 1f; 
        
        Debug.Log($"[SceneCursorSetter] Cursor {(lockCursorInThisScene ? "Locked" : "Visible")}. TimeScale Reset.");
    }
}