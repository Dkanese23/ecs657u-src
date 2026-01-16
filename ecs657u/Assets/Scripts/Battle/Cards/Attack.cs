using UnityEngine;

// ScriptableObject for a basic physical attack card
[CreateAssetMenu(menuName="Cards/Attack")]
public class AttackCard : CardBase
{
    public int BaseDamage = 3; // Base damage before scaling

    // Initialize card metadata when loaded
    private void OnEnable()
    {
        School = CardSchool.Physical;
        Title = "Strike";
        Description = "Physical attack.";
    }

    // Executes the card's effect in battle
    public override void Play(BattleContext ctx)
    {
        // Play attack animation on the acting unit
        ctx.Actor.GetComponent<BattleAnim>()?.PlayAttack();

        // Calculate final damage with scaling
        int dmg = Scale(ctx.Actor, BaseDamage);

        // Apply damage and log the result
        ctx.BM.DamageEnemy(dmg);
        ctx.BM.LogDamage(ctx.Actor.displayName, ctx.Enemy.enemyName, dmg);
    }
}
