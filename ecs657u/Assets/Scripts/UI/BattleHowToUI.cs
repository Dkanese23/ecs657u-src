using UnityEngine;
using UnityEngine.UI;
using System;

// A code-driven tutorial popup that informs the player without needing prefabs
public class BattleHowToUI : MonoBehaviour
{
    static BattleHowToUI _i;
    
    // The static entry point—call this from any script to trigger a help window
    public static void Show(string message, Action onClose)
    {
        if (_i == null)
        {
            var go = new GameObject("BattleHowToUI");
            _i = go.AddComponent<BattleHowToUI>();
            DontDestroyOnLoad(go);
        }
        _i.Build(message, onClose);
    }

    Canvas canvas;
    GameObject blocker, panel;
    Button closeBtn;
    Action _onClose;

    void Build(string message, Action onClose)
    {
        _onClose = onClose;

        if (!canvas)
        {
            // Set up a scalable UI root
            var cgo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            cgo.transform.SetParent(transform, false);
            canvas = cgo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var scaler = cgo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Create a dim background to isolate the popup
            blocker = new GameObject("Dim", typeof(Image));
            blocker.transform.SetParent(canvas.transform, false);
            var dim = blocker.GetComponent<Image>();
            dim.color = new Color(0, 0, 0, 0.45f);
            
            // Layout logic: anchoring ensures the panel stays centered
            var drt = dim.rectTransform; 
            drt.anchorMin = Vector2.zero; 
            drt.anchorMax = Vector2.one; 
            drt.offsetMin = drt.offsetMax = Vector2.zero;

            // [Hierarchy construction logic for Text and Buttons...]
            // Using legacy text here for maximum compatibility without extra package setup
        }

        // Update the message and show the panel
        var bodyRef = panel.transform.Find("Body")?.GetComponent<Text>();
        if (bodyRef) bodyRef.text = message;

        canvas.gameObject.SetActive(true);
        blocker.SetActive(true);
        panel.SetActive(true);

        // Ensure the player can actually use their mouse
        Cursor.visible = true; 
        Cursor.lockState = CursorLockMode.None;
    }

    void Close()
    {
        if (panel) panel.SetActive(false);
        if (blocker) blocker.SetActive(false);
        
        // Execute the 'callback' action—this tells the rest of the game to resume
        _onClose?.Invoke();
    }
}