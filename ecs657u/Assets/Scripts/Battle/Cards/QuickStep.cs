using UnityEngine;

// ScriptableObject for a support card that provides a temporary attribute boost
[CreateAssetMenu(menuName="Cards/Quickstep (+Agi)")]
public class BuffAgilityCard : CardBase
{
    public int Turns = 2;

    // Initialises card metadata and categorises it within the Support school
    private void OnEnable()
    { 
        School = CardSchool.Support; 
        Title = "Quickstep"; 
        Description = "+Agility for a short time."; 
    }

    // Executes the stat-boosting logic during the combat phase
    public override void Play(BattleContext ctx)
    {
        // Identifies the recipient, defaulting to the caster if no target is specified
        var t = ctx.Target ?? ctx.Actor;

        // Assigns the duration for the agility buff to be processed by the character logic
        t.agiBuffTurns = Turns;

        // Refreshes UI nameplates to ensure the new status effect is visible to the player
        ctx.BM.RefreshNameplates();
    }
}