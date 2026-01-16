using UnityEngine;

// Foundational enemy class representing a basic combatant
public class EnemySimple : MonoBehaviour
{
    [Header("Basic Stats")]
    public int attackDamage = 3;

    // Reference to the modular Health system
    public Health Health { get; private set; }

    // Initialises component references before the first frame
    void Awake()
    {
        Health = GetComponent<Health>();
        
        // Defensive check to ensure the Health component is attached
        if (Health == null)
        {
            Debug.LogError($"[EnemySimple] Health component missing on {gameObject.name}!", this);
        }
    }
}