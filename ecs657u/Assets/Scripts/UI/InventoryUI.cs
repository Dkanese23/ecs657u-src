using UnityEngine;
using System.Collections.Generic; 

public class InventoryUI : MonoBehaviour
{
    public Transform contentRoot; // The UI container (e.g. ScrollView content)
    public GameObject cardItemPrefab;

    void OnEnable()
    {
        // Keep the inventory in sync whenever the deck service updates
        if (DeckService.I)
            DeckService.I.OnDeckChanged += Build;

        Build();
    }

    void OnDisable()
    {
        // Always clean up events to prevent "ghost" updates in the background
        if (DeckService.I)
            DeckService.I.OnDeckChanged -= Build;
    }

    void Build()
    {
        if (DeckService.I == null) return;

        // Wipe the old UI list before redrawing the current inventory
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        // Instantiate a UI element for every card the player owns but hasn't equipped
        foreach (var card in DeckService.I.GetInventoryCopy())
        {
            var go = Instantiate(cardItemPrefab, contentRoot);
            var ci = go.GetComponent<CardItem>();
            
            // Reuses the same CardItem logic, but tells it to trigger 'AddCard' instead
            ci.Bind(card, "Add", (c) => DeckService.I.AddCard(c));
        }
    }
}