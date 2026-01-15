using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyTank : EnemyBase
{
    [Header("Tank Settings")]
    public int aoeDamage = 6;
    public int defensiveBuff = 5;
    public int chargeAttackDamage = 10;

    private bool isDefending = false;
    private int chargeCounter = 0; // Charges up every 3 turns

    protected override void OnInitialize()
    {
        // Tank has bonus HP
        Health.Heal(20);
    }

    public override void PlanNextAction(List<BattleCharacter> party)
    {
        chargeCounter++;

        float hpPercent = (Health.CurrentHP / (float)Health.MaxHP) * 100f;

        // Defensive stance when low HP
        if (hpPercent < 40f && !isDefending)
        {
            nextAction = "Defensive Stance";
            actionValue = defensiveBuff;
        }
        // Charged attack every 3 turns
        else if (chargeCounter >= (isHardMode ? 2 : 3))
        {
            nextAction = "Charge Attack";
            actionValue = chargeAttackDamage;
        }
        // AOE attack
        else
        {
            nextAction = "Ground Slam";
            actionValue = aoeDamage;
        }
    }

    public override IEnumerator ExecuteTurn(List<BattleCharacter> party)
    {
        if (nextAction == "Defensive Stance")
        {
            battleManager.LogAction($"{enemyName} takes a Defensive Stance!");
            isDefending = true;
            // You could reduce damage taken here or add shield
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

    // Optional: Override damage received if defending
    public void ReceiveDamage(int amount)
    {
        if (isDefending)
        {
            amount = Mathf.Max(1, amount - defensiveBuff);
            battleManager.LogAction($"{enemyName}'s defense reduces damage to {amount}!");
            isDefending = false; // Defense lasts one hit
        }
        Health.TakeDamage(amount);
    }
}