using UnityEngine;

[CreateAssetMenu(menuName="Cards/Heal")]
public class HealCard : CardBase
{
    public int BaseHeal = 5;

    private void OnEnable()
    {
        School = CardSchool.Support;
        Title = "Heal";
        Description = "Restore HP to an ally.";
    }

    public override TargetingType Targeting => TargetingType.SelfOrAlly;

    public override void Play(BattleContext ctx)
    {
        var target = ctx.Target ?? ctx.Actor; // fallback to self if no target
        int amount = Scale(ctx.Actor, BaseHeal);

        target.Health.Heal(amount);
        ctx.BM.LogHeal(ctx.Actor.displayName, target.displayName, amount);
        
    }
}
