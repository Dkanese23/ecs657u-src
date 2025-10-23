using UnityEngine;
[CreateAssetMenu(menuName="Cards/Attack")]
public class AttackCard : CardBase
{
    public int BaseDamage = 3;
    private void OnEnable(){ School = CardSchool.Physical; Title="Strike"; Description="Physical attack."; }
    public override void Play(BattleContext ctx)
    {
        int dmg = Scale(ctx.Actor, BaseDamage);
        ctx.BM.DamageEnemy(dmg);
    }
}
