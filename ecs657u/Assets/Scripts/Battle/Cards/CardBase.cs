using UnityEngine;

// Defines the elemental or mechanical category of a card
public enum CardSchool { Physical, Support, Magic }

// Base class for all card assets, using ScriptableObject for data persistence
public abstract class CardBase : ScriptableObject
{
    // Logic for determining how a card interacts with other units
    public enum TargetingType
    {
        None,
        Self,
        Ally,
        SelfOrAlly,
        Enemy
    }

    public string Title = "Card";
    [TextArea] public string Description;
    public CardSchool School = CardSchool.Physical;
    public virtual TargetingType Targeting => TargetingType.None;

    // Abstract method to be implemented by specific card types (e.g. Attack or Block)
    public abstract void Play(BattleContext ctx);

    // Calculates final values by scaling base amounts against character attributes
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

// Data container used to pass battle state information during a turn
public class BattleContext
{
    public BattleManager BM;      // Reference to the main battle controller
    public BattleCharacter Actor;   // The unit currently performing an action
    public BattleCharacter Target;  // The ally receiving an effect; defaults to Actor
    public EnemyBase Enemy;         // The opponent involved in the interaction
}