using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Shaman AI class that utilises support magic and strategic targeting
public class EnemyShaman : EnemyBase
{
    [Header("Shaman Settings")]
    public int healAmount = 10;
    public int magicDamage = 8;
    public int buffAmount = 3;

    private int attackBuff = 0;
    private int turnsSinceHeal = 0;

    // Evaluates the current battle state to decide between healing, buffing, or attacking
    public override void PlanNextAction(List<BattleCharacter> party)
    {
        turnsSinceHeal++;
        float hpPercent = (Health.CurrentHP / (float)Health.MaxHP) * 100f;

        // Reactive Logic: Priority is given to healing if health is low and cooldown has passed
        if (hpPercent < 50f && turnsSinceHeal >= 2)
        {
            nextAction = "Heal";
            actionValue = healAmount;
        }
        // Setup Logic: Increases offensive capability if not currently buffed
        else if (attackBuff == 0 && Random.value < (isHardMode ? 0.7f : 0.4f))
        {
            nextAction = "Power Up";
            actionValue = buffAmount;
        }
        // Offensive Logic: Executes a magical strike
        else
        {
            nextAction = "Dark Bolt";
            actionValue = magicDamage + attackBuff;
        }
    }

    // Handles the execution of the Shaman's mystical abilities
    public override IEnumerator ExecuteTurn(List<BattleCharacter> party)
    {
        if (nextAction == "Heal")
        {
            battleManager.LogAction($"{enemyName} casts Heal!");
            Health.Heal(healAmount);
            turnsSinceHeal = 0;
            GetComponent<BattleAnim>()?.PlayAttack();
        }
        else if (nextAction == "Power Up")
        {
            battleManager.LogAction($"{enemyName} powers up! Attack increased by {buffAmount}!");
            attackBuff = buffAmount;
            GetComponent<BattleAnim>()?.PlayAttack();
        }
        else // Dark Bolt
        {
            // Opportunistic AI: Selects the most vulnerable party member
            var target = PickTarget(party, TargetStrategy.LowestHP);
            if (target != null)
            {
                battleManager.LogAction($"{enemyName} casts Dark Bolt on {target.displayName}!");
                GetComponent<BattleAnim>()?.PlayAttack();
                yield return new WaitForSeconds(0.2f);
                target.ReceiveDamage(magicDamage + attackBuff);
            }
        }

        // Delay to maintain the turn-based rhythm
        yield return new WaitForSeconds(0.5f);
    }
}