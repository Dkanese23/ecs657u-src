using UnityEngine;

// A modular health system that uses events to decouple logic from visual representation
public class Health : MonoBehaviour
{
    [SerializeField] int maxHP = 20;
    public int MaxHP => maxHP;
    public int CurrentHP { get; private set; }

    // Events used to notify listeners (UI, Animators) of state changes
    public System.Action OnDeath;
    public System.Action<int, int> OnHealthChanged; // Returns (Current, Max)

    // Initialises health to the maximum value upon object creation
    void Awake() => CurrentHP = maxHP;

    // Reduces health and triggers death logic if life reaches zero
    public void TakeDamage(int amt)
    {
        // Clamp health to zero to prevent negative values
        CurrentHP = Mathf.Max(0, CurrentHP - Mathf.Abs(amt));
        
        // Notify any active UI elements to refresh their health bars
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
        
        if (CurrentHP == 0) OnDeath?.Invoke();
    }

    // Adjusts the character's upper limits, often used for boss scaling or leveling
    public void SetMaxHP(int newMax)
    {
        maxHP = newMax;
        CurrentHP = maxHP; // Restores to full health on maximum increase
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }

    // Increases current health while ensuring it does not exceed the cap
    public void Heal(int amt)
    {
        CurrentHP = Mathf.Min(maxHP, CurrentHP + Mathf.Abs(amt));
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }
}