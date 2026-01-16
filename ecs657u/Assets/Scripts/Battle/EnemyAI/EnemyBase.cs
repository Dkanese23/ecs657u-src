using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Abstract base class for all enemy types, handling AI and stat scaling
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    public string enemyName = "Enemy";
    public int attackDamage = 3;
    public Health Health { get; private set; }

    [Header("Procedural Generation")]
    public bool randomizeStats = true; // Enables variance to ensure unique encounters
    [Range(0f, 0.5f)] 
    public float variationRange = 0.2f; // +/- 20% random variation

    [Header("Difficulty State")]
    protected bool isHardMode;

    [Header("AI Behavior")]
    public string nextAction = ""; 
    public int actionValue = 0;    

    protected BattleManager battleManager;

    protected virtual void Awake()
    {
        Health = GetComponent<Health>();
    }

    // Adjusts base stats with a random multiplier to support procedural variety
    protected virtual void ApplyRandomization()
    {
        if (!randomizeStats) return;

        float multiplier = Random.Range(1f - variationRange, 1f + variationRange);

        // Randomise Attack
        attackDamage = Mathf.RoundToInt(attackDamage * multiplier);
        if (attackDamage < 1) attackDamage = 1;

        // Randomise Health components
        if (Health != null)
        {
            int newMaxHP = Mathf.RoundToInt(Health.MaxHP * multiplier);
            Health.SetMaxHP(newMaxHP); 
        }
    }

    // Scales stats based on the global difficulty setting
    protected virtual void ApplyDifficultyScaling()
    {
        if (DifficultyManager.Instance == null) return;

        // Apply multipliers from the DifficultyManager singleton
        attackDamage = Mathf.RoundToInt(
            attackDamage * DifficultyManager.Instance.enemyDamageMultiplier
        );

        Health.SetMaxHP(
            Mathf.RoundToInt(Health.MaxHP * DifficultyManager.Instance.enemyHealthMultiplier)
        );
    }

    // Initialises the enemy state and applies the scaling hierarchy
    public void Initialize(BattleManager bm)
    {
        battleManager = bm;
        isHardMode = DifficultyManager.Instance != null && DifficultyManager.Instance.currentDifficulty == Difficulty.Hard;

        // Apply randomization before difficulty scaling for consistent results
        ApplyRandomization();
        ApplyDifficultyScaling();
        
        OnInitialize();
    }

    protected virtual void OnInitialize() { }

    // Core turn logic to be implemented by specific enemy types
    public abstract IEnumerator ExecuteTurn(List<BattleCharacter> party);

    // AI decision-making phase called at the start of each round
    public abstract void PlanNextAction(List<BattleCharacter> party);

    // Determines the optimal target based on specific AI strategies
    protected BattleCharacter PickTarget(List<BattleCharacter> party, TargetStrategy strategy)
    {
        BattleCharacter target = null;

        // Prioritise units currently using a Taunt card
        foreach (var ch in party)
            if (ch.isTaunting && ch.Health.CurrentHP > 0) { target = ch; return target; }

        switch (strategy)
        {
            case TargetStrategy.LowestHP:
                // Logic to target the most vulnerable party member
                int lowestHP = int.MaxValue;
                foreach (var ch in party)
                {
                    if (ch.Health.CurrentHP > 0 && ch.Health.CurrentHP < lowestHP)
                    {
                        lowestHP = ch.Health.CurrentHP;
                        target = ch;
                    }
                }
                break;

            case TargetStrategy.HighestHP:
                // Logic to target the healthiest party member
                int highestHP = 0;
                foreach (var ch in party)
                {
                    if (ch.Health.CurrentHP > 0 && ch.Health.CurrentHP > highestHP)
                    {
                        highestHP = ch.Health.CurrentHP;
                        target = ch;
                    }
                }
                break;

            case TargetStrategy.Random:
                // Selects a random living party member
                var aliveParty = new List<BattleCharacter>();
                foreach (var ch in party)
                    if (ch.Health.CurrentHP > 0) aliveParty.Add(ch);
                if (aliveParty.Count > 0)
                    target = aliveParty[Random.Range(0, aliveParty.Count)];
                break;
        }

        return target;
    }

    protected enum TargetStrategy { LowestHP, HighestHP, Random }
}