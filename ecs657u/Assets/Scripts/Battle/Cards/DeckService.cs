using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckService : MonoBehaviour
{
    public static DeckService I { get; private set; }

    [Header("Deck / Inventory")]
    public DeckData currentDeck;
    public InventoryData currentInventory;

    private List<CardBase> deckRuntime = new();
    private List<CardBase> inventoryRuntime = new();

    public event Action OnDeckChanged;

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

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

    public void AddCard(CardBase card)
    {
        if (card == null) return;

        // FIX 1: Remove the "Contains" check. 
        // We only care if the inventory actually HAS one to give us.
        if (inventoryRuntime.Contains(card)) 
        {
            deckRuntime.Add(card);
            inventoryRuntime.Remove(card); // Removes the *first* instance found
            
            Debug.Log($"[DeckService] Added card: {card.Title}");
            OnDeckChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning("Tried to add a card not present in Runtime Inventory!");
        }
    }

    public void RemoveCard(CardBase card)
    {
        if (card == null) return;

        // FIX 2: Just check if we have one to remove.
        if (deckRuntime.Contains(card))
        {
            deckRuntime.Remove(card); // Removes the *first* instance found

            // FIX 3: Always add it back to inventory. 
            // Do NOT check (!inventoryRuntime.Contains), or you can't have stacks in inventory!
            inventoryRuntime.Add(card);
            
            Debug.Log($"[DeckService] Removed card: {card.Title}");
            OnDeckChanged?.Invoke();
        }
    }


    // --- ADD THIS TO DeckService.cs ---

    // Call this when picking up a card from the floor
    public void CollectNewCard(CardBase newCard)
    {
        if (newCard == null) return;

        // Add to the "spare cards" list
        inventoryRuntime.Add(newCard);

        Debug.Log($"[DeckService] Collected new card: {newCard.Title}");
        
        // Refresh the UI so the red dot/new card appears
        OnDeckChanged?.Invoke();
    }

    public void ResetToDefaults()
    {
        deckRuntime = new List<CardBase>(currentDeck.cards);
        inventoryRuntime = new List<CardBase>(currentInventory.cards);
        OnDeckChanged?.Invoke();
    }



}