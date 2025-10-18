using UnityEngine;

public class SceneCursorSetter : MonoBehaviour
{
    public bool lockCursorInThisScene = false;

    void OnEnable()
    {
        Cursor.lockState = lockCursorInThisScene ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible   = !lockCursorInThisScene;
        Time.timeScale   = 1f; // just in case something paused the game
    }
}
