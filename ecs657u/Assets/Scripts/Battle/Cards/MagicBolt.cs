using UnityEngine;

// ScriptableObject for a magic-based offensive card using intelligence scaling
[CreateAssetMenu(menuName="Cards/Magic Bolt")]
public class MagicBoltCard : CardBase
{
    public int BaseDamage = 4;

    // Initialises the card's metadata and categorises it under the Magic school
    private void OnEnable()
    { 
        School = CardSchool.Magic; 
        Title = "Magic Bolt"; 
        Description = "Magic damage."; 
    }

    // Executes the combat logic for a magical projectile attack
    public override void Play(BattleContext ctx)
    {
        // Triggers the magical attack animation on the acting unit
        ctx.Actor.GetComponent<BattleAnim>()?.PlayAttack();

        // Calculates damage based on the actor's Intelligence attribute
        int dmg = Scale(ctx.Actor, BaseDamage);

        // Applies the final damage to the target enemy
        ctx.BM.DamageEnemy(dmg);

        // Logs the interaction for clear player feedback in the UI
        ctx.BM.LogDamage(ctx.Actor.displayName, ctx.Enemy.enemyName, dmg);
    }
}