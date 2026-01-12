
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Base enemy class - replace your EnemySimple with this
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    public string enemyName = "Enemy";
    public int attackDamage = 3;
    public Health Health { get; private set; }

    [Header("Difficulty State")]
    protected bool isHardMode;

    [Header("AI Behavior")]
    public string nextAction = ""; // What enemy will do next turn
    public int actionValue = 0;    // Damage/heal amount for display

    protected BattleManager battleManager;

    protected virtual void Awake()
    {
        Health = GetComponent<Health>();
    }

    protected virtual void ApplyDifficultyScaling()
    {
    // Safety check (in case manager isn't loaded)
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

    isHardMode = DifficultyManager.Instance.currentDifficulty == Difficulty.Hard;

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