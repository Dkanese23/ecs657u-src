using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    [Header("Identity")]
    public string displayName = "Hero";

    [Header("Core Stats")]
    public int Strength = 5;
    public int Agility = 5;
    public int Intelligence = 5;

    [Header("Derived / Effects")]
    public int baseAttack = 2;          // flat base (added to physical)
    public int shield { get; private set; }
    public bool isTaunting;             // forces enemy targeting
    public int tauntTurns;
    public int atkBuffTurns;            // +% or flat; MVP will use flat for simplicity
    public int defBuffTurns;
    public int agiBuffTurns;

    public Health Health { get; private set; }

    void Awake() => Health = GetComponent<Health>();

    public void AddShield(int amount) => shield += Mathf.Max(0, amount);

    public void ReceiveDamage(int amount)
    {
        int left = amount;
        if (shield > 0)
        {
            int consume = Mathf.Min(shield, left);
            shield -= consume;
            left  -= consume;
        }
        if (left > 0) Health.TakeDamage(left);
    }

    public void TickEndOfRound()
    {
        if (tauntTurns > 0 && --tauntTurns == 0) { isTaunting = false; }
        if (atkBuffTurns > 0) atkBuffTurns--;
        if (defBuffTurns > 0) defBuffTurns--;
        if (agiBuffTurns > 0) agiBuffTurns--;
    }
}
