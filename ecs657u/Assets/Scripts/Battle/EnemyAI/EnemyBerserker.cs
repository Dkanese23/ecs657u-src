using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBerserker : EnemyBase
{
    [Header("Berserker Settings")]
    public int heavyAttackDamage = 10;
    public int enrageThreshold = 30; // HP % to enrage

    private bool isEnraged = false;

    public override void PlanNextAction(List<BattleCharacter> party)
    {
        float hpPercent = (Health.CurrentHP / (float)Health.MaxHP) * 100f;
        isEnraged = hpPercent <= enrageThreshold;

        if (isEnraged)
        {
            // Enraged: always heavy attack
            nextAction = "Heavy Strike";
            actionValue = heavyAttackDamage + 2;
        }
        else if (Random.value < 0.3f)
        {
            // 30% chance for heavy attack when not enraged
            nextAction = "Heavy Strike";
            actionValue = heavyAttackDamage;
        }
        else
        {
            // Normal attack
            nextAction = "Attack";
            actionValue = attackDamage;
        }
    }

    public override IEnumerator ExecuteTurn(List<BattleCharacter> party)
    {
        var target = PickTarget(party, TargetStrategy.HighestHP);
        if (target == null) yield break;

        if (nextAction == "Heavy Strike")
        {
            int damage = isEnraged ? heavyAttackDamage + 2 : heavyAttackDamage;
            battleManager.LogAction($"{enemyName} uses Heavy Strike on {target.displayName}!");
            // play attack animation
            GetComponent<BattleAnim>()?.PlayAttack();
            yield return new WaitForSeconds(0.2f);
            target.ReceiveDamage(damage);

        }
        else
        {
            battleManager.LogAction($"{enemyName} attacks {target.displayName}!");
            target.ReceiveDamage(attackDamage);
        }

        yield return new WaitForSeconds(0.5f);
    }
}