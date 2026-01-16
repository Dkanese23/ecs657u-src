using UnityEngine;

// ScriptableObject for a utility card that removes debuffs and status effects
[CreateAssetMenu(menuName="Cards/Cleanse")]
public class CleanseCard : CardBase
{
    // Initialises card metadata for the Shamanic support role
    private void OnEnable()
    { 
        School = CardSchool.Support; 
        Title = "Cleanse"; 
        Description = "Remove negative effects."; 
    }

    // Executes the purification logic on the actor or selected target
    public override void Play(BattleContext ctx)
    {
        var t = ctx.Target ?? ctx.Actor;

        // Reset status-related variables to clear the unit's negative states
        t.isTaunting = false; 
        t.tauntTurns = 0;

        // Update the UI to reflect the removal of status icons
        ctx.BM.RefreshNameplates();
    }
}