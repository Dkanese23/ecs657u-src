using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckService : MonoBehaviour
{
    public static DeckService I { get; private set; }

    [Header("Deck / Inventory")]
    public DeckData currentDeck;           // Your deck asset
    public InventoryData currentInventory; // New inventory asset

    // runtime copies so we don't edit assets directly
    private List<CardBase> deckRuntime = new();
    private List<CardBase> inventoryRuntime = new();

    public event Action OnDeckChanged;

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // Make safe runtime copies so the assets don’t get modified
        if (currentDeck)
            deckRuntime = new List<CardBase>(currentDeck.cards);

        if (currentInventory)
            inventoryRuntime = new List<CardBase>(currentInventory.cards);
    }

    void Start()
    {
        OnDeckChanged?.Invoke(); // so UI can refresh
    }

    public List<CardBase> GetDeckCopy() => new(deckRuntime);
    public List<CardBase> GetInventoryCopy() => new(inventoryRuntime);

    public void AddCard(CardBase card)
    {
        if (card == null) return;
        if (!deckRuntime.Contains(card))
        {
            deckRuntime.Add(card);
            inventoryRuntime.Remove(card);
            Debug.Log($"[DeckService] Added card: {card.Title}");
            OnDeckChanged?.Invoke();
        }
    }

    public void RemoveCard(CardBase card)
    {
        if (card == null) return;
        if (deckRuntime.Contains(card))
        {
            deckRuntime.Remove(card);
            if (!inventoryRuntime.Contains(card))
                inventoryRuntime.Add(card);
            Debug.Log($"[DeckService] Removed card: {card.Title}");
            OnDeckChanged?.Invoke();
        }
    }

    // Optional helpers to sync back or reset if needed
    public void ResetToDefaults()
    {
        deckRuntime = new List<CardBase>(currentDeck.cards);
        inventoryRuntime = new List<CardBase>(currentInventory.cards);
        OnDeckChanged?.Invoke();
    }
}
