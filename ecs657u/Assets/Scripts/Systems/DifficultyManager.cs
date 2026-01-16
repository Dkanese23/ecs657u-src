using UnityEngine;

public class DifficultyManager : MonoBehaviour
{ 
    public static DifficultyManager Instance;

    public Difficulty currentDifficulty = Difficulty.Normal;

    [Header("Active Settings")]
    public float enemyHealthMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
    public float specialMoveChanceBonus = 0f;
    public int bonusCardDraw = 0;

    [Header("Saved Custom Settings")] 
    private float customHealth = 1f;
    private float customDamage = 1f;
    private float customSpecial = 0f;
    private int customDraw = 0;

    private void Awake()
    {   
        // Singleton pattern: stays alive between scenes and prevents duplicates
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        customHealth = enemyHealthMultiplier;
        customDamage = enemyDamageMultiplier;
    }

    public void ApplyDifficulty(Difficulty diff)
    {
        currentDifficulty = diff;

        // Switches between user-defined values or the preset hardcoded ones
        if (diff == Difficulty.Custom)
        {
            enemyHealthMultiplier = customHealth;
            enemyDamageMultiplier = customDamage;
            specialMoveChanceBonus = customSpecial;
            bonusCardDraw = customDraw;
        }
        else 
        {
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

    // Connects toUI sliders in the settings menu
    public void SetCustomDifficulty(float health, float damage, float specialChance, int cardDraw)
    {
        currentDifficulty = Difficulty.Custom;

        enemyHealthMultiplier = customHealth = health;
        enemyDamageMultiplier = customDamage = damage;
        specialMoveChanceBonus = customSpecial = specialChance;
        bonusCardDraw = customDraw = cardDraw;
    }
}