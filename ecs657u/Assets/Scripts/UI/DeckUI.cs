using UnityEngine;

// Syncs the player's current deck data with the UI display
public class DeckUI : MonoBehaviour
{
    public Transform contentRoot; // Where the cards will live (usually a Grid or Vertical Layout)
    public GameObject cardItemPrefab;

    void OnEnable()
    {
        // Subscribe to the event so we only refresh when the deck actually changes
        if (DeckService.I)
            DeckService.I.OnDeckChanged += Refresh;

        Refresh();
    }

    void OnDisable()
    {
        // Unsubscribe to avoid memory leaks or errors when the UI is closed
        if (DeckService.I)
            DeckService.I.OnDeckChanged -= Refresh;
    }

    void Refresh()
    {
        if (DeckService.I == null) return;

        // Clean out the old card objects before redrawing
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        // Loop through the data and create the physical UI for each card
        foreach (var card in DeckService.I.GetDeckCopy())
        {
            var go = Instantiate(cardItemPrefab, contentRoot);
            var ci = go.GetComponent<CardItem>();
            
            // "Bind" connects the data to the UI button logic using a Lambda
            ci.Bind(card, "Remove", (c) => DeckService.I.RemoveCard(c));
        }
    }
}