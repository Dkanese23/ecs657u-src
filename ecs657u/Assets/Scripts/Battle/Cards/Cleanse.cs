using UnityEngine;
[CreateAssetMenu(menuName="Cards/Cleanse")]
public class CleanseCard : CardBase
{
    private void OnEnable(){ School = CardSchool.Support; Title="Cleanse"; Description="Remove negative effects."; }
    public override void Play(BattleContext ctx)
    {
        var t = ctx.Target ?? ctx.Actor;
        // MVP: clear taunt and future debuffs you add
        t.isTaunting = false; t.tauntTurns = 0;
        // (extend: clear poison, slow, etc.)
        ctx.BM.RefreshNameplates();
    }
}