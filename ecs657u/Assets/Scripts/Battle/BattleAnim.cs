using UnityEngine;

public class BattleAnim : MonoBehaviour
{
    [Tooltip("Optional: drag the exact Animator here. If empty, it will auto-find.")]
    public Animator anim;

    static readonly int AttackHash = Animator.StringToHash("Attack");
    static readonly int HitHash    = Animator.StringToHash("Hit");
    static readonly int DieHash    = Animator.StringToHash("Die");

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        if (!anim) anim = GetComponentInChildren<Animator>(true);

        if (!anim)
        {
            Debug.LogError($"[BattleAnim] No Animator found on '{name}' or its children.", this);
            return;
        }

        // Verify parameters exist
        bool hasAttack = HasParam(anim, AttackHash);
        bool hasHit    = HasParam(anim, HitHash);
        bool hasDie    = HasParam(anim, DieHash);

        if (!hasAttack || !hasHit || !hasDie)
        {
            Debug.LogError(
                $"[BattleAnim] Animator on '{anim.gameObject.name}' is missing triggers. " +
                $"Attack:{hasAttack} Hit:{hasHit} Die:{hasDie}. Check parameter names EXACTLY.",
                this
            );
        }
    }

    bool HasParam(Animator a, int hash)
    {
        foreach (var p in a.parameters)
            if (p.nameHash == hash) return true;
        return false;
    }

    public void PlayAttack() { if (anim) { anim.ResetTrigger(AttackHash); anim.SetTrigger(AttackHash); } }
    public void PlayHit()    { if (anim) { anim.ResetTrigger(HitHash);    anim.SetTrigger(HitHash); } }
    public void PlayDie()    { if (anim) { anim.ResetTrigger(DieHash);    anim.SetTrigger(DieHash); } }
}
