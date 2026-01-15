using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDragonBoss : EnemyBase
{
    [Header("Dragon Settings")]
    public int clawDamage = 9;
    public int fireBreathDamage = 6;     // applied to ALL party members
    public int wingBuffetDamage = 4;     // applied to ALL party members
    public int recoverHeal = 12;

    [Header("Boss Logic")]
    [Range(0,100)] public float enrageAtHpPercent = 35f;
    public int enrageBonusDamage = 4;

    public int fireBreathCooldownTurns = 3;
    public int wingBuffetCooldownTurns = 2;
    public int recoverCooldownTurns = 3;

    int bonusDamage = 0;
    bool enraged = false;

    int fireBreathCD = 0;
    int wingBuffetCD = 0;
    int recoverCD = 0;

    public override void PlanNextAction(List<BattleCharacter> party)
    {
        // tick cooldowns
        if (fireBreathCD > 0) fireBreathCD--;
        if (wingBuffetCD > 0) wingBuffetCD--;
        if (recoverCD > 0) recoverCD--;

        float hpPercent = (Health.CurrentHP / (float)Health.MaxHP) * 100f;

        // Enrage check
        if (!enraged && hpPercent <= enrageAtHpPercent)
        {
            nextAction = "Enrage";
            actionValue = enrageBonusDamage;
            return;
        }

        // If low-ish HP: sometimes recover (with cooldown)
        if (hpPercent < 40f && recoverCD == 0 && Random.value < (isHardMode ? 0.75f : 0.5f))
        {
            nextAction = "Recover";
            actionValue = recoverHeal;
            return;
        }

        // Fire Breath: big move, AoE, on cooldown
        if (fireBreathCD == 0 && Random.value < (isHardMode ? 0.75f : 0.55f))
        {
            nextAction = "Fire Breath";
            actionValue = fireBreathDamage + bonusDamage;
            return;
        }

        // Wing Buffet: smaller AoE, more frequent
        if (wingBuffetCD == 0 && Random.value < (isHardMode ? 0.70f : 0.45f))
        {
            nextAction = "Wing Buffet";
            actionValue = wingBuffetDamage + bonusDamage;
            return;
        }

        // Default: Claw Swipe (single target)
        nextAction = "Claw Swipe";
        actionValue = clawDamage + bonusDamage;
    }

    public override IEnumerator ExecuteTurn(List<BattleCharacter> party)
    {
        // Basic wind-up feel
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
            // Boss-y choice: hit highest HP to pressure the tanky one
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
