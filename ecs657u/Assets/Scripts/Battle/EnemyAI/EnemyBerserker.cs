using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Specific enemy implementation with conditional state-based AI
public class EnemyBerserker : EnemyBase
{
    [Header("Berserker Settings")]
    public int heavyAttackDamage = 10;
    public int enrageThreshold = 30; // HP percentage required to trigger enrage state

    private bool isEnraged = false;

    // Determines the enemy's intent for the upcoming round
    public override void PlanNextAction(List<BattleCharacter> party)
    {
        // Calculate current health percentage for state checking
        float hpPercent = (Health.CurrentHP / (float)Health.MaxHP) * 100f;
        isEnraged = hpPercent <= enrageThreshold;

        if (isEnraged)
        {
            // Enraged state: Prioritises high-damage output
            nextAction = "Heavy Strike";
            actionValue = heavyAttackDamage + 2;
        }
        else if (Random.value < 0.3f)
        {
            // Weighted probability for heavy attacks when healthy
            nextAction = "Heavy Strike";
            actionValue = heavyAttackDamage;
        }
        else
        {
            // Standard offensive action
            nextAction = "Attack";
            actionValue = attackDamage;
        }
    }

    // Processes the visual and logical execution of the chosen action
    public override IEnumerator ExecuteTurn(List<BattleCharacter> party)
    {
        // Berserker strategy: Target the healthiest player to even the field
        var target = PickTarget(party, TargetStrategy.HighestHP);
        if (target == null) yield break;

        if (nextAction == "Heavy Strike")
        {
            int damage = isEnraged ? heavyAttackDamage + 2 : heavyAttackDamage;
            battleManager.LogAction($"{enemyName} uses Heavy Strike on {target.displayName}!");
            
            // Trigger combat animations for visual feedback
            GetComponent<BattleAnim>()?.PlayAttack();
            yield return new WaitForSeconds(0.2f);
            
            target.ReceiveDamage(damage);
        }
        else
        {
            battleManager.LogAction($"{enemyName} attacks {target.displayName}!");
            target.ReceiveDamage(attackDamage);
        }

        // Pacing delay to ensure the player can follow the combat log
        yield return new WaitForSeconds(0.5f);
    }
}