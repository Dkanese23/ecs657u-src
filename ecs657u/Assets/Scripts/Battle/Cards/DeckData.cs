using System.Collections.Generic;
using UnityEngine;

// ScriptableObject acting as a persistent data container for card collections
[CreateAssetMenu(menuName="Cards/Deck Data")]
public class DeckData : ScriptableObject
{
    // List of card assets that form a player's deck or inventory
    public List<CardBase> cards = new();
}