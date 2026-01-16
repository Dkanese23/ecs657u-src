using System.Collections.Generic;
using UnityEngine;

// Controls the dynamic display of the player's hand during combat
public class HandUI : MonoBehaviour
{
    public Transform handRoot;           // Horizontal Layout Group for card positioning
    public GameObject cardButtonPrefab;  // Prefab containing the CardView component
    public int handLimit = 5;

    readonly List<CardBase> hand = new();
    public IReadOnlyList<CardBase> Cards => hand;

    // Purges all card objects from the UI and clears the internal list
    public void Clear()
    {
        foreach (Transform t in handRoot) Destroy(t.gameObject);
        hand.Clear();
    }

    // Instantiates a new card in the UI and binds its click functionality
    public void AddCard(CardBase card, System.Action<CardBase> onClicked)
    {
        if (hand.Count >= handLimit) return;
        hand.Add(card);

        // Create the card UI element and parent it to the hand layout
        var go = Instantiate(cardButtonPrefab, handRoot);
        go.name = $"Card_{card.Title}";
        (go.transform as RectTransform).localScale = Vector3.one;

        var view = go.GetComponent<CardView>();
        if (!view) { Debug.LogError("CardButton prefab missing CardView (Legacy)."); return; }

        // Link the card data to the view and define the callback action
        view.Bind(card, () => onClicked?.Invoke(card));
    }

    // Removes a specific card from the hand and destroys its corresponding UI object
    public void Remove(CardBase c)
    {
        int i = hand.IndexOf(c);
        if (i < 0) return;
        hand.RemoveAt(i);
        
        // Ensure the correct child is destroyed to maintain UI alignment
        Destroy(handRoot.GetChild(i).gameObject);
    }
}