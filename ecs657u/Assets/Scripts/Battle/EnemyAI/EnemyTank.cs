using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Specialized enemy subclass focused on high survivability and area-of-effect attacks
public class EnemyTank : EnemyBase
{
    [Header("Tank Settings")]
    public int aoeDamage = 6;
    public int defensiveBuff = 5;
    public int chargeAttackDamage = 10;

    private bool isDefending = false;
    private int chargeCounter = 0; // Tracks turns elapsed to trigger a powerful ability

    // Initialises the tank with enhanced health pools to reflect its role
    protected override void OnInitialize()
    {
        // Tank role specific stat adjustment
        Health.Heal(20);
    }

    // Evaluates battle state to cycle between area attacks and defensive stances
    public override void PlanNextAction(List<BattleCharacter> party)
    {
        chargeCounter++;

        float hpPercent = (Health.CurrentHP / (float)Health.MaxHP) * 100f;

        // Reactive AI: Switches to a defensive stance when health is critically low
        if (hpPercent < 40f && !isDefending)
        {
            nextAction = "Defensive Stance";
            actionValue = defensiveBuff;
        }
        // Predictable Threat: Executes a high-damage strike on a set frequency
        else if (chargeCounter >= (isHardMode ? 2 : 3))
        {
            nextAction = "Charge Attack";
            actionValue = chargeAttackDamage;
        }
        // Standard AOE: Constant pressure on the player's entire party
        else
        {
            nextAction = "Ground Slam";
            actionValue = aoeDamage;
        }
    }

    // Handles the execution of sequences, including AoE damage loops
    public override IEnumerator ExecuteTurn(List<BattleCharacter> party)
    {
        if (nextAction == "Defensive Stance")
        {
            battleManager.LogAction($"{enemyName} takes a Defensive Stance!");
            isDefending = true;
        }
        else if (nextAction == "Charge Attack")
        {
            var target = PickTarget(party, TargetStrategy.Random);
            if (target != null)
            {
                battleManager.LogAction($"{enemyName} unleashes a Charge Attack on {target.displayName}!");
                GetComponent<BattleAnim>()?.PlayAttack();
                yield return new WaitForSeconds(0.2f);
                target.ReceiveDamage(chargeAttackDamage);
                chargeCounter = 0;
            }
        }
        else // Ground Slam (AOE)
        {
            battleManager.LogAction($"{enemyName} uses Ground Slam! All party members take {aoeDamage} damage!");
            
            // Iterates through the party to apply damage to all living members
            foreach (var ch in party)
            {
                if (ch.Health.CurrentHP > 0)
                {
                    GetComponent<BattleAnim>()?.PlayAttack();
                    yield return new WaitForSeconds(0.2f);
                    ch.ReceiveDamage(aoeDamage);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    // Logic override to reduce incoming damage while the defensive stance is active
    public void ReceiveDamage(int amount)
    {
        if (isDefending)
        {
            amount = Mathf.Max(1, amount - defensiveBuff);
            battleManager.LogAction($"{enemyName}'s defense reduces damage to {amount}!");
            isDefending = false; // The stance is consumed upon taking damage
        }
        Health.TakeDamage(amount);
    }
}