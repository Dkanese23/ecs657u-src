using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Cards/Deck Data")]
public class DeckData : ScriptableObject
{
    public List<CardBase> cards = new();
}
