using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Complex boss AI featuring AoE attacks, cooldown management, and state transitions
public class EnemyDragonBoss : EnemyBase
{
    [Header("Dragon Settings")]
    public int clawDamage = 9;
    public int fireBreathDamage = 6;     // Targeted at all active party members
    public int wingBuffetDamage = 4;     // Targeted at all active party members
    public int recoverHeal = 12;

    [Header("Boss Logic")]
    [Range(0, 100)] public float enrageAtHpPercent = 35f;
    public int enrageBonusDamage = 4;

    public int fireBreathCooldownTurns = 3;
    public int wingBuffetCooldownTurns = 2;
    public int recoverCooldownTurns = 3;

    int bonusDamage = 0;
    bool enraged = false;

    int fireBreathCD = 0;
    int wingBuffetCD = 0;
    int recoverCD = 0;

    // Evaluates the combat state and selects the most impactful ability
    public override void PlanNextAction(List<BattleCharacter> party)
    {
        // Reduce ability cooldowns at the start of the planning phase
        if (fireBreathCD > 0) fireBreathCD--;
        if (wingBuffetCD > 0) wingBuffetCD--;
        if (recoverCD > 0) recoverCD--;

        float hpPercent = (Health.CurrentHP / (float)Health.MaxHP) * 100f;

        // Transition to Enraged state if health falls below the threshold
        if (!enraged && hpPercent <= enrageAtHpPercent)
        {
            nextAction = "Enrage";
            actionValue = enrageBonusDamage;
            return;
        }

        // Recovery logic: Weighted by difficulty to increase boss survivability
        if (hpPercent < 40f && recoverCD == 0 && Random.value < (isHardMode ? 0.75f : 0.5f))
        {
            nextAction = "Recover";
            actionValue = recoverHeal;
            return;
        }

        // High-damage AoE: Prioritised when available
        if (fireBreathCD == 0 && Random.value < (isHardMode ? 0.75f : 0.55f))
        {
            nextAction = "Fire Breath";
            actionValue = fireBreathDamage + bonusDamage;
            return;
        }

        // Utility AoE: Frequent disruption of the player party
        if (wingBuffetCD == 0 && Random.value < (isHardMode ? 0.70f : 0.45f))
        {
            nextAction = "Wing Buffet";
            actionValue = wingBuffetDamage + bonusDamage;
            return;
        }

        // Standard single-target attack when abilities are on cooldown
        nextAction = "Claw Swipe";
        actionValue = clawDamage + bonusDamage;
    }

    // Handles the execution of sequences and visual feedback for the boss
    public override IEnumerator ExecuteTurn(List<BattleCharacter> party)
    {
        yield return new WaitForSeconds(0.15f);

        if (nextAction == "Enrage")
        {
            battleManager.LogAction($"{enemyName} roars and becomes enraged!");
            bonusDamage += enrageBonusDamage;
            enraged = true;
            GetComponent<BattleAnim>()?.PlayAttack();
            yield return new WaitForSeconds(0.35f);
        }
        else if (nextAction == "Recover")
        {
            battleManager.LogAction($"{enemyName} gathers strength and recovers!");
            GetComponent<BattleAnim>()?.PlayAttack();
            yield return new WaitForSeconds(0.25f);
            Health.Heal(recoverHeal);
            recoverCD = recoverCooldownTurns;
        }
        else if (nextAction == "Fire Breath")
        {
            battleManager.LogAction($"{enemyName} unleashes Fire Breath!");
            GetComponent<BattleAnim>()?.PlayAttack();
            yield return new WaitForSeconds(0.25f);

            // Apply damage to all living party members
            foreach (var ch in party)
            {
                if (ch != null && ch.Health.CurrentHP > 0)
                    ch.ReceiveDamage(fireBreathDamage + bonusDamage);
            }
            fireBreathCD = fireBreathCooldownTurns;
        }
        else if (nextAction == "Wing Buffet")
        {
            battleManager.LogAction($"{enemyName} blasts the party with a Wing Buffet!");
            GetComponent<BattleAnim>()?.PlayAttack();
            yield return new WaitForSeconds(0.25f);

            foreach (var ch in party)
            {
                if (ch != null && ch.Health.CurrentHP > 0)
                    ch.ReceiveDamage(wingBuffetDamage + bonusDamage);
            }
            wingBuffetCD = wingBuffetCooldownTurns;
        }
        else // Claw Swipe
        {
            // Focuses pressure on the tankiest party member
            var target = PickTarget(party, TargetStrategy.HighestHP);
            if (target != null)
            {
                battleManager.LogAction($"{enemyName} slashes {target.displayName} with a Claw Swipe!");
                GetComponent<BattleAnim>()?.PlayAttack();
                yield return new WaitForSeconds(0.2f);
                target.ReceiveDamage(clawDamage + bonusDamage);
            }
        }

        yield return new WaitForSeconds(0.5f);
    }
}