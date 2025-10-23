using UnityEngine;
using System.Collections.Generic; 

public class InventoryUI : MonoBehaviour
{
    public Transform contentRoot;
    public GameObject cardItemPrefab;

    void OnEnable()
    {
        if (DeckService.I)
            DeckService.I.OnDeckChanged += Build;

        Build();
    }

    void OnDisable()
    {
        if (DeckService.I)
            DeckService.I.OnDeckChanged -= Build;
    }

    void Build()
    {
        if (DeckService.I == null) return;

        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var card in DeckService.I.GetInventoryCopy())
        {
            var go = Instantiate(cardItemPrefab, contentRoot);
            var ci = go.GetComponent<CardItem>();
            ci.Bind(card, "Add", (c) => DeckService.I.AddCard(c));
        }
    }


}
