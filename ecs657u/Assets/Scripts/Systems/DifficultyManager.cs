using UnityEngine;
// This defines the types of difficulty available in the game.
// Placing it outside the class makes it accessible to all other scripts.
public class DifficultyManager : MonoBehaviour
{   
    // Singleton pattern: allows other scripts (like EnemyBase) to access 
    // these settings easily via DifficultyManager.Instance
    public static DifficultyManager Instance;

    public Difficulty currentDifficulty = Difficulty.Normal;

    [Header("Enemy Scaling")]
    public float enemyHealthMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;

    [Header("AI Scaling")]
    public float specialMoveChanceBonus = 0f;

    [Header("Player Scaling")]
    public int bonusCardDraw = 0;

    private void Awake()
    {   //Ensures only one DifficultyManager exists across all scenes.
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // This makes sure the difficulty selection isn't lost when moving 
        // from the Main Menu to the Battle Scene.
        DontDestroyOnLoad(gameObject);

        ApplyDifficulty(currentDifficulty);
    }
    // Updates all game multipliers based on the chosen difficulty level.
    // Call this from your Main Menu buttons.
    public void ApplyDifficulty(Difficulty diff)
    {
        currentDifficulty = diff;

        switch (diff)
        {
            case Difficulty.Easy:
                enemyHealthMultiplier = 0.8f;
                enemyDamageMultiplier = 0.75f;
                specialMoveChanceBonus = -0.15f; // Enemies use specials 15% less often
                bonusCardDraw = 1; // Player draws 1 extra card per turn
                break;

            case Difficulty.Normal:
                enemyHealthMultiplier = 1f; 
                enemyDamageMultiplier = 1f;
                specialMoveChanceBonus = 0f;
                bonusCardDraw = 0;
                break;

            case Difficulty.Hard:
                enemyHealthMultiplier = 1.7f;
                enemyDamageMultiplier = 1.5f;
                specialMoveChanceBonus = 0.2f; // Enemies use specials 20% more often
                bonusCardDraw = 0;
                break;
        }
    }
}
