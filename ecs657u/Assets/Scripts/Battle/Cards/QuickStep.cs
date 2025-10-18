using UnityEngine;
[CreateAssetMenu(menuName="Cards/Quickstep (+Agi)")]
public class BuffAgilityCard : CardBase
{
    public int Turns = 2;
    private void OnEnable(){ School = CardSchool.Support; Title="Quickstep"; Description="+Agility for a short time."; }
    public override void Play(BattleContext ctx)
    {
        var t = ctx.Target ?? ctx.Actor;
        t.agiBuffTurns = Turns;
        ctx.BM.RefreshNameplates();
    }
}