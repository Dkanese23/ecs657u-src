using UnityEngine;
[CreateAssetMenu(menuName="Cards/Block")]
public class BlockCard : CardBase
{
    public int BaseShield = 5;
    private void OnEnable(){ School = CardSchool.Support; Title="Block"; Description="Gain shield (absorbs damage)."; }
    public override void Play(BattleContext ctx)
    {
        (ctx.Target ?? ctx.Actor).AddShield(Scale(ctx.Actor, BaseShield));
        ctx.BM.RefreshNameplates();
    }
}
