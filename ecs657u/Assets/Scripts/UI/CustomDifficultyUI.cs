using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomDifficultyUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider healthSlider;
    public Slider damageSlider;
    public Slider specialSlider;
    //public Slider cardDrawSlider;

    [Header("Value Labels")]
    public Text healthText;
    public Text damageText;
    public Text specialText;
    //public TextMeshProUGUI cardText;

    

    // A flag to prevent the sliders from updating the Manager while we are just setting them up visually
    private bool isUpdatingVisuals = false; 

    void Start()
    {
        // On start, sync UI with whatever the current difficulty is
        RefreshUI();
    }

    void OnEnable()
    {
        // Whenever this panel opens, refresh the data
        RefreshUI();
    }

    // Call this whenever the Difficulty changes (e.g. from your Easy/Normal/Hard buttons)
    public void RefreshUI()
    {
        if (DifficultyManager.Instance == null) return;

        isUpdatingVisuals = true; // STOP the sliders from telling the Manager to switch to "Custom"

        // 1. Set Slider Values
        healthSlider.value = DifficultyManager.Instance.enemyHealthMultiplier;
        damageSlider.value = DifficultyManager.Instance.enemyDamageMultiplier;
        specialSlider.value = DifficultyManager.Instance.specialMoveChanceBonus;
        //cardDrawSlider.value = DifficultyManager.Instance.bonusCardDraw;

        // 2. Lock or Unlock sliders based on mode
        bool isCustom = DifficultyManager.Instance.currentDifficulty == Difficulty.Custom;
        SetSlidersInteractable(isCustom);

        // 3. Update Text
        UpdateLabels();

        isUpdatingVisuals = false; // OK, now listening for user input again
    }

    // Called by the Sliders "OnValueChanged" event
    public void OnSliderChanged()
    {
        // If we are just refreshing the UI via code, ignore this event
        if (isUpdatingVisuals) return; 

        UpdateLabels();

        // Send data to the Manager (and switch to Custom mode)
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetCustomDifficulty(
                healthSlider.value,
                damageSlider.value,
                specialSlider.value,
                0
            );
            
            // Ensure sliders are unlocked (in case we weren't already in Custom)
            SetSlidersInteractable(true);
        }
    }

    void UpdateLabels()
    {
        healthText.text = $"Health: {healthSlider.value:F1}x"; 
        damageText.text = $"Damage: {damageSlider.value:F1}x";
        specialText.text = $"Special Chance: +{specialSlider.value * 100:F0}%"; 
        //cardText.text = $"Bonus Cards: {cardDrawSlider.value}";
    }

    void SetSlidersInteractable(bool state)
    {
        healthSlider.interactable = state;
        damageSlider.interactable = state;
        specialSlider.interactable = state;
        //cardDrawSlider.interactable = state;
    }
}