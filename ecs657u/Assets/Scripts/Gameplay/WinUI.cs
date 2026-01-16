using UnityEngine;
using UnityEngine.UI;

// A self-contained, singleton-driven UI system for victory screens
public class WinUI : MonoBehaviour
{
    static WinUI _instance;

    // Static entry point allows any script to trigger the Win screen instantly
    public static void Show(string title = "Congrats! You Win 🎉",
                            string note  = "Boss coming later…")
    {
        if (_instance == null)
        {
            // Creates a persistent manager object that survives scene transitions
            var go = new GameObject("WinUI");
            _instance = go.AddComponent<WinUI>();
            DontDestroyOnLoad(go);
        }
        _instance.Build(title, note);
    }

    Canvas canvas;
    GameObject blocker;
    Button quitBtn;

    // Procedurally generates the UI hierarchy to ensure the system is "Zero-Setup"
    void Build(string title, string note)
    {
        if (!canvas)
        {
            // 1. Setup Canvas and Scaler for multi-resolution support
            var cgo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            cgo.transform.SetParent(transform, false);
            canvas = cgo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var scaler = cgo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // 2. Create Background 'Dim' (Accessibility: Focuses user attention)
            blocker = new GameObject("Dim", typeof(Image));
            blocker.transform.SetParent(canvas.transform, false);
            var dim = blocker.GetComponent<Image>();
            dim.color = new Color(0, 0, 0, 0.6f);
            
            // 3. Setup Layout (Anchors/RectTransform) to ensure UI doesn't break on different screens
            var dimRT = dim.rectTransform; 
            dimRT.anchorMin = Vector2.zero; 
            dimRT.anchorMax = Vector2.one;
            dimRT.offsetMin = dimRT.offsetMax = Vector2.zero;

            // [Hierarchy construction continues: Panel -> Title -> Note -> Button]
            // ... (rest of your procedural logic) ...

            quitBtn = canvas.transform.Find("Panel/QuitButton").GetComponent<Button>();
            quitBtn.onClick.AddListener(QuitGame);
        }

        // 4. Game State Management: Pause the game world while UI is active
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Handles clean exit for both the Unity Editor and the final build
    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}