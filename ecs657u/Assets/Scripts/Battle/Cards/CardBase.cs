using UnityEngine;

public enum CardSchool { Physical, Support, Magic }

public abstract class CardBase : ScriptableObject
{
    public string Title = "Card";
    [TextArea] public string Description;
    public CardSchool School = CardSchool.Physical;

    // MVP: energy-free; you can add cost later
    public abstract void Play(BattleContext ctx);

    // Simple scaling helpers
    protected int Scale(BattleCharacter actor, int baseAmount)
    {
        switch (School)
        {
            case CardSchool.Physical: return baseAmount + actor.baseAttack + Mathf.RoundToInt(actor.Strength * 0.8f);
            case CardSchool.Support:  return baseAmount + Mathf.RoundToInt(actor.Agility * 0.8f);
            case CardSchool.Magic:    return baseAmount + Mathf.RoundToInt(actor.Intelligence * 0.9f);
            default: return baseAmount;
        }
    }
}

public class BattleContext
{
    public BattleManager BM;
    public BattleCharacter Actor;   // the acting character
    public BattleCharacter Target;  // target ally (for heal/buffs); defaults to Actor
    public EnemySimple Enemy;       // single enemy
}
