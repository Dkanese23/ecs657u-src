using UnityEngine;
[CreateAssetMenu(menuName="Cards/Magic Bolt")]
public class MagicBoltCard : CardBase
{
    public int BaseDamage = 4;
    private void OnEnable(){ School = CardSchool.Magic; Title="Magic Bolt"; Description="Magic damage."; }
    public override void Play(BattleContext ctx)
    {
        int dmg = Scale(ctx.Actor, BaseDamage);
        ctx.BM.DamageEnemy(dmg);
    }
}