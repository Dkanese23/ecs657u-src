using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyShaman : EnemyBase
{
    [Header("Shaman Settings")]
    public int healAmount = 10;
    public int magicDamage = 6;
    public int buffAmount = 3;

    private int attackBuff = 0;
    private int turnsSinceHeal = 0;

    public override void PlanNextAction(List<BattleCharacter> party)
    {
        turnsSinceHeal++;
        float hpPercent = (Health.CurrentHP / (float)Health.MaxHP) * 100f;

        // Heal if below 50% HP and haven't healed in 2+ turns
        if (hpPercent < 50f && turnsSinceHeal >= 2)
        {
            nextAction = "Heal";
            actionValue = healAmount;
        }
        // Buff if not buffed yet
        else if (attackBuff == 0 && Random.value < 0.4f)
        {
            nextAction = "Power Up";
            actionValue = buffAmount;
        }
        // Magic attack on lowest HP target
        else
        {
            nextAction = "Dark Bolt";
            actionValue = magicDamage + attackBuff;
        }
    }

    public override IEnumerator ExecuteTurn(List<BattleCharacter> party)
    {
        if (nextAction == "Heal")
        {
            battleManager.LogAction($"{enemyName} casts Heal!");
            Health.Heal(healAmount);
            turnsSinceHeal = 0;
        }
        else if (nextAction == "Power Up")
        {
            battleManager.LogAction($"{enemyName} powers up! Attack increased by {buffAmount}!");
            attackBuff = buffAmount;
        }
        else // Dark Bolt
        {
            var target = PickTarget(party, TargetStrategy.LowestHP);
            if (target != null)
            {
                battleManager.LogAction($"{enemyName} casts Dark Bolt on {target.displayName}!");
                target.ReceiveDamage(magicDamage + attackBuff);
            }
        }

        yield return new WaitForSeconds(0.5f);
    }
}
