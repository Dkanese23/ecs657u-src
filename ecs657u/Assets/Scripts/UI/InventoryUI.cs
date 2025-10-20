// InventoryUI.cs
using UnityEngine;
using System.Collections.Generic; 
public class InventoryUI : MonoBehaviour
{
    public List<CardBase> allCards = new();  // fill manually in Inspector for now
    public Transform contentRoot;
    public GameObject cardItemPrefab;

    void Start()
    {
        Build();
    }

    void Build()
    {
        Debug.Log("InventoryUI.Build() running");
        Debug.Log($"Cards in list: {allCards.Count}");
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var card in allCards)
        {
            Debug.Log($"Spawning card: {card?.Title}");
            var go = Instantiate(cardItemPrefab, contentRoot);
            var ci = go.GetComponent<CardItem>();
            ci.Bind(card, "Add", (c) =>
            {
                DeckService.I.AddCard(c);
            });
            Debug.Log($"Spawned card {card.Title} at {go.transform.position} size {go.GetComponent<RectTransform>().rect.size}");
        }
    }
}
