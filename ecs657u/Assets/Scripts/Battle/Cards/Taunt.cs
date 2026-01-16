using UnityEngine;

// ScriptableObject for a utility card that alters enemy targeting logic
[CreateAssetMenu(menuName="Cards/Taunt")]
public class TauntCard : CardBase
{
    public int Turns = 2;

    // Initialises card metadata for the support school and UI
    private void OnEnable()
    { 
        School = CardSchool.Support; 
        Title = "Taunt"; 
        Description = "Force enemy to target you."; 
    }

    // Executes the taunt logic to draw enemy aggression
    public override void Play(BattleContext ctx)
    {
        // Sets the acting unit's state to active taunting
        ctx.Actor.isTaunting = true;
        ctx.Actor.tauntTurns = Turns;

        // Refreshes the UI to display the taunt status icon to the player
        ctx.BM.RefreshNameplates();
    }
}