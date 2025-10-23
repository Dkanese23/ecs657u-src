// InventoryData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Inventory Data")]
public class InventoryData : ScriptableObject
{
    public List<CardBase> cards = new();
}
