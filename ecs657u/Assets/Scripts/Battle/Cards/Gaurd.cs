using UnityEngine;
[CreateAssetMenu(menuName="Cards/Guard (+Def)")]
public class BuffDefenseCard : CardBase
{
    public int Turns = 2;
    public int ShieldFlat = 3;
    private void OnEnable(){ School = CardSchool.Support; Title="Guard"; Description="Harden defenses briefly."; }
    public override void Play(BattleContext ctx)
    {
        var t = ctx.Target ?? ctx.Actor;
        t.defBuffTurns = Turns;
        t.AddShield(ShieldFlat);
        ctx.BM.RefreshNameplates();
    }
}