using UnityEngine;

// ScriptableObject defining a support card that provides defensive shielding
[CreateAssetMenu(menuName="Cards/Block")]
public class BlockCard : CardBase
{
    public int BaseShield = 4; // Base protection value before stat scaling is applied

    // Initialises card metadata for the user interface
    private void OnEnable()
    { 
        School = CardSchool.Support; 
        Title = "Block"; 
        Description = "Gain shield."; 
    }

    // Performs the defensive logic during the battle phase
    public override void Play(BattleContext ctx)
    {
        // Applies scaled shield to the target or the caster as a fallback
        (ctx.Target ?? ctx.Actor).AddShield(Scale(ctx.Actor, BaseShield));

        // Refreshes the UI nameplates to ensure the new shield values are visible
        ctx.BM.RefreshNameplates();
    }
}