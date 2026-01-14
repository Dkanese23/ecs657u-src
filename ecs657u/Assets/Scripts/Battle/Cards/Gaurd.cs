using UnityEngine;

[CreateAssetMenu(menuName="Cards/Guard (+Def to Ally)")]
public class BuffDefenseCard : CardBase
{
    public int Turns = 2;
    public int BaseShield = 4;

    private void OnEnable()
    {
        School = CardSchool.Support;
        Title = "Guard";
        Description = "Grant an ally shield.";
    }

    // Force selecting another ally (no self)
    public override CardBase.TargetingType Targeting => CardBase.TargetingType.Ally;

    public override void Play(BattleContext ctx)
    {
        var actor  = ctx.Actor;                 // the one playing the card
        var target = ctx.Target;                // must be an ally chosen via nameplate (never null if targeting is enforced)

        // scale shield from the ACTOR's stats (like Block)
        int shieldAmt = Scale(actor, BaseShield);

        target.defBuffTurns = Turns;
        target.AddShield(shieldAmt);

        ctx.BM.RefreshNameplates();
        ctx.BM.LogBuff(actor.displayName, target.displayName, $"{Title} (+{shieldAmt} Shield for {Turns} turns)");
    }
}
