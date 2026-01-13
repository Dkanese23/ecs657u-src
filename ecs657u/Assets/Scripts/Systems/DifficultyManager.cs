using UnityEngine;
// This defines the types of difficulty available in the game.
// Placing it outside the class makes it accessible to all other scripts.

public class DifficultyManager : MonoBehaviour
{ 
    public static DifficultyManager Instance;

    public Difficulty currentDifficulty = Difficulty.Normal;

    [Header("Active Settings")]
    public float enemyHealthMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
    public float specialMoveChanceBonus = 0f;
    public int bonusCardDraw = 0;

    // NEW: Variables to remember the user's custom config
    [Header("Saved Custom Settings")] 
    private float customHealth = 1f;
    private float customDamage = 1f;
    private float customSpecial = 0f;
    private int customDraw = 0;

    private void Awake()
    {   
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize custom defaults
        customHealth = enemyHealthMultiplier;
        customDamage = enemyDamageMultiplier;
    }

    public void ApplyDifficulty(Difficulty diff)
    {
        currentDifficulty = diff;

        // If switching TO Custom, load the SAVED custom stats
        if (diff == Difficulty.Custom)
        {
            enemyHealthMultiplier = customHealth;
            enemyDamageMultiplier = customDamage;
            specialMoveChanceBonus = customSpecial;
            bonusCardDraw = customDraw;
        }
        else 
        {
            // Otherwise, load the preset stats (Easy/Normal/Hard)
            switch (diff)
            {
                case Difficulty.Easy:
                    enemyHealthMultiplier = 0.8f;
                    enemyDamageMultiplier = 0.75f;
                    specialMoveChanceBonus = -0.15f; 
                    bonusCardDraw = 1; 
                    break;

                case Difficulty.Normal:
                    enemyHealthMultiplier = 1f; 
                    enemyDamageMultiplier = 1f;
                    specialMoveChanceBonus = 0f;
                    bonusCardDraw = 0;
                    break;

                case Difficulty.Hard:
                    enemyHealthMultiplier = 1.25f;
                    enemyDamageMultiplier = 1.3f;
                    specialMoveChanceBonus = 0.2f; 
                    bonusCardDraw = 0;
                    break;
            }
        }
    }

    // Call this when sliders move
    public void SetCustomDifficulty(float health, float damage, float specialChance, int cardDraw)
    {
        currentDifficulty = Difficulty.Custom;

        // 1. Update the Active Game Variables
        enemyHealthMultiplier = health;
        enemyDamageMultiplier = damage;
        specialMoveChanceBonus = specialChance;
        bonusCardDraw = cardDraw;

        // 2. Save them to the "Memory" variables
        customHealth = health;
        customDamage = damage;
        customSpecial = specialChance;
        customDraw = cardDraw;
    }
}
