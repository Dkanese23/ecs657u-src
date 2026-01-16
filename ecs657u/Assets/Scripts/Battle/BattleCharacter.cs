using UnityEngine;

// Core component managing character attributes, status effects, and damage mitigation
public class BattleCharacter : MonoBehaviour
{
    [Header("Identity")]
    public string displayName = "Hero";

    [Header("Core Stats")]
    public int Strength = 5;      // Influences physical damage and shield scaling
    public int Agility = 5;       // Determines turn order and evasion potential
    public int Intelligence = 5;  // Scales magical card effectiveness

    [Header("Derived / Effects")]
    public int baseAttack = 2;          
    public int shield { get; private set; }
    public bool isTaunting;             // Flag to override enemy AI targeting logic
    public int tauntTurns;
    public int atkBuffTurns;            
    public int defBuffTurns;
    public int agiBuffTurns;

    public Health Health { get; private set; }

    void Awake() => Health = GetComponent<Health>();

    // Increases the temporary shield pool to absorb incoming damage
    public void AddShield(int amount) => shield += Mathf.Max(0, amount);

    // Processes incoming damage, prioritising shield depletion before health reduction
    public void ReceiveDamage(int amount)
    {
        // Provide immediate visual feedback for the hit
        GetComponent<BattleAnim>()?.PlayHit();
        
        int left = amount;
        if (shield > 0)
        {
            // Calculate how much damage the shield can absorb
            int consume = Mathf.Min(shield, left);
            shield -= consume;
            left   -= consume;
        }

        // Apply any remaining damage to the Health component
        if (left > 0) Health.TakeDamage(left);
    }

    // Reduces the duration of all active status effects at the end of a combat round
    public void TickEndOfRound()
    {
        // Decay taunt state if the duration has expired
        if (tauntTurns > 0 && --tauntTurns == 0) { isTaunting = false; }
        
        if (atkBuffTurns > 0) atkBuffTurns--;
        if (defBuffTurns > 0) defBuffTurns--;
        if (agiBuffTurns > 0) agiBuffTurns--;
    }
}