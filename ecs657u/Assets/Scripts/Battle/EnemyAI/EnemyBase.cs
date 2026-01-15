using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Base enemy class
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    public string enemyName = "Enemy";
    public int attackDamage = 3;
    public Health Health { get; private set; }

    [Header("Procedural Generation")]
    public bool randomizeStats = true; // Check this to enable random stats
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

    // NEW: Randomizes stats before difficulty is applied
    protected virtual void ApplyRandomization()
    {
        if (!randomizeStats) return;

        // 1. Calculate a random multiplier (e.g., 0.8 to 1.2)
        float multiplier = Random.Range(1f - variationRange, 1f + variationRange);

        // 2. Randomize Attack
        attackDamage = Mathf.RoundToInt(attackDamage * multiplier);
        if (attackDamage < 1) attackDamage = 1;

        // 3. Randomize Health
        if (Health != null)
        {
            int newMaxHP = Mathf.RoundToInt(Health.MaxHP * multiplier);
            
            // We set MaxHP. Note: Depending on your Health script, 
            // you might need to ensure CurrentHP is also set to full.
            Health.SetMaxHP(newMaxHP); 
            
            // Optional: Refill health to match new Max if SetMaxHP doesn't do it
            // Health.CurrentHP = newMaxHP; 
        }
    }

    protected virtual void ApplyDifficultyScaling()
    {
        // Safety check 
        if (DifficultyManager.Instance == null)
            return;

        // Scale enemy attack damage
        attackDamage = Mathf.RoundToInt(
            attackDamage * DifficultyManager.Instance.enemyDamageMultiplier
        );

        // Scale enemy max HP
        Health.SetMaxHP(
            Mathf.RoundToInt(Health.MaxHP * DifficultyManager.Instance.enemyHealthMultiplier)
        );
    }

    public void Initialize(BattleManager bm)
    {
        battleManager = bm;

        isHardMode = DifficultyManager.Instance != null && DifficultyManager.Instance.currentDifficulty == Difficulty.Hard;

        // ORDER MATTERS:
        // 1. Randomize the "base" monster stats first
        ApplyRandomization();

        // 2. Then multiply by difficulty (so Hard mode multiplies the randomized result)
        ApplyDifficultyScaling();
        
        OnInitialize();
    }


    protected virtual void OnInitialize() { }

    // Each enemy decides what to do on their turn
    public abstract IEnumerator ExecuteTurn(List<BattleCharacter> party);

    // Called at start of each round to decide next action
    public abstract void PlanNextAction(List<BattleCharacter> party);

    // Get target based on enemy strategy
    protected BattleCharacter PickTarget(List<BattleCharacter> party, TargetStrategy strategy)
    {
        BattleCharacter target = null;

        // Check for taunt first (overrides strategy)
        foreach (var ch in party)
            if (ch.isTaunting && ch.Health.CurrentHP > 0) { target = ch; return target; }

        switch (strategy)
        {
            case TargetStrategy.LowestHP:
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