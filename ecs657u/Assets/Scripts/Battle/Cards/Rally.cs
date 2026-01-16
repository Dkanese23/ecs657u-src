using UnityEngine;

// ScriptableObject for a support card that enhances an ally's offensive stats
[CreateAssetMenu(menuName="Cards/Rally (+Atk)")]
public class BuffAttackCard : CardBase
{
    public int Turns = 2;
    public int FlatBonus = 2; // Fixed value added to damage calculations during the buff period

    // Initialises card metadata for the support school and UI display
    private void OnEnable()
    { 
        School = CardSchool.Support; 
        Title = "Rally"; 
        Description = "Boost offense temporarily."; 
    }

    // Executes the offensive buff logic on the chosen target
    public override void Play(BattleContext ctx)
    {
        // Selects the recipient, defaulting to the caster if no specific target is provided
        var t = ctx.Target ?? ctx.Actor;

        // Sets the duration for the attack buff within the character's state logic
        t.atkBuffTurns = Turns;

        // Registers a flat damage bonus with the Battle Manager for calculation during turns
        ctx.BM.TagFlatAttackBonus(t, FlatBonus, Turns);
    }
}