using UnityEngine;
[CreateAssetMenu(menuName="Cards/Block")]
public class BlockCard : CardBase
{
    public int BaseShield = 4;
    private void OnEnable(){ School = CardSchool.Support; Title="Block"; Description="Gain shield."; }
    public override void Play(BattleContext ctx)
    {
        (ctx.Target ?? ctx.Actor).AddShield(Scale(ctx.Actor, BaseShield));
        ctx.BM.RefreshNameplates();
    }
}
