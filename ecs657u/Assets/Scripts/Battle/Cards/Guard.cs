using UnityEngine;

// ScriptableObject for a support card that buffs an ally's defensive capabilities
[CreateAssetMenu(menuName="Cards/Guard (+Def to Ally)")]
public class BuffDefenseCard : CardBase
{
    public int Turns = 2;
    public int BaseShield = 4;

    // Initialises card metadata for the support school
    private void OnEnable()
    {
        School = CardSchool.Support;
        Title = "Guard";
        Description = "Grant an ally shield.";
    }

    // Overrides default targeting to restrict usage to allied units only
    public override CardBase.TargetingType Targeting => CardBase.TargetingType.Ally;

    // Executes the defensive buff logic during the combat phase
    public override void Play(BattleContext ctx)
    {
        var actor  = ctx.Actor;                 // The unit initiating the buff
        var target = ctx.Target;                // The specific ally selected via the UI

        // Scale the shield value based on the caster's current attributes
        int shieldAmt = Scale(actor, BaseShield);

        // Apply the turn-based buff and immediate shield protection
        target.defBuffTurns = Turns;
        target.AddShield(shieldAmt);

        // Update UI nameplates and combat log for player feedback
        ctx.BM.RefreshNameplates();
        ctx.BM.LogBuff(actor.displayName, target.displayName, $"{Title} (+{shieldAmt} Shield for {Turns} turns)");
    }
}