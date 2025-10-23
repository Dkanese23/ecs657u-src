using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHP = 20;
    public int MaxHP => maxHP;
    public int CurrentHP { get; private set; }

    public System.Action OnDeath;
    public System.Action<int,int> OnHealthChanged;

    void Awake() => CurrentHP = maxHP;

    public void TakeDamage(int amt)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - Mathf.Abs(amt));
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
        if (CurrentHP == 0) OnDeath?.Invoke();
    }

    public void Heal(int amt)
    {
        CurrentHP = Mathf.Min(maxHP, CurrentHP + Mathf.Abs(amt));
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }
}
