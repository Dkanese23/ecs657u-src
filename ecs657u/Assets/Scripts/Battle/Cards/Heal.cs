using UnityEngine;

// ScriptableObject defining a restorative support card
[CreateAssetMenu(menuName="Cards/Heal")]
public class HealCard : CardBase
{
    public int BaseHeal = 5;

    // Initialises card metadata for the support school
    private void OnEnable()
    {
        School = CardSchool.Support;
        Title = "Heal";
        Description = "Restore HP to an ally.";
    }

    // Allows the player to target themselves or a friendly unit
    public override TargetingType Targeting => TargetingType.SelfOrAlly;

    // Executes the healing logic during the battle phase
    public override void Play(BattleContext ctx)
    {
        // Identifies the recipient, defaulting to the caster if no target is specified
        var target = ctx.Target ?? ctx.Actor; 
        
        // Calculates the recovery amount modified by the caster's attributes
        int amount = Scale(ctx.Actor, BaseHeal);

        // Applies the healing to the target's health component
        target.Health.Heal(amount);

        // Outputs the action to the combat log for clear player feedback
        ctx.BM.LogHeal(ctx.Actor.displayName, target.displayName, amount);
    }
}