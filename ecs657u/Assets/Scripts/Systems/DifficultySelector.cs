using UnityEngine;
using UnityEngine.UI;

public class MainMenuDifficulty : MonoBehaviour
{
    [Header("Buttons")]
    public Image easyBtnImage;
    public Image normalBtnImage;
    public Image hardBtnImage;
    public Image customBtnImage; // <--- NEW: Add this!

    [Header("Colors")]
    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;

    public CustomDifficultyUI sliderPanel;

    void Start()
    {
        UpdateButtonVisuals();
    }

    public void SetEasy()
    {
        DifficultyManager.Instance.ApplyDifficulty(Difficulty.Easy);
        UpdateButtonVisuals();
        sliderPanel.RefreshUI();
    }

    public void SetNormal()
    {
        DifficultyManager.Instance.ApplyDifficulty(Difficulty.Normal);
        UpdateButtonVisuals();
        sliderPanel.RefreshUI();
    }

    public void SetHard()
    {
        DifficultyManager.Instance.ApplyDifficulty(Difficulty.Hard);
        UpdateButtonVisuals();
        sliderPanel.RefreshUI();
    }

    public void SetCustomMode()
    {
        // Check if we are already in custom to avoid resetting values
        if (DifficultyManager.Instance.currentDifficulty != Difficulty.Custom)
        {
            DifficultyManager.Instance.ApplyDifficulty(Difficulty.Custom);
        }
        
        UpdateButtonVisuals(); // <--- NEW: Now we update colors here too!
        sliderPanel.RefreshUI();
    }

    void UpdateButtonVisuals()
    {
        // 1. Reset ALL buttons to white first
        easyBtnImage.color = defaultColor;
        normalBtnImage.color = defaultColor;
        hardBtnImage.color = defaultColor;
        customBtnImage.color = defaultColor; // <--- NEW: Reset custom too

        // 2. Color only the active one
        switch (DifficultyManager.Instance.currentDifficulty)
        {
            case Difficulty.Easy:
                easyBtnImage.color = selectedColor;
                break;
            case Difficulty.Normal:
                normalBtnImage.color = selectedColor;
                break;
            case Difficulty.Hard:
                hardBtnImage.color = selectedColor;
                break;
            case Difficulty.Custom: // <--- NEW: Handle Custom case
                customBtnImage.color = selectedColor;
                break;
        }
    }
}