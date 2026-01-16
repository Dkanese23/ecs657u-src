using UnityEngine;
using UnityEngine.UI;

public class MainMenuDifficulty : MonoBehaviour
{
    [Header("Buttons")]
    public Image easyBtnImage;
    public Image normalBtnImage;
    public Image hardBtnImage;
    public Image customBtnImage;

    [Header("Colors")]
    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;

    // Reference to the panel with health/damage sliders
    public CustomDifficultyUI sliderPanel;

    void Start()
    {
        // Sync visuals with current manager state on load
        UpdateButtonVisuals();
    }

    // Methods for the Button OnClick events
    public void SetEasy()   => ChangeDifficulty(Difficulty.Easy);
    public void SetNormal() => ChangeDifficulty(Difficulty.Normal);
    public void SetHard()   => ChangeDifficulty(Difficulty.Hard);

    public void SetCustomMode()
    {
        // Avoid resetting values if we are already in custom mode
        if (DifficultyManager.Instance.currentDifficulty != Difficulty.Custom)
        {
            DifficultyManager.Instance.ApplyDifficulty(Difficulty.Custom);
        }
        
        UpdateButtonVisuals();
        sliderPanel.RefreshUI();
    }

    
    private void ChangeDifficulty(Difficulty setting)
    {
        DifficultyManager.Instance.ApplyDifficulty(setting);
        UpdateButtonVisuals();
        sliderPanel.RefreshUI();
    }

    void UpdateButtonVisuals()
    {
        // Reset all to default first to clear previous selection
        easyBtnImage.color = defaultColor;
        normalBtnImage.color = defaultColor;
        hardBtnImage.color = defaultColor;
        customBtnImage.color = defaultColor;

        // Highlight the current active choice
        switch (DifficultyManager.Instance.currentDifficulty)
        {
            case Difficulty.Easy:   easyBtnImage.color = selectedColor;   break;
            case Difficulty.Normal: normalBtnImage.color = selectedColor; break;
            case Difficulty.Hard:   hardBtnImage.color = selectedColor;   break;
            case Difficulty.Custom: customBtnImage.color = selectedColor; break;
        }
    }
}