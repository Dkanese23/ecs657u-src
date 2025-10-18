using UnityEngine;
[CreateAssetMenu(menuName="Cards/Taunt")]
public class TauntCard : CardBase
{
    public int Turns = 2;
    private void OnEnable(){ School = CardSchool.Support; Title="Taunt"; Description="Force enemy to target you."; }
    public override void Play(BattleContext ctx)
    {
        ctx.Actor.isTaunting = true;
        ctx.Actor.tauntTurns = Turns;
        ctx.BM.RefreshNameplates();
    }
}