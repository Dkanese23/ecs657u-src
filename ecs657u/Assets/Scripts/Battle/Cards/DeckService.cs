using System;
using System.Collections.Generic;
using UnityEngine;

// Singleton service managing the persistent state of the player's deck and inventory
public class DeckService : MonoBehaviour
{
    public static DeckService I { get; private set; }

    [Header("Deck / Inventory")]
    public DeckData currentDeck;
    public InventoryData currentInventory;

    // Runtime lists to prevent modifying the original ScriptableObject assets
    private List<CardBase> deckRuntime = new();
    private List<CardBase> inventoryRuntime = new();

    // Event triggered to notify UI components of data changes
    public event Action OnDeckChanged;

    void Awake()
    {
        // Implements singleton pattern and ensures persistence across scene transitions
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // Initialise runtime data from persistent asset templates
        if (currentDeck)
            deckRuntime = new List<CardBase>(currentDeck.cards);

        if (currentInventory)
            inventoryRuntime = new List<CardBase>(currentInventory.cards);
    }

    void Start()
    {
        OnDeckChanged?.Invoke(); 
    }

    public List<CardBase> GetDeckCopy() => new(deckRuntime);
    public List<CardBase> GetInventoryCopy() => new(inventoryRuntime);

    // Moves a card from the inventory to the active battle deck
    public void AddCard(CardBase card)
    {
        if (card == null) return;

        if (inventoryRuntime.Contains(card)) 
        {
            deckRuntime.Add(card);
            inventoryRuntime.Remove(card); 
            
            Debug.Log($"[DeckService] Added card: {card.Title}");
            OnDeckChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning("Tried to add a card not present in Runtime Inventory!");
        }
    }

    // Returns a card from the active deck to the inventory
    public void RemoveCard(CardBase card)
    {
        if (card == null) return;

        if (deckRuntime.Contains(card))
        {
            deckRuntime.Remove(card); 
            inventoryRuntime.Add(card);
            
            Debug.Log($"[DeckService] Removed card: {card.Title}");
            OnDeckChanged?.Invoke();
        }
    }

    // Handles logic for picking up new cards during exploration
    public void CollectNewCard(CardBase newCard)
    {
        if (newCard == null) return;

        inventoryRuntime.Add(newCard);
        Debug.Log($"[DeckService] Collected new card: {newCard.Title}");
        
        OnDeckChanged?.Invoke();
    }

    // Reverts runtime lists to their original asset states
    public void ResetToDefaults()
    {
        deckRuntime = new List<CardBase>(currentDeck.cards);
        inventoryRuntime = new List<CardBase>(currentInventory.cards);
        OnDeckChanged?.Invoke();
    }
}