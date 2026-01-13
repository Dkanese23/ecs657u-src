using UnityEngine;

[CreateAssetMenu(menuName = "Hero/Hero Data")]
public class HeroData : ScriptableObject
{
    public HeroType heroType;

    [Header("Base Stats")]
    public int maxHealth;
    public float damageMultiplier;

    [Header("Passive Effects")]
    public float lifestealPercent;   // Monk
    public float critChance;         // Assassin
    public float damageReduction;    // Knight

    [Header("Visuals")]
    public GameObject overworldPrefab;
    public Sprite portrait;
}
