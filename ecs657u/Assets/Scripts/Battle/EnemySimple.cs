using UnityEngine;

public class EnemySimple : MonoBehaviour
{
    public int attackDamage = 3;
    public Health Health { get; private set; }
    void Awake() => Health = GetComponent<Health>();
}
