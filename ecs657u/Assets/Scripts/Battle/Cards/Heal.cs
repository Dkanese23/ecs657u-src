using UnityEngine;
[CreateAssetMenu(menuName="Cards/Heal")]
public class HealCard : CardBase
{
    public int BaseHeal = 5;
    private void OnEnable(){ School = CardSchool.Support; Title="Heal"; Description="Restore HP to an ally."; }
    public override void Play(BattleContext ctx)
    {
        (ctx.Target ?? ctx.Actor).Health.Heal(Scale(ctx.Actor, BaseHeal));
    }
}