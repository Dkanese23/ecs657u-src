using System.Collections.Generic;
using UnityEngine;

// ScriptableObject serving as a persistent data template for the player's card collection
[CreateAssetMenu(menuName = "Cards/Inventory Data")]
public class InventoryData : ScriptableObject
{
    // List of card assets currently owned by the player but doesn't have to be in the active deck
    public List<CardBase> cards = new();
}