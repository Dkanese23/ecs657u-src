using UnityEngine;
[CreateAssetMenu(menuName="Cards/Rally (+Atk)")]
public class BuffAttackCard : CardBase
{
    public int Turns = 2;
    public int FlatBonus = 2; // used by your scaling or damage calc later
    private void OnEnable(){ School = CardSchool.Support; Title="Rally"; Description="Boost offense temporarily."; }
    public override void Play(BattleContext ctx)
    {
        var t = ctx.Target ?? ctx.Actor;
        t.atkBuffTurns = Turns;
        // You can apply a flat flag tracked on BM if you want exact math now.
        ctx.BM.TagFlatAttackBonus(t, FlatBonus, Turns);
    }
}